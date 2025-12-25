using System;

namespace KCP_SERVER.Network
{
    public readonly record struct MetricSample(
        DateTime Timestamp,
        int PacketLoss,
        double AverageLatencyMs,
        int TimeoutCount
    );
}
