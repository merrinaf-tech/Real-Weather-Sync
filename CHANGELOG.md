# Changelog

All notable changes to Real Weather Sync are documented here.
This project follows [Semantic Versioning](https://semver.org/).

## [1.0.0] - 2026-08-06

First public release, published to Paradox Mods. Built with the official Cities: Skylines II
modding toolchain against game build `1.6.0f1`.

### Added

- Resolve a real city typed in the mod options to coordinates via the Open-Meteo geocoding API.
  Supports `City` and `City, Country` / `City, Region` forms; geocoding runs only on **Apply City**.
- Fetch current conditions from the Open-Meteo forecast API using `temperature_2m`,
  `relative_humidity_2m`, `is_day`, `precipitation`, `rain`, `showers`, `snowfall`, `weather_code`,
  `cloud_cover`, plus hourly `visibility` sampled at the current hour. Celsius and millimetres.
- Override four visual climate values through `Game.Simulation.ClimateSystem`: `temperature`,
  `cloudiness`, `precipitation` and `fog`.
- `WeatherMapper`: documented, game-independent mapping with piecewise-linear rain and snow
  curves, a WMO weather-code intensity floor, a conservative visibility-based fog curve, and
  clamping into the game-supported ranges.
- Smooth transitions over 120 seconds driven by real wall-clock time, unaffected by pause or
  simulation speed. A reading arriving mid-transition continues from the current interpolated
  values.
- Options page: `Enable Real Weather`, `City`, `Apply City`, `Smooth Weather Transitions`,
  `Update Interval` (15 / 30 / 60 minutes, default 15), `Refresh Weather Now`,
  `Reset to Game Weather`.
- Advanced options: `Synchronise fog`, `Show snow when it is really snowing`,
  `Ignore mod conflicts`.
- Live read-only status in the options page: state, resolved location, last update, and a block
  comparing observed values with the values applied to the game.
- Asynchronous networking on a single reusable `HttpClient` with a 15 second timeout,
  single-flight requests, cancellation, and 30 s / 60 s / 120 s / 300 s / 600 s retry backoff
  (rate limiting jumps to the longest delay). No blocking calls anywhere.
- Last known good weather retained in memory, so a failed refresh never produces zeroed or
  extreme values.
- Best-effort detection of other weather-overriding mods; when one is found, Real Weather Sync
  releases its own overrides and reports the conflict.
- Overrides released on: mod disabled, `Reset to Game Weather`, mod disposal, returning to the
  menu, entering the editor, invalid configuration, and detected incompatibility.
- en-US localisation for every player-facing string, including the dynamically built status text.
- Project builds with or without the official CS2 modding toolchain; without it, the game
  assemblies are referenced directly and the output is copied into the local mods folder.

### Not included, by design

Simulation changes, clock / date / season / day-night synchronisation, thunder, lightning,
rainbow, aurora, hail, wind, disasters, planetary latitude / longitude, and any savegame
serialisation.
