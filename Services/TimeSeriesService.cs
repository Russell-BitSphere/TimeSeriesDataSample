using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeSeriesDataSample.Repositories;

namespace TimeSeriesDataSample.Services;

/// <summary>
/// High-level API for retrieving decoded time-series data
/// for a given (simulation run, lap, channel).
/// </summary>
public sealed class TimeSeriesService
{
    private readonly IChannelRepository _channelRepository;
    private readonly ITimeSeriesBlobRepository _blobRepository;
    private readonly TimeSeriesDecoder _decoder;

    public TimeSeriesService(
        IChannelRepository channelRepository,
        ITimeSeriesBlobRepository blobRepository,
        TimeSeriesDecoder decoder)
    {
        _channelRepository = channelRepository;
        _blobRepository = blobRepository;
        _decoder = decoder;
    }

    /// <summary>
    /// Returns a decoded time-series for the given simulation run, lap, and channel name.
    /// If no channel or blob exists, an empty sequence is returned.
    /// This matches a pattern where invalid IDs are treated as "no data" rather than errors.
    /// </summary>
    public async Task<IReadOnlyList<double>> GetTimeSeriesAsync(
        Guid simulationRunId,
        int lapNumber,
        string channelName,
        CancellationToken ct = default)
    {
        // Unknown run / lap / channel -> no data.
        var channel = await _channelRepository
            .GetChannelAsync(simulationRunId, lapNumber, channelName, ct)
            .ConfigureAwait(false);

        if (channel is null)
            return Array.Empty<double>();

        var blob = await _blobRepository
            .GetBlobAsync(channel.DataHash, ct)
            .ConfigureAwait(false);

        if (blob is null)
            return Array.Empty<double>();

        return _decoder.DecodeDoubles(blob);
    }
}
