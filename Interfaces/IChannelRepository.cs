using System;
using System.Threading;
using System.Threading.Tasks;
using TimeSeriesDataSample.Domain;

namespace TimeSeriesDataSample.Repositories;

/// <summary>
/// Read-only access to channel metadata stored per (simulation run, lap, channel).
/// Each channel points at a shared time-series blob via a deterministic content hash.
/// </summary>
public interface IChannelRepository
{
    /// <summary>
    /// Lookup the channel for a given simulation run, lap, and channel name.
    /// Returns null if no channel exists (e.g. unknown run id or channel name).
    /// This matches a pattern where invalid IDs are treated as "no data".
    /// </summary>
    Task<Channel?> GetChannelAsync(
        Guid simulationRunId,
        int lapNumber,
        string channelName
        CancellationToken ct = default);
}
