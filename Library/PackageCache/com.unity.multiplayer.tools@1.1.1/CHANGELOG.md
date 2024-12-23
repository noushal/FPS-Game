# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2023-09-07

### *Fix*

- Removed empty directories that generated warnings during package import

## [1.1.0] - 2022-11-07

### *Metrics*
- Improve the warning message for throttling, and increase the threshold for throttling a metric from 100 to 1,000 recorded events per frame

### *Misc*
- Fixed compilation warning related to unsupported build targets

### *Network Simulator*

This release adds the Network Simulator to the Multiplayer Tools Package. 
This tool offers a configurable component to simulate adverse network condition. 
Packet delay, jitter, packet loss and loss interval are all parameters that can be configured to simulate different kind of networks.
A set of built-in network scenarios are provided to simulate more complex scenarios. User-defined scenarios are also supported.

For more information about the Network Simulator, please see the [tools documentation](https://docs-multiplayer.unity3d.com/tools/current/install-tools/index.html).

### *Runtime Net Stats Monitor*
- Graphs and Simple Moving Average counters can now be configured to be sampled per-second rather than per-frame
- Fixed an issue where RNSM line graphs could retain incorrect maximum values in some cases

## [1.0.0] - 2022-06-27

### *Runtime Net Stats Monitor*
- Doc-comment fixes based on 1.0 release XML doc validation 

## [1.0.0-pre.8] - 2022-06-15

### *Runtime Net Stats Monitor*
- Clamping numerical values to acceptable limits for public APIs
- Improve generated counter labels
- Prevent an exception when there's only one sample
- Added spacing between divider graph and axis number alignment
- Reusing existing numerical labels when the value doesn't change or barely changed
- Fix incorrect values for gauges in counter display elements using SMA
- Ensure RNSM counters display 1 rather than 1,000 milli
- Use infinity rather than float.Min for counter config bounds
- Reduce vertices in graphs with large sample count

## [1.0.0-pre.7] - 2022-04-27

### *Runtime Net Stats Monitor*

This release adds the Runtime Net Stats Monitor (RNSM) to the Multiplayer Tools Package. This tool offers a configurable component for displaying various network stats in an on-screen display at runtime, including stats from the Netcode for GameObjects package, as well as custom, user-defined stats.

For more information about the Runtime Net Stats Monitor, please see its documentation.

## [1.0.0-pre.6] - 2022-02-28

### *Network Profiler*
- Changed NetworkMessage to use the name of the message in the Type column

### *Metrics*
- Added throttling to event metric types
- Added a system to generate random data for tests
- Refactored underlying data structures to reduce redundancy
- Dramatically reduced runtime allocations caused by dispatching metrics to the profiler by updating the serialization implementation to use native buffers instead of BinaryFormatter
- Deprecated support for String when collecting metric payloads
- Added RTT to server metrics
- Added Packet count to metrics

### *Misc*
- Updated some internals exposed flags to enable some test improvements on NGO side

## [1.0.0-pre.2] - 2021-10-19

- Updated documentation files in preparation for publish

## [1.0.0-pre.1] - 2021-08-18

### *Netcode Profiler*

This release adds the Netcode Profiler to the Multiplayer Tools Package. This tool is used to inspect the network activity of **Netcode for GameObjects**.

#### Activity Section
- View detailed metrics about custom messages, scene transitions, and server logs
- View activity related to individual game objects, including network variable updates, rpcs, spawn and destroy events, and ownership changes

#### Messages Section
- View the raw messages that are being sent to the transport interface

#### Other Usability
- Connect to built players to inspect netcode activity remotely
- Filter results by name, type, number of bytes, and network direction
- Correlate network objects in the profiler with objects in the scene hierarchy
- View key metrics in graph form in the profiler charts
