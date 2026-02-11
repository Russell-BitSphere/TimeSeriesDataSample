using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using TimeSeriesDataSample.Domain;

namespace TimeSeriesDataSample.Infrastructure;

/// <summary>
/// Repository responsible for retrieving <see cref="Channel"/> entities
/// from persistent storage using Dapper.
/// </summary>
public sealed class ChannelRepository : IChannelRepository
{
    private readonly IDbConnection _connection;

    /// <summary>
    /// SQL query for selecting a single <see cref="Channel"/> record.
    /// Extracted to keep method bodies clean and maintainable.
    /// </summary>
    private const string SqlGetChannel = @"
        SELECT
            Id,
            SimulationRunId,
            LapNumber,
            ChannelName,
            DataHash
        FROM Channels
        WHERE SimulationRunId = @SimulationRunId
          AND LapNumber = @LapNumber
          AND ChannelName = @ChannelName
        LIMIT 1;";

    /// <summary>
    /// Constructs a new <see cref="ChannelRepository"/> with the given database connection.
    /// The caller is responsible for managing the connection's lifetime.
    /// </summary>
    /// <param name="connection">An open and valid database connection.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="connection"/> is null.</exception>
    public ChannelRepository(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    /// <summary>
    /// Retrieves a single <see cref="Channel"/> matching the given simulation run,
    /// lap number, and channel name. Returns <c>null</c> when no record is found.
    /// </summary>
    /// <param name="simulationRunId">The simulation run identifier.</param>
    /// <param name="lapNumber">The lap index within the run.</param>
    /// <param name="channelName">The time-series channel name (e.g. "speed").</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A matching <see cref="Channel"/> instance, or <c>null</c> if not found.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid input arguments.</exception>
    public async Task<Channel?> GetChannelAsync(
        Guid simulationRunId,
        int lapNumber,
        string channelName,
        CancellationToken ct = default)
    {
        // Validate parameters early to avoid unnecessary DB calls
        if (simulationRunId == Guid.Empty)
            throw new ArgumentException("SimulationRunId cannot be empty.", nameof(simulationRunId));

        if (lapNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(lapNumber), "LapNumber cannot be negative.");

        if (string.IsNullOrWhiteSpace(channelName))
            throw new ArgumentException("ChannelName must be provided.", nameof(channelName));

        // Execute parameterized query. Dapper will bind DB columns
        // directly to the constructor parameters of Channel.
        return await _connection.QueryFirstOrDefaultAsync<Channel>(
            SqlGetChannel,
            new
            {
                SimulationRunId = simulationRunId,
                LapNumber = lapNumber,
                ChannelName = channelName
            });
    }
}
