# TimeSeriesDataSample

This repository contains a minimal example of how time-series data was loaded and decoded in a larger simulation-data rework project I previously led.  
The code is synthetic, but it reflects the architectural patterns and data-access workflow used in the real system.

---

## Context

The production system processed a large volume of simulated and real world telemetry.

A key architectural decision was to pre-split the data into laps and channels, rather than store it as a single monolithic dataset. This allowed consumers to load only the specific `(SimulationRunId, LapNumber, ChannelName)` they needed without reading unnecessary data.

Each channel entry referenced a content-addressed binary blob via a deterministic hash. The hash was derived solely from the blob bytes, meaning:

- Identical data always produced the same hash.
- Re-ingesting identical data reused the same blob (idempotent behaviour).
- Millions of small time-series objects could be efficiently deduplicated.

The binary blobs themselves contained tightly packed IEEE-754 doubles, representing individual time-series samples. Higher-level computed metadata (for example, summaries or derived statistics such as average velocity or fastest lap) was stored separately in JSON for flexibility.

Due to NDA and IP restrictions, the actual production implementation cannot be shared. This repository therefore provides a small, representative slice of the design and code style.

---

## What This Example Shows

This repository demonstrates a minimal vertical slice of the read path:

1. Look up a channel using `(SimulationRunId, LapNumber, ChannelName)`.
2. Resolve that channel to a `DataHash`.
3. Fetch the corresponding raw binary blob by hash.
4. Decode the blob into a `List<double>`.
5. Return an empty result when no channel or blob exists (mirroring the real system's behaviour, where invalid IDs were treated as "no data" rather than errors).

Only the **read path** is shown.  
Database and storage implementations are intentionally omitted; only the interfaces and decoding/service logic are included.

---

## Repository Structure

```text
TimeSeriesDataSample/
├── Domain
│   └── Channel.cs
├── Repositories
│   ├── IChannelRepository.cs
│   └── ITimeSeriesBlobRepository.cs
├── Services
│   ├── TimeSeriesDecoder.cs
│   └── TimeSeriesService.cs
└── README.md
