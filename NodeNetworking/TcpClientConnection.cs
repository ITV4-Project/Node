using NodeNetworking.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NodeNetworking
{
    public class TcpClientConnection
    {
        public readonly TcpTransport Transport;
        public readonly IPEndPoint TargetEndPoint;

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected,
            Sending,
            Dead
        }

        private TcpClient _client;

        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public int FailedReconnectionAttempts = 0;

        private ConnectionState _state = ConnectionState.Disconnected;
        public ConnectionState State { get => _state; private set => _state = value; }
        private readonly object _lock = new object();

        private readonly ILogger _logger;

        public TcpClientConnection(TcpTransport transport, IPEndPoint targetEndPoint, ILogger logger, TcpClient client = null)
        {
            Transport = transport;
            TargetEndPoint = targetEndPoint;
            _logger = logger;
            _client = client;
        }

        private async Task Connect()
        {
            if (State == ConnectionState.Connecting)
            {
                return;
            }
            if (_client is not null && _client.Connected)
            {
                State = ConnectionState.Connected;
                Transport.MessageLoop(_client);
                return;
            }
            if (State == ConnectionState.Dead)
            {
                _logger.LogDebug($"Node {TargetEndPoint} is marked as dead, not connecting");
                return;
            }

            _logger.LogInformation($"Opening connection to {TargetEndPoint}");
            State = ConnectionState.Connecting;
            var client = _client = new TcpClient();

            try
            {
                await client.ConnectAsync(TargetEndPoint.Address, TargetEndPoint.Port).ConfigureAwait(false);
                State = ConnectionState.Connected;
                FailedReconnectionAttempts = 0;
                lock (_lock)
                {
                    if (_client != client)
                    {
                        try
                        {
                            _logger.LogDebug("Closing connection!");
                            client.Close();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error while closing connection!");
                        }
                        return;
                    }
                    _logger.LogDebug($"Succesfully connected to {TargetEndPoint}");
                    Transport.MessageLoop(client);
                }
                
            }
            catch (Exception ex)
            {
                State = ConnectionState.Disconnected;
                FailedReconnectionAttempts++;
                if (FailedReconnectionAttempts >= 3)
                {
                    State = ConnectionState.Dead;
                }
                if (ex is SocketException socketException && socketException.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    _logger.LogError($"Connecting refused by {TargetEndPoint}");
                    return;
                }
                _logger.LogError(ex, "Unexpected error while connecting!");
            }
        }

        private int loopRunning = 0;
        private async Task StartSending()
        {
            if (!_client.Connected)
            {
                State = ConnectionState.Disconnected;
                await Connect();
            }

            _logger.LogDebug("Starting to send messages to {TargetEndPoint}. {MessageQueueLength} messages in queue.",
                TargetEndPoint, _messageQueue.Count);

            if (Interlocked.Exchange(ref loopRunning, 1) == 1)
            {
                return;
            }
            try
            {
                var client = _client;
                State = ConnectionState.Sending;
                while (_messageQueue.Count > 0)
                {
                    if (!_client.Connected)
                    {
                        _logger.LogError($"Lost connecting while sending messages, attempting to reconnect");
                        State = ConnectionState.Disconnected;
                        while (State == ConnectionState.Disconnected)
                        {
                            _logger.LogError($"Attempting to reconnect to {TargetEndPoint} (Attempt {FailedReconnectionAttempts})");
                            await Connect();
                            await Task.Delay(1000);
                        }
                        if (State == ConnectionState.Dead)
                        {
                            _logger.LogError($"Failed to reconnect after {FailedReconnectionAttempts} attempts");
                            Interlocked.Exchange(ref loopRunning, 0);
                            return;
                        }
                    }
                    Message msg;
                    lock (_lock)
                    {
                        if (_client != client)
                        {
                            Interlocked.Exchange(ref loopRunning, 0);
                            return;
                        }
                        if (!_messageQueue.TryPeek(out msg))
                        {
                            _logger.LogError("TryPeek returned false!");
                            Interlocked.Exchange(ref loopRunning, 0);
                            continue;
                        }
                    }
                    var stream = _client.GetStream();
                    var bytes = Serializer.Serialize(msg);
                    _logger.LogDebug($"Sending message: {Encoding.UTF8.GetString(bytes)} of length {bytes.Length}");
                    var lengthBytes = BitConverter.GetBytes(bytes.Length);
                    stream.Write(lengthBytes, 0, lengthBytes.Length);
                    stream.Write(bytes, 0, bytes.Length);
                    //await Serializer.SerializeToStream(stream, msg);
                    //await stream.WriteAsync(bytes, 0, bytes.Length);
                    lock (_lock)
                    {
                        if (_messageQueue.TryPeek(out var _msg) && _msg == msg)
                        {
                            _messageQueue.TryDequeue(out _);
                        }
                    }
                }

                _logger.LogDebug("Message queue for {TargetEndPoint} empty, switching to state {ClientState}.", TargetEndPoint, ConnectionState.Connected);
                State = ConnectionState.Connected;
                Interlocked.Exchange(ref loopRunning, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending messages!");

            }
        }

        //public void Send<T>(IMessage<T> message)
        //{
        //    Send(Serializer.Serialize(message));
        //}

        public async Task Send(Message message)
        {
            _messageQueue.Enqueue(message);

            if (State == ConnectionState.Disconnected || (_client is not null && !_client.Connected))
            {
                await Connect();
            }

            if (State == ConnectionState.Connected)
            {
                StartSending();
            }
            //Send(Serializer.Serialize(message));
        }

        //private void Send(byte[] message)
        //{
        //    _messageQueue.Enqueue(message);
        //    lock (_lock)
        //    {
        //        if (_state == State.Disconnected)
        //            StartConnecting();
        //        else if (_state == State.Connected)
        //            StartSending();
        //    }
        //}

        public void Dispose()
        {
            _logger.LogInformation($"Disposing client connection to {TargetEndPoint}");
            lock (_lock)
            {
                State = ConnectionState.Disconnected;
                _messageQueue.Clear();
                if (_client != null)
                {
                    try {
                        _client.Dispose();
                    } catch { }
                    _client = null;
                }
            }
        }
    }
}
