using System;
using System.Collections.Generic;

namespace TimeSeriesDataSample.Services;

/// <summary>
/// Decodes a binary blob into a sequence of double-precision samples.
/// The blob is expected to contain a packed sequence of IEEE-754 doubles.
/// </summary>
public sealed class TimeSeriesDecoder
{
    public IReadOnlyList<double> DecodeDoubles(byte[] blob)
    {
        if (blob is null || blob.Length == 0)
            return Array.Empty<double>();

        if (blob.Length % sizeof(double) != 0)
        {
            throw new FormatException(
                $"Blob length {blob.Length} is not a multiple of {sizeof(double)} bytes.");
        }

        var span = new ReadOnlySpan<byte>(blob);
        var count = span.Length / sizeof(double);
        var result = new List<double>(count);

        for (var i = 0; i < count; i++)
        {
            var offset = i * sizeof(double);
            var value = BitConverter.ToDouble(span.Slice(offset, sizeof(double)));
            result.Add(value);
        }

        return result;
    }
}
