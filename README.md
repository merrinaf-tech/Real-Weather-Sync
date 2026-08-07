# Real Weather Sync

A Cities: Skylines II code mod that makes your city look like the current real weather of a
city you choose. Type `Lyon`, press **Apply City**, and the game's sky, temperature, rain and
fog follow Lyon's actual conditions.

**This mod is strictly cosmetic.**

---

## What it does

- Resolves the city name you type into coordinates, using Open-Meteo's free geocoding service.
- Fetches the current conditions for those coordinates every 15, 30 or 60 minutes (your choice).
- Overrides exactly four visual climate values through `Game.Simulation.ClimateSystem`:
  - `temperature`
  - `cloudiness`
  - `precipitation`
  - `fog`
- Fades smoothly between readings over about two minutes of **real** time.
- Releases every override the moment you disable the mod, reset it, leave to the menu, or the mod
  is unloaded.

## What it does not do

It does not touch, and contains no code that could touch:

- the simulation - citizens, traffic, energy, economy, services, growth;
- disasters, thunder, lightning, rainbows, auroras, hail or wind;
- the game clock, the date, the season or the day/night cycle;
- the in-game planet's latitude / longitude (`PlanetarySystem` is never referenced);
- savegame data - nothing this mod produces is serialised into a city save.

The only game state it writes is the `overrideValue` / `overrideState` pair of the four
properties listed above, which is exactly what the game's own developer weather tools write.

---

## Installation

### From a build on this machine

The build already copies the mod into the game's local mods folder:

```
%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Mods\RealWeatherSync\RealWeatherSync.dll
```

Start the game, open **Options -> Modding**, and make sure `Real Weather Sync` is enabled in
your active playset. Then open **Options -> Real Weather Sync**.

### Manual

Copy `RealWeatherSync.dll` into a folder named `RealWeatherSync` under
`%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Mods\`.

---

## Configuration

Everything lives on one options page, **Options -> Real Weather Sync**.

### General

| Setting | Meaning |
|---|---|
| **Enable Real Weather** | Master switch. Turning it off hands the weather straight back to the game. |
| **City** | The real city to copy. `Lyon`, `Lyon, France`, `Milazzo, Italy`, `New York, United States`. |
| **Smooth Weather Transitions** | Fade to each new reading over ~120 seconds of real time instead of snapping. |
| **Update Interval** | 15 / 30 / 60 minutes. Default 15. |

### Actions

| Button | Meaning |
|---|---|
| **Apply City** | Geocode the text in *City*, store the coordinates, and fetch the weather immediately. |
| **Refresh Weather Now** | Fetch current conditions without waiting for the next interval. Disabled until a city is resolved. |
| **Reset to Game Weather** | Release all overrides and let the game drive the weather again. Real weather resumes on the next **Apply City** or **Refresh Weather Now**. |

### Status (read-only)

`Status`, `Resolved location`, `Last update` and a `Current weather` block showing both the
observed values and the values actually applied to the game. These refresh live while the
options page is open.

Possible statuses: *Disabled*, *City not configured*, *Resolving location*, *Refreshing weather*,
*Connected*, *Offline - using last valid weather*, *Error resolving city*,
*Incompatible weather mod active*, *Overrides released - using game weather*,
*Waiting for a city to be loaded*.

### Advanced

| Setting | Default | Meaning |
|---|---|---|
| **Synchronise fog** | on | Derive fog from fog weather codes and visibility. Turn off to leave the game's fog alone. |
| **Show snow when it is really snowing** | on | See [Snow](#snow-and-its-one-unavoidable-compromise) below. |
| **Ignore mod conflicts** | off | Skip the other-weather-mod check. See [Compatibility](#compatibility). |

### The normal workflow

1. Enter a city.
2. Press **Apply City**.
3. Done.

No latitude/longitude, no API key, no config files, no weather codes, no manual sliders.

---

## City resolution

`Apply City` trims your input, splits it on the first comma into a name and an optional
country/region qualifier, and asks Open-Meteo's geocoder for up to 10 candidates.

- **No qualifier** (`Lyon`): the best ranked result wins.
- **With a qualifier** (`Lyon, France`, `Springfield, Illinois`): the best ranked candidate whose
  country, country code, region or sub-region matches the qualifier wins. If nothing matches,
  the lookup reports "no matching city found" rather than silently returning a city on another
  continent.

The resolved name, region, country, timezone and coordinates are stored in the mod's settings.
**Geocoding runs only when you press Apply City** - weather refreshes reuse the stored
coordinates and never re-geocode.

If a lookup fails or finds nothing:

- the previously resolved location is kept,
- the error is shown in *Status* and written to the log,
- the game is never destabilised.

The timezone is stored and displayed for diagnostics only. The game clock is never synchronised.

---

## Weather mapping

All conversion constants live in a single file, `Mapping/WeatherMapper.cs`, which has no game
dependencies. Every value is clamped into the range the game accepts.

Game-side ranges were read out of `Game.Simulation.ClimateSystem` in the installed build:
`temperature` is degrees Celsius (developer UI exposes -50..50), and `cloudiness`,
`precipitation` and `fog` are all 0..1.

### Temperature

The real Celsius value, clamped to -50..50 C. No scaling, no offset (except the snow case below).

### Cloudiness

`cloud_cover` percent divided by 100.

| Real | Game |
|---|---|
| 0 % | 0.00 - clear |
| 50 % | 0.50 - partly/mostly cloudy |
| 100 % | 1.00 - overcast |

### Precipitation

Open-Meteo reports precipitation as millimetres accumulated over the preceding hour. That is
mapped through a piecewise-linear curve, deliberately compressed at the bottom so a drizzle looks
like a drizzle:

| mm/h | Game value | Reads as |
|---|---|---|
| 0.0 | 0.00 | dry |
| 0.1 | 0.10 | very light / drizzle |
| 0.5 | 0.25 | light rain |
| 2.5 | 0.50 | moderate rain |
| 7.5 | 0.75 | heavy rain |
| 20.0+ | 1.00 | violent rain |

Snowfall is reported in **centimetres**, not millimetres of water, and snow is far more visible
per unit of water than rain. It therefore gets its own, steeper curve:

| cm/h | Game value |
|---|---|
| 0.0 | 0.00 |
| 0.1 | 0.15 |
| 0.5 | 0.35 |
| 1.0 | 0.55 |
| 3.0 | 0.80 |
| 6.0+ | 1.00 |

The stronger of the two curves wins. `precipitation`, `rain`, `showers` and `snowfall` are all
used: `precipitation` is the primary driver, with `rain + showers + snow-water-equivalent`
(0.7 mm of water per cm of snow) as a fallback when it is missing.

Finally, the WMO `weather_code` supplies a **floor**, so a reported shower is never drawn as a
dry sky just because the hourly accumulation bucket has not filled yet - 0.08 for light codes,
0.20 for moderate, 0.38 for dense, 0.60 for heavy. Measured amounts always win when they are
larger.

### Snow, and its one unavoidable compromise

Cities: Skylines II decides whether precipitation is drawn as rain or as snow from the **visual
temperature**, compared against `ClimateSystem.freezingTemperature`. There is no separate
"draw snow" switch a mod can set.

So when Open-Meteo reports snow while the air temperature is above the game's freezing point,
faithfully applying the real temperature would make the game draw rain.

With **Show snow when it is really snowing** enabled (the default), the mod lowers the *visual*
temperature to 1.5 C below the game's freezing point in exactly that case. The consequence, stated
plainly: **the temperature shown in game will then differ from the real temperature.** Turn the
setting off if you would rather have an accurate temperature reading than accurate precipitation.

The season, the date and the simulation are still never changed. Nothing forces winter.

### Fog

Fog is deliberately conservative, and **cloud cover never produces fog**.

1. WMO code 45 (fog) -> 0.72; code 48 (depositing rime fog) -> 0.82.
2. Otherwise, from visibility (an hourly Open-Meteo variable, sampled at the current hour):

   | Visibility | Fog |
   |---|---|
   | >= 10 km | 0.00 |
   | 4 km | 0.20 |
   | 1 km | 0.50 |
   | 500 m | 0.70 |
   | 100 m | 0.90 |

3. If visibility is low but there is no fog code *and* it is precipitating, fog is capped at 0.25 -
   heavy rain reduces visibility without being fog.

If Open-Meteo returns no visibility sample for the current hour, fog falls back to the weather
code alone.

### Deliberately not controlled

Thunder, lightning, rainbow, aurora, hail, wind, disasters, season, date, game time, and the
planet's latitude / longitude. None of these are written anywhere in the mod.

---

## Transitions

- Default duration: 120 seconds.
- Driven by a `System.Diagnostics.Stopwatch`, i.e. **real wall-clock time**. Simulation speed does
  not stretch or compress a transition, and pausing the game does not freeze one.
- A new reading arriving mid-transition starts from the **currently interpolated** values, not
  from the previous starting values, so there is never a visible jump backwards.
- Temperature, cloudiness, precipitation and fog are all interpolated together.
- Turning **Smooth Weather Transitions** off sets the duration to zero (instant).
- Loading a save or returning to the menu resets the transition state and releases the overrides;
  the first application after loading fades in from whatever the game is currently showing.

---

## Networking

- One reusable `HttpClient` for the mod's lifetime, 15 second timeout.
- Fully asynchronous. There is no `.Result`, no `.Wait()`, no busy waiting, no per-frame request.
- At most one request in flight. Automatic refreshes are skipped while one is running; pressing
  a button cancels the running request and chains the new one behind it, so tasks cannot pile up.
- No game API is touched from a background thread. Results are handed to the main thread by
  polling, and only the ECS system writes to `ClimateSystem`.
- Retry backoff after a failure: 30 s, 60 s, 120 s, 300 s, 600 s. Rate limiting (HTTP 429) jumps
  straight to the longest delay. The API is never hammered.

Handled failure modes: no internet, DNS failure, TLS failure, HTTP errors, timeouts, malformed
JSON, missing fields, rate limiting, cancellation during disposal, and the city being changed
while a request is still running.

The last valid weather is kept in memory. **A failed refresh never produces zeroed or extreme
values** - the game simply keeps showing the last good reading, and the status changes to
*Offline - using last valid weather*.

---

## Privacy

Real Weather Sync uses [Open-Meteo](https://open-meteo.com/). No account, no sign-up, no API key.

Exactly two things leave your machine:

1. **The city name you type**, sent to `https://geocoding-api.open-meteo.com/v1/search` when you
   press *Apply City*.
2. **The latitude and longitude** that lookup returned, sent to
   `https://api.open-meteo.com/v1/forecast` on each refresh.

Nothing else is collected, stored remotely or transmitted. No telemetry, no analytics, no
identifiers. The city name and the coordinates are also written to the mod's log file on your own
machine, as diagnostics.

Weather data by Open-Meteo, licensed CC BY 4.0.

---

## Update frequency and offline behaviour

- Refreshes happen every 15 / 30 / 60 minutes of real time (Open-Meteo itself updates roughly
  every 15 minutes, so shorter would gain nothing).
- Loading a city triggers a refresh promptly rather than waiting out the previous interval.
- With no connection, the mod keeps applying the last good weather and reports *Offline*. It
  retries with the backoff above and recovers on its own when connectivity returns.
- With no connection and no weather ever received, the mod applies nothing at all and the game's
  own weather is left untouched.

---

## Compatibility

> **Do not run Real Weather Sync at the same time as another mod that overrides the visual
> climate** - Time & Weather Anarchy, Weather Plus, Nice Weather, day/night switch mods, and
> similar. Two mods writing the same `ClimateSystem` properties will overwrite each other and the
> result depends on system ordering.

Real Weather Sync tries to protect you from that: on each load it scans the loaded mods for known
weather-overriding mods by name and assembly name. If it finds one it **releases its own
overrides**, logs a clear warning, and shows *Incompatible weather mod active* in the status.

**The limits of that detection, stated honestly:**

- It matches on names. A renamed mod, or any weather mod not in the list, is not detected.
- Presence is not proof of conflict. A detected mod that never enables its own overrides would
  not actually fight us, and there is no way to tell from outside.
- The mod manager's shape is not a documented stable contract. If enumeration ever fails, the
  check reports "no conflict" rather than blocking you.

**Compatibility with other weather mods has not been implemented or tested.** The detection is a
guard rail, not a compatibility layer. If you hit a false positive, `Ignore mod conflicts` in the
Advanced group turns the check off.

Real Weather Sync does **not** require Time & Weather Anarchy or any other mod. It uses its own
namespace, assembly name, settings key and settings file, and shares no assets.

No Harmony patching is used. The mod uses only public game and modding APIs.

---

## Known limitations

1. **Rain vs snow is decided by the game from temperature.** See
   [Snow](#snow-and-its-one-unavoidable-compromise). This is a property of the engine, not
   something a mod can work around without distorting the temperature.
2. **Fog depends on data availability.** Open-Meteo exposes visibility as an hourly variable, not
   a current one. The mod samples the hourly series at the current hour; when no sample lines up,
   fog falls back to the weather code alone and will be 0 for most conditions.
3. **Conflict detection is name-based** and cannot be complete (see above).
4. **Transition duration is fixed at 120 seconds** and is not exposed as a setting.
5. **Editor is excluded.** Overrides are only applied when `GameMode.IsGame()` is true.
6. Open-Meteo's "current" values describe the preceding hour for accumulated quantities
   (precipitation, snowfall), so very short showers may be smoothed out.
7. **The Burst artifacts shipped alongside the assembly embed the build machine's temporary
   Burst path**, which contains the developer's Windows user name. This is emitted by the
   official `ModPostProcessor` and is present in effectively every published Cities: Skylines II
   mod. The mod assembly itself is built with `PathMap` and carries no local paths.

---

## Build instructions

### Requirements

- .NET SDK 8 or newer (`dotnet build`).
- Cities: Skylines II installed.
- The **official Cities: Skylines II Modding Toolchain**, installed from the game's
  *Options → Modding*. It sets `CSII_TOOLPATH` and provides `Mod.props`, `Mod.targets`,
  `ModPostProcessor.exe` and `ModPublisher.exe`.

The project imports the official `Mod.props` / `Mod.targets` when `CSII_TOOLPATH` is set, and
then uses the toolchain's own post-processing, deploy and publish pipeline. If the toolchain is
absent it falls back to referencing `Cities2_Data\Managed` directly and copying the output to the
local Mods folder itself, so the project still builds — but a release must be built with the
toolchain, because only `ModPostProcessor` runs Unity's IL post-processing.

`CSII_TOOLPATH` is read from the *user* environment target rather than the process environment,
so a terminal opened before the toolchain was installed still builds correctly.

### Build

```bash
dotnet build RealWeatherSync.sln -c Release
```

The build runs `ModPostProcessor`, strips the PDB, and deploys to
`%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Mods\RealWeatherSync\`. A guard target fails
the build if anything other than the mod assembly and the official Burst artifacts would end up
in that folder, because that folder is exactly what gets uploaded.

Note: Release builds still produce a PDB during compilation. Unity's `EntitiesILPostProcessors`
reads `InMemoryAssembly.PdbData` and throws `ArgumentNullException` when it is missing, so
`DebugType` must not be `none`; the PDB is deleted again before the deploy step.

### Publishing to Paradox Mods

First publication (`ModId` absent or `0` in `Properties\PublishConfiguration.xml`):

```bash
dotnet publish -c Release /p:ModPublisherCommand=Publish
```

Set `DOTNET_ROLL_FORWARD=LatestMajor` first — `ModPublisher.exe` and `ModPostProcessor.exe`
target .NET 6 and need roll-forward to run on a newer runtime.

Subsequent releases use `Update` (metadata only) or `NewVersion` (new uploadable version), both
of which require `ModId` and `ChangeLog` to be set in `PublishConfiguration.xml`.

Authentication uses the Paradox account already signed in through the game launcher, via the
PDX SDK cache. Never put an account password in the project or on the command line.

### Project layout

```
RealWeatherSync.sln
RealWeatherSync/
  RealWeatherSync.csproj
  Mod.cs                                  IMod entry point, options-page callbacks
  Compatibility/WeatherModCompatibility.cs
  Diagnostics/StatusKind.cs
  Diagnostics/StatusReport.cs             thread-safe status shown in the options page
  Localization/LocaleEN.cs                en-US dictionary source
  Localization/LocaleKeys.cs
  Localization/Translation.cs             runtime lookup for dynamically built strings
  Mapping/WeatherMapper.cs                all mapping constants, no game dependencies
  Models/ClimateTarget.cs
  Models/LocationResult.cs
  Models/OpenMeteoGeocodingResponse.cs
  Models/OpenMeteoWeatherResponse.cs
  Models/WeatherSnapshot.cs               provider-independent observation
  Services/ILocationService.cs
  Services/IWeatherService.cs
  Services/OpenMeteoClient.cs             the only Open-Meteo-specific code
  Services/WeatherCoordinator.cs          async orchestration, single-flight, backoff
  Services/WeatherProviderException.cs
  Settings/RealWeatherSettings.cs
  Settings/UpdateIntervalOption.cs
  Systems/ClimateOverrideController.cs    the only writer of ClimateSystem
  Systems/RealWeatherSystem.cs            MainLoop system, transitions, lifecycle
  Properties/PublishConfiguration.xml
```

Swapping weather provider means writing one class implementing `ILocationService` and
`IWeatherService`; nothing else knows Open-Meteo exists.

### Logging

Logs go to
`%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Logs\RealWeatherSync.log`.

Logged: mod loaded / disposed, settings loaded, city resolution started / resolved / failed,
weather refresh started / received / failed, mapped target values, overrides activated /
deactivated, incompatible mod detected, interval changes. Nothing is logged per frame.

---

## Manual test checklist

The mod has been confirmed to load and run correctly in Cities: Skylines II (game build
`1.6.0f1`); `Modding.log` reports `Loaded RealWeatherSync, Version=1.0.0.0`. The checklist below
is kept as the regression pass for future releases - tick the items you have actually run rather
than assuming any of them still hold after a change.

- [ ] 1. Launch the game with Real Weather Sync **disabled**; confirm it appears in the mod list.
- [ ] 2. Load an existing city; confirm the weather behaves normally.
- [ ] 3. Enable the mod (Options -> Real Weather Sync -> Enable Real Weather).
- [ ] 4. Enter `Lyon, France` in the *City* field.
- [ ] 5. Press **Apply City**.
- [ ] 6. Confirm *Resolved location* shows `Lyon, ..., France` with plausible coordinates, and
      *Status* becomes *Connected*.
- [ ] 7. Press **Refresh Weather Now**; confirm *Last update* changes.
- [ ] 8. Compare the *Current weather* block against https://open-meteo.com for Lyon:
      temperature, cloud cover, rain/snow and fog should match the "Observed" line, and the
      "Applied" line should follow the mapping tables above.
- [ ] 9. Watch a refresh with *Smooth Weather Transitions* on; confirm the change is gradual, not
      instant.
- [ ] 10. Pause the game mid-transition; confirm the transition keeps progressing smoothly and
      nothing flickers.
- [ ] 11. Change the simulation speed (1x / 2x / 3x); confirm transition pacing is unaffected.
- [ ] 12. Disconnect the internet, wait for a refresh; confirm the status becomes *Offline - using
      last valid weather*, the weather does **not** jump to zero or an extreme, and no exception
      appears in the log. Reconnect and confirm recovery.
- [ ] 13. Enter an invalid city (`Zzzqqqxyz`) and press Apply City; confirm *Error resolving city*,
      the previous location is **kept**, and the game does not crash.
- [ ] 14. Change to another valid city (`Reykjavik, Iceland`); confirm it resolves and the weather
      transitions to the new location's conditions.
- [ ] 15. Press **Reset to Game Weather**; confirm the overrides are released and the game's own
      weather resumes.
- [ ] 16. Disable the mod; confirm all overrides are released immediately.
- [ ] 17. Return to the main menu and reload the city; confirm the stored city is still there and
      the weather re-applies without needing Apply City again.
- [ ] 18. Test with Time & Weather Anarchy **installed but disabled**; confirm Real Weather Sync
      works normally. *(Time & Weather Anarchy is already installed on this machine, subscribed
      as `88893`.)*
- [ ] 19. Enable Time & Weather Anarchy and reload; confirm Real Weather Sync reports
      *Incompatible weather mod active*, releases its overrides, and logs a warning. Then confirm
      `Ignore mod conflicts` overrides that behaviour.
- [ ] 20. Inspect `Logs\RealWeatherSync.log` and `Logs\Player.log` for exceptions.

Extra checks worth doing:

- [ ] 21. Save the city, exit, reopen: confirm the save is unaffected and no weather state was
      serialised into it.
- [ ] 22. Open the map editor; confirm no overrides are applied there.
- [ ] 23. Try a city that is currently snowing (in winter, e.g. `Tromso, Norway`) and confirm snow
      is drawn; check the temperature difference caused by *Show snow when it is really snowing*.

---

## Attribution and licensing

Real Weather Sync is released under the MIT License - see [LICENSE](LICENSE).

Weather and geocoding data come from **Open-Meteo** (https://open-meteo.com/), licensed
**CC BY 4.0**. See [NOTICE](NOTICE).

**Time & Weather Anarchy** by rodrigmatrix (Apache License 2.0) was read as a technical
reference while writing this mod, to understand the current Cities: Skylines II modding
toolchain and to confirm which `ClimateSystem` properties are safe to override.

**No source code from Time & Weather Anarchy was copied or adapted into this project**, and Real
Weather Sync has no build-time or runtime dependency on it. Every game API used here was verified
directly against the installed `Game.dll`. Because nothing was copied, Apache 2.0's redistribution
obligations are not triggered; the reference is acknowledged here as a courtesy. See
[NOTICE](NOTICE) for the full statement.

Newtonsoft.Json (MIT) is used for JSON parsing. It is **not** redistributed with this mod - the
assembly that ships with Cities: Skylines II is referenced at build time and resolved from the
game at runtime.
