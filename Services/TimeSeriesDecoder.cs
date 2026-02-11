using System;
using System.Collections.Generic;

namespace TimeSeriesDataSample.Services;

/// <summary>
/// Provides helper methods for decoding binary blobs into numeric time-series data.
/// The blob is expected to contain a packed sequence of IEEE-754 doubles.
/// </summary>
public static class TimeSeriesDecoder
{
    /// <summary>
    /// Decodes a binary blob into a sequence of double-precision samples.
    /// </summary>
    /// <param name="blob">A byte array containing packed IEEE-754 double values.</param>
    /// <returns>A read-only list of decoded double samples.</returns>
    /// <exception cref="FormatException">
    /// Thrown when the blob length is not evenly divisible by the size of <see cref="double"/>.
    /// </exception>
    public static IReadOnlyList<double> DecodeDoubles(byte[] blob)
    {
        // Return an empty list if the blob is null or empty.
        if (blob is null || blob.Length == 0)
            return Array.Empty<double>();

        // Validate that the array length aligns with 8-byte doubles.
        if (blob.Length % sizeof(double) != 0)
        {
            throw new FormatException(
                $"Blob length {blob.Length} is not a multiple of {sizeof(double)} bytes.");
        }

        var span = new ReadOnlySpan<byte>(blob);
        int count = span.Length / sizeof(double);

        var result = new List<double>(count);

        // Decode the bytes into doubles
        for (int i = 0; i < count; i++)
        {
            var value = BitConverter.ToDouble(span.Slice(i * sizeof(double), sizeof(double)));
            result.Add(value);
        }

        return result;
    }
}
