using NodeNetworking.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NodeNetworking
{
    public class TcpTransport
    {
        private readonly object _listenerlock = new ();
        private TcpListener _listener;
        private Random random = new();

        public delegate void TcpTransportTickHandler(object sender);
        public event TcpTransportTickHandler OnTick;

        public string ServerId { get; private set; }

        public ConcurrentDictionary<RelayMessage, object> RelayMessages = new ();
        public ConcurrentDictionary<BroadcastMessage, object> BroadcastMessages = new ();
        public IPEndPoint CurrentEndpoint { get; private set; }
        public int TickTime { get; set; } = 250;
        public int Fanout { get; set; } = 3;
        public int MaxRelayCount { get; set; } = 3;

        private readonly ConcurrentDictionary<Guid, object> receivedMessages = new ();
        private readonly ConcurrentDictionary<TcpClient, object> _serverConnections = new ();
        private readonly ConcurrentDictionary<string, TcpClientConnection> _clientConnections = new();
        public readonly ConcurrentDictionary<string, IPEndPoint> ServerIPs = new ();

        private readonly ILogger _logger;
        public TcpTransport(ILogger logger)
        {
            _logger = logger;
            OnTick += DoTick;
        }

        public Task Start() {
			StartListening();
			return Task.CompletedTask;
		}

		public async void StartListening(int port = 0)
        {
            TcpListener listener;
            lock(_listenerlock)
            {
                if (_listener != null)
                {
                    return;
                }
                listener = _listener = new TcpListener(IPAddress.Loopback, port);
            }
            try
            {
                listener.Start();
                _logger.LogInformation($"Listener started on {listener.LocalEndpoint}");
                CurrentEndpoint = listener.LocalEndpoint as IPEndPoint;
                while (true)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _logger.LogInformation($"Incoming connection from {client.Client.RemoteEndPoint};");

                    _serverConnections.TryAdd(client, null);
                    MessageLoop(client);
                }
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while starting listener!");
                listener.Stop();
            }
        }

        private bool mainLoopRunning = false;
        private object mainLoopLock = new();
        public async void MainLoop()
        {
            lock(mainLoopLock)
            {
                if (mainLoopRunning)
                {
                    return;
                }

                mainLoopRunning = true;
            }
            try
            {
                while (true)
                {
                    await Task.Delay(TickTime);
                    OnTick(this);
                }
            }
            finally
            {
                lock (mainLoopLock)
                {
                    mainLoopRunning = false;
                }
            }
        }

        public void DoTick(object sender)
        {
            UpdateClientConnections();
            UpdateRelayMessages();
            BroadcastTick();
        }

        public void UpdateRelayMessages()
        {
            var count = RelayMessages.Count;
            if (count == 0)
            {
                return;
            }
            _logger.LogDebug($"Relaying {count} messages");
            foreach(var message in RelayMessages.Keys)
            {
                message.TimesRelayed++;
                foreach (var server in GetRandomServers(Fanout))
                {
					_ = server.Send(message.Message);
                }
                
                if (message.TimesRelayed >= MaxRelayCount)
                {
                    RelayMessages.TryRemove(message, out _);
                }
            }
        }

        public void UpdateClientConnections()
        {
            using (_logger.BeginScope("Updating client connections"))
            {
                foreach (var key in ServerIPs)
                {
                    TcpClientConnection conn;
                    if (!ServerIPs.TryGetValue(key.Key, out var endpoint))
                    {
                        if (_clientConnections.TryRemove(key.Key, out conn))
                        {
                            conn.Dispose();
                        }
                    }
                    else if (!_clientConnections.TryGetValue(key.Key, out conn) && key.Value != CurrentEndpoint)
                    {
                        _clientConnections[key.Key] = new TcpClientConnection(this, endpoint, _logger);

                        _ = _clientConnections[key.Key].Send(new HelloMessage { Port = CurrentEndpoint.Port, }.ToMessage());
                    }
                }
                foreach(var conn in _clientConnections.ToList())
                {
                    if (conn.Value.State == TcpClientConnection.ConnectionState.Dead)
                    {
                        conn.Value.Dispose();
                        _clientConnections.TryRemove(conn.Key, out var _);
                        ServerIPs.TryRemove(conn.Key, out var _);
                        _logger.LogInformation($"Connecting to node {conn.Key} failed after three attempts, removing node");
                    }
                }
            }
        }

        public void Broadcast<T>(IMessage<T> msg)
        {
            Broadcast(msg.ToMessage());
        }

        public void Broadcast(Message msg)
        {
            BroadcastMessages.TryAdd(new BroadcastMessage { Message = msg }, null);
        }

        private void BroadcastTick()
        {
            try
            {
                foreach (var msg in BroadcastMessages.Keys)
                {
                    foreach (var conn in GetRandomServers(Fanout))
                    {
                        _ = conn.Send(msg.Message);
                    }
                    BroadcastMessages.TryRemove(msg, out _);
                }
            } catch(Exception e)
            {
                _logger.LogError(e, "Error while running BroadcastTick!");
            }
        }

        public void Send<T>(string serverKey, IMessage<T> msg)
        {

        }

        public void Send(string serverKey, Message msg) {
            if (_clientConnections.TryGetValue(serverKey, out var tcpClient))
            {

            }
        }

        public IEnumerable<TcpClientConnection> GetRandomServers(int count = 3)
        {
            if (_clientConnections.Count == 0)
            {
                yield break;
            }

            for (int i = 0; i < count; i++)
            {
                var server = _clientConnections.ElementAt(random.Next(0, _clientConnections.Count - 1)).Value;
                yield return server;
            }
        }

        public Task RelayMessage(Message message)
        {
            RelayMessages[new RelayMessage { Message = message }] = null;
            return Task.CompletedTask;
        }

        public async Task HandleMessage(TcpClient client, Message message)
        {
            if (receivedMessages.ContainsKey(message.guid))
            {
                return;
            }

            receivedMessages.TryAdd(message.guid, null);
            switch (message.Type)
            {
                case MessageTypes.Hello:
                    var helloMessage = message.Deserialize<HelloMessage>();
                    var endpoint = new IPEndPoint((client.Client.RemoteEndPoint as IPEndPoint).Address, helloMessage.Port);
                    ServerIPs.TryAdd(endpoint.ToString(), endpoint);

                    if (!_clientConnections.TryGetValue(endpoint.ToString(), out var tcpClient))
                    {
                        tcpClient = new TcpClientConnection(this, endpoint, _logger, client);
                        _clientConnections[endpoint.ToString()] = tcpClient;
                    }

                    var response = new HelloMessageResponse { Address = endpoint.Address.GetAddressBytes(), ServerKey = endpoint.ToString(), Port = endpoint.Port };
                    foreach (var ip in ServerIPs)
                    {
                        response.ServerIps.Add(new IPAddressSerializible(ip.Value));
                    }

                    await tcpClient.Send(response.ToMessage());

                    if (helloMessage.Address == null)
                    {
                        helloMessage.Address = endpoint.Address.GetAddressBytes();
                    }

                    break;

                case MessageTypes.HelloResponse:
                    var helloMessageResponse = message.Deserialize<HelloMessageResponse>();
                    foreach (var ipAddress in helloMessageResponse.ServerIps)
                    {
                        if (!ServerIPs.ContainsKey(ipAddress.ServerKey))
                        {
                            ServerIPs.TryAdd(ipAddress.ServerKey, new IPEndPoint(new IPAddress(ipAddress.Address), ipAddress.Port));
                        }
                    }
                    ServerId = helloMessageResponse.ServerKey;
                    ServerIPs[helloMessageResponse.ServerKey] = new IPEndPoint(new IPAddress(helloMessageResponse.Address), helloMessageResponse.Port);
                    var newServerMessage = new NewServer { ServerKey = helloMessageResponse.ServerKey, Address = helloMessageResponse.Address, Port = helloMessageResponse.Port };
                    Broadcast(newServerMessage);
                    break;

                case MessageTypes.NewServer:
                    var newServer = message.Deserialize<NewServer>();
                    _logger.LogDebug($"NewServer message received in {newServer.ServerKey}: {newServer.Address}:{newServer.Port}");
                    ServerIPs[newServer.ServerKey] = new IPEndPoint(new IPAddress(newServer.Address), newServer.Port);
                    await RelayMessage(message);
                    break;

                case MessageTypes.Test:
                    var testMessage = JsonSerializer.Deserialize< TestMessage >(message.Data);

                    _logger.LogInformation($"Received testmessage: {testMessage.Message}");
                    await RelayMessage(message);

                    break;

                default:
                    _logger.LogWarning($"Unknown message type in {message.Type}");
                    break;
            }
        }

        public async Task HandleMessage(TcpClient client, string str)
        {
            var message = Serializer.Deserialize(str);

            await HandleMessage(client, message);
        }

        private ConcurrentDictionary<TcpClient, object> runningLoops = new ConcurrentDictionary<TcpClient, object>();
        internal async void MessageLoop(TcpClient client)
        {
            if (runningLoops.ContainsKey(client))
            {
                return;
            }
            runningLoops.TryAdd(client, null);
            try
            {
                var stream = client.GetStream();
                var arrayPool = ArrayPool<byte>.Shared;
                var lengthBuffer = new byte[sizeof(int)];
                async Task<(int, byte[], string)> ReadBuffer(NetworkStream stream)
                {
                    byte[] buffer = null;
                    try
                    {
                        var read = await stream.ReadAsync(lengthBuffer, 0, sizeof(int));
                        if (read == 0)
                        {
                            return (0, null, null);
                        }
                        
                        var length = BitConverter.ToInt32(lengthBuffer, 0);
                        read = await stream.ReadAsync(lengthBuffer, 0, sizeof(int));

                        var nameLength = BitConverter.ToInt32(lengthBuffer, 0);
                        if (read == 0)
                        {
                            return (0, null, null);
                        }

                        buffer = arrayPool.Rent(nameLength);
                        _ = await stream.ReadAsync(buffer, 0, nameLength);
                        var eventName = Encoding.UTF8.GetString(buffer, 0, nameLength);
                        arrayPool.Return(buffer);
                        buffer = null;

                        _logger.LogDebug($"Received message of length {length}");
                        buffer = arrayPool.Rent(length);
                        read = await stream.ReadAsync(buffer, 0, length);
                        if (read == 0)
                        {
                            return (0, null, null);
                        }
                        return (read, buffer, eventName);
                    }
                    catch (Exception e)
                    {
                        if (e is System.IO.IOException ioException)
                        {
                            if (ioException.InnerException is System.Net.Sockets.SocketException socketException && socketException.SocketErrorCode == SocketError.ConnectionReset) {
                                _logger.LogError($"Lost connection to {client.Client.RemoteEndPoint}");
                                if (buffer is not null)
                                {
                                    arrayPool.Return(buffer);
                                }
                                return (0, null, null);
                            }
                        }
                        _logger.LogError(e, $"Exception while reading data!");
                        if (buffer is not null) {
                            arrayPool.Return(buffer);
                        }
                        return (0, null, null);
                    }
                }

                //async Task<Message> ReadMessage(NetworkStream stream)
                //{
                //    try
                //    {
                //        return await Serializer.DeserializeFromStream(stream);
                //    }
                //    catch(Exception e)
                //    {
                //        _logger.LogError(e, "Error while reading Message");
                //    }
                //    return null;
                //}

                var clientEndpoint = client.Client.RemoteEndPoint;
                while (client.Connected)
                {
                    var (read, buffer, eventName) = await ReadBuffer(stream);
                    if (read == 0)
                    {
                        _logger.LogError($"Failed to read from client: {clientEndpoint}");
                        _serverConnections.TryRemove(client, out var _);
                        return;
                    }

                    var span = new ReadOnlyMemory<byte>(buffer, 0, read);
                    var message = Serializer.Deserialize(span.Span);
                    arrayPool.Return(buffer);
                    await HandleMessage(client, message);
                }
            }
            finally
            {
                runningLoops.TryRemove(client, out _);
            }
        }

        public void Dispose()
        {
            foreach (var client in _clientConnections)
            {
                client.Value.Dispose();
            }
            foreach(var server in _serverConnections)
            {
                server.Key.Dispose();
            }
        }
    }
}
