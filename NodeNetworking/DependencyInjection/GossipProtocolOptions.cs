using System.Collections.Generic;

namespace NodeNetworking.NodeNetworking.DependencyInjection;

public class GossipProtocolOptions
{
    public List<string> SeedServers { get; set; }
    public int ListenPort { get; set; } = 7456;
    public int Fanout { get; set; } = 3;
    public int TickTime { get; set; } = 2000;
    public int MaxRelayCount { get; set; } = 3;
}