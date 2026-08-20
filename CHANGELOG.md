# Changelog

All notable changes to Real Weather Sync are documented here.
This project follows [Semantic Versioning](https://semver.org/).

## [1.2.0] - 2026-08-20

Two more entries under "Options nobody asked for". Both strictly cosmetic; the weather mapping
itself is unchanged.

### Added

- **Antipode mode.** Fetches the weather for the point diametrically opposite the chosen city
  (`lat' = -lat`, `lon' = lon ± 180`). The resolved city is left alone — only the request
  coordinates are mirrored, so "Lyon" stays Lyon while the sky comes from the South Pacific.
  Verified that Open-Meteo answers for open-ocean points; most antipodes are at sea.
- **"Take me somewhere awful".** One-click jumps to seven famously miserable places — Yakutsk,
  Longyearbyen, Ushuaia, Reykjavik, Mount Washington, Death Valley, Cherrapunji — with hardcoded
  coordinates, so a preset costs no geocoding request. Deliberately not persisted: it is an
  action, not a preference, so it resets to None on restart while the city it applied is kept
  like any other. Doubles as the quickest way to exercise the snow, fog and heavy-rain branches
  of the mapping on demand.

### Notes

- The status block now labels a reading taken from the antipode, alongside the existing
  "Opposite day" label.

## [1.1.0] - 2026-08-13

Usability release. The weather mapping itself is unchanged.

### Added

- **Follow the in-game clock** (opt-in, off by default). Instead of one frozen reading, the mod
  walks the city's last 24 hours of real weather, choosing the hour that matches the in-game
  clock: at 15:00 in game you get the city's most recent real 15:00. Values are interpolated
  continuously between the two bracketing hours, so no fade is involved — the transition settings
  and the manual time shift are disabled while it is on.
  The in-game clock is **read only**: `PlanetarySystem.time` is queried, never assigned, so the
  time, date, season and day/night cycle are untouched. Costs no extra requests — the hourly
  series was already part of the response (`Models/WeatherTimeline.cs`).
- **City search.** A `Search` button lists every geocoding match with region, country and
  coordinates in a dropdown; picking one applies it immediately. The old `Apply City` button is
  still there for the "best match, no questions" path.
- **Recent cities.** The last 10 resolved cities are remembered and offered in a dropdown; a
  pick switches instantly with no lookup. Persisted as a flat delimited string
  (`Settings/FavouriteCities.cs`) rather than a nested structure.
- **`Apply Immediately`** button: refreshes and snaps to the result, and collapses a transition
  already in progress.
- **Transition length slider**, 0-600 s, replacing the hardcoded 120 s constant.
- **Human-readable conditions** in the status panel, from the WMO code
  (`Mapping/WeatherCodes.cs`), fully localised.
- **Temperature unit awareness**: the status panel follows the game's own
  `InterfaceSettings.temperatureUnit` (Celsius / Fahrenheit / Kelvin). Only the display is
  converted; everything internal stays Celsius.
- **"Options nobody asked for"** group:
  - **Time shift**, -24 to +24 hours, reading a past or forecast hour instead of now. Implemented
    by indexing Open-Meteo's hourly series from the location's own local timestamp, so no
    timezone arithmetic happens mod-side. The game clock is not affected.
  - **Opposite day**, mirroring temperature around 15 °C and inverting cloudiness and
    precipitation. Fog is deliberately not inverted.

### Changed

- `IWeatherService.GetCurrentWeatherAsync` became `GetWeatherAsync(lat, lon, timeShiftHours, ct)`.
- `ILocationService` gained `SearchLocationsAsync` for multi-result lookups.
- Source code published at https://github.com/merrinaf-tech/Real-Weather-Sync

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
