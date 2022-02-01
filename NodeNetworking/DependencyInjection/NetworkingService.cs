using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NodeNetworking.DependencyInjection
{
    public class NetworkingService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _factory;
        private readonly GossipProtocolOptions _options;
        private readonly IConfiguration _configuration; 

        private readonly TcpTransport _tcpTransport;

        public NetworkingService(ILoggerFactory factory, IOptions<GossipProtocolOptions> options, IConfiguration configuration)
        {
            _logger = factory.CreateLogger("GossipProtocol");
            _factory = factory;
            _options = options.Value;
            _configuration = configuration;

            _tcpTransport = new TcpTransport(_logger);
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken) {
            if (_options.SeedServers is not null && _options.SeedServers.Count > 0) {
                foreach (var item in _options.SeedServers) {
                    if (IPEndPoint.TryParse(item, out var endpoint)) {
                        _tcpTransport.ServerIPs.TryAdd(endpoint.ToString(), endpoint);
                    } else {
                        _logger.LogError($"Invalid IP {item} in SeedServers!");
                    }
                }
            }

            _tcpTransport.Fanout = _options.Fanout;
            _tcpTransport.TickTime = _options.TickTime;
            _tcpTransport.MaxRelayCount = _options.MaxRelayCount;

            int port;
            if (_configuration["Port"] != null) {
                port = int.Parse(_configuration["Port"]);
            } else {
                port = _options.ListenPort;
			}

            _tcpTransport.StartListening(port);
            _ = _tcpTransport.MainLoop();
            _logger.LogDebug($"GossipProtocol started");

            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _tcpTransport.Dispose();
            return Task.CompletedTask;
        }
    }
}
