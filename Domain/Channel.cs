using System;

namespace TimeSeriesDataSample.Domain;

/// <summary>
/// Represents a single time-series channel within a simulation run and lap,
/// pointing at shared time-series data via a deterministic content hash.
/// </summary>
public sealed class Channel
{
    /// <summary>
    /// Unique identifier for this channel record.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identifier for the simulation run this channel belongs to.
    /// </summary>
    public Guid SimulationRunId { get; }

    /// <summary>
    /// Logical segment within the run (e.g. lap index).
    /// </summary>
    public int LapNumber { get; }

    /// <summary>
    /// Name of the time-series channel (e.g. "speed", "throttle").
    /// </summary>
    public string ChannelName { get; }

    /// <summary>
    /// A deterministic content hash of the underlying time-series blob.
    /// Identical blob content always results in the same hash (idempotent).
    /// The hash is derived from the blob bytes only, so ingesting the same
    /// data multiple times produces the same hash and reuses the same blob.
    /// </summary>
    public string DataHash { get; }

    public Channel(
        Guid simulationRunId,
        int lapNumber,
        string channelName,
        string dataHash)
    {
        Id = Guid.NewGuid();
        SimulationRunId = simulationRunId;
        LapNumber = lapNumber;

        if (string.IsNullOrWhiteSpace(channelName))
            throw new ArgumentException("Channel name must be provided.", nameof(channelName));

        if (string.IsNullOrWhiteSpace(dataHash))
            throw new ArgumentException("Data hash must be provided.", nameof(dataHash));

        ChannelName = channelName;
        DataHash = dataHash;
    }
}
