using System.Threading;
using System.Threading.Tasks;

namespace TimeSeriesDataSample.Repositories;

/// <summary>
/// Read-only access to raw time-series blobs, keyed by a content hash.
/// The same blob can be shared by many channels when their data is identical.
/// </summary>
public interface ITimeSeriesBlobRepository
{
    /// <summary>
    /// Returns the raw time-series blob for the given hash, or null if it does not exist.
    /// </summary>
    Task<byte[]?> GetBlobAsync(
        string dataHash,
        CancellationToken ct = default);
}
