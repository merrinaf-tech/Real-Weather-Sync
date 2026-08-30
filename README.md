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

## What it does not write

It does not write, and contains no code that could write:

- thunder, lightning, rainbows, auroras, hail or wind;
- the game clock, the date, the season or the day/night cycle (`PlanetarySystem.time` is *read*
  by the follow-the-clock mode, never assigned);
- the in-game planet's latitude / longitude;
- savegame data - nothing this mod produces is serialised into a city save.

The only game state it writes is the `overrideValue` / `overrideState` pair of the four
properties listed above, which is exactly what the game's own developer weather tools write.

## What the game reads back

**Versions up to 1.2.0 claimed this mod did not affect the simulation. That was wrong.** The
claim has been retracted; this section replaces it.

`OverridableProperty.op_Implicit` returns the override whenever one is active, so every consumer
reading a climate property through the implicit conversion sees *this mod's* value. Ten
simulation systems do:

| System | Reads | Effect |
|---|---|---|
| `AdjustElectricityConsumptionSystem` | temperature | heating / cooling demand |
| `BuildingUpkeepSystem` | temperature | building upkeep cost |
| `FireHazardSystem` | temperature, `isRaining` | fire risk |
| `FireSimulationSystem` | temperature | fire behaviour |
| `LeisureSystem` | temperature | leisure demand |
| `SnowSystem` | temperature, precipitation | snow on the ground |
| `TourismSystem` | temperature, precipitation, `isRaining`, `isSnowing` | tourism |
| `TouristSpawnSystem` | same | tourist arrivals |
| `WeatherHazardSystem` | temperature, precipitation, cloudiness | creates weather events |
| `WetnessSystem` | temperature, precipitation | surface wetness |

`isRaining`, `isSnowing` and `isPrecipitating` are *computed from* the overridden temperature and
precipitation, so systems reading only those flags are affected too.

`WeatherHazardSystem.WeatherHazardJob.CreateWeatherEvent` is gated by
`CityConfigurationSystem.naturalDisasters` — a city with natural disasters switched off is not
exposed to that one.

**Verified not affected**, because they read the base value: `PowerPlantAISystem` (solar output),
`SoilWaterSystem` (groundwater), and `LeisureSystem`'s precipitation input. **Fog is read by
nothing outside rendering**, which makes it the safest value to drive.

The mod adds no systems and changes no rules — it feeds the ones that already exist, exactly as
the game's own weather does. Switching off **Synchronise temperature** removes 8 of the 10
couplings; the precipitation ones remain, because there is no way to show weather without
something reading it.

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
| **Follow the in-game clock** | Off by default. See [below](#following-the-in-game-clock). |
| **Smooth Weather Transitions** | Fade to each new reading instead of snapping. |
| **Transition length** | 0–600 seconds of real time. Default 120. Disabled when smoothing is off. |
| **Update Interval** | 15 / 30 / 60 minutes. Default 15. |

### City

| Setting | Meaning |
|---|---|
| **City** | The real city to copy. `Lyon`, `Lyon, France`, `Milazzo, Italy`, `New York, United States`. |
| **Search** | Looks the name up and lists every match below, so you can confirm the right one. |
| **Search results** | Every matching city with its region, country and coordinates, best match first. Picking one applies it immediately. |
| **Recent cities** | The last 10 cities you used. Picking one switches instantly, with no lookup. |

### Actions

| Button | Meaning |
|---|---|
| **Apply City** | Use the best match for the typed name without picking from the list. |
| **Refresh Weather Now** | Fetch current conditions without waiting for the next interval. Disabled until a city is resolved. |
| **Apply Immediately** | Refresh and jump straight to the new weather, skipping the fade. Also cuts short a transition already running. |
| **Reset to Game Weather** | Release all overrides and let the game drive the weather again. Real weather resumes on the next apply or refresh. |

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
| **Synchronise temperature** | on | Drive the visual temperature from the real city. Temperature is what most game systems read back — see [What the game reads back](#what-the-game-reads-back) — so turning it off is the most effective way to keep the mod's influence minimal. Cost: the game can no longer tell rain from snow. |
| **Synchronise fog** | on | Derive fog from fog weather codes and visibility. Turn off to leave the game's fog alone. |
| **Show snow when it is really snowing** | on | See [Snow](#snow-and-its-one-unavoidable-compromise) below. |
| **Ignore mod conflicts** | off | Skip the other-weather-mod check. See [Compatibility](#compatibility). |

### Options nobody asked for

Still purely cosmetic — these bend the *reading*, never the simulation.

| Setting | Default | Meaning |
|---|---|---|
| **Time shift** | 0 h | Read the weather from up to 24 hours in the past or the future. At −24 your city lives yesterday's weather; at +24 it gets tomorrow's forecast a day early. **The game clock, date and season are not affected** — only which weather reading is used. |
| **Antipode mode** | off | Take the weather from the point diametrically opposite your city (`lat' = −lat`, `lon' = lon ± 180`). For most of Europe that is the middle of the South Pacific. Your city stays the one you chose; only the request coordinates are mirrored. |
| **Take me somewhere awful** | None | One-click jumps to Yakutsk, Longyearbyen, Ushuaia, Reykjavik, Mount Washington, Death Valley or Cherrapunji. Coordinates are built in, so no lookup is needed. Also the fastest way to see snow, fog or a downpour without waiting for the weather at home. Resets to None on restart; the city it applied is kept. |
| **Opposite day** | off | Mirror the weather: temperature reflected around 15 °C, cloudiness and precipitation inverted. Fog is deliberately left alone, because permanent fog hides the city and stops being funny within seconds. |

### Following the in-game clock

Normally the mod shows the city's weather **right now**: one reading, refreshed every 15–60
minutes.

With **Follow the in-game clock** enabled it instead walks the city's **last 24 hours** of real
weather, using your in-game hour to choose which one:

> It is 10:00 in the real world and 15:00 in your city → you get the weather the chosen city
> actually had at its most recent 15:00, which was yesterday.
> Your clock rolls on to 16:00 → you get that city's 16:00 weather. And so on.

So a full in-game day replays a full real day of that city's weather, in order, instead of
holding one frozen snapshot. Values are interpolated continuously between the two bracketing
hours, so there are no steps and no fade is needed — the transition settings are ignored (and
greyed out) while this mode is on, as is the manual time shift.

**The in-game clock is only read, never written.** The time, date, season and day/night cycle
stay exactly as the game set them — the mod just asks what time it is to decide which weather to
show. This costs no extra network traffic: the hourly series already comes back with the request
the mod was making anyway.

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
run-tests.ps1                             one command to run the automated suite
AGENTS.md                                 working rules for AI assistants
RealWeatherSync/
  RealWeatherSync.csproj
  Mod.cs                                  IMod entry point, options-page callbacks
  Compatibility/WeatherModCompatibility.cs
  Diagnostics/StatusKind.cs
  Diagnostics/StatusReport.cs             thread-safe status shown in the options page
  Diagnostics/TemperatureDisplay.cs       formats to the game's own temperature unit
  Localization/LocaleEN.cs                en-US dictionary source
  Localization/LocaleKeys.cs
  Localization/Translation.cs             runtime lookup for dynamically built strings
  Mapping/WeatherMapper.cs                all mapping constants, no game dependencies
  Mapping/WeatherCodes.cs                 WMO code to condition name
  Models/ClimateTarget.cs
  Models/LocationResult.cs                includes the antipode transform
  Models/OpenMeteoGeocodingResponse.cs
  Models/OpenMeteoWeatherResponse.cs
  Models/WeatherSnapshot.cs               provider-independent observation
  Models/WeatherTimeline.cs               24h series + in-game-hour resolution
  Services/ILocationService.cs
  Services/IWeatherService.cs
  Services/OpenMeteoClient.cs             the only Open-Meteo-specific code
  Services/WeatherCoordinator.cs          async orchestration, single-flight, backoff
  Services/WeatherProviderException.cs
  Settings/RealWeatherSettings.cs
  Settings/UpdateIntervalOption.cs
  Settings/FavouriteCities.cs             recent-cities encoding
  Settings/ExtremeLocations.cs            built-in preset coordinates
  Systems/ClimateOverrideController.cs    the only writer of ClimateSystem
  Systems/RealWeatherSystem.cs            MainLoop system, transitions, lifecycle
  Properties/PublishConfiguration.xml
tests/RealWeatherSync.Tests/              links the game-free sources, runs on net8.0
```

Swapping weather provider means writing one class implementing `ILocationService` and
`IWeatherService`; nothing else knows Open-Meteo exists.

### Automated tests

```bash
.\run-tests.ps1
```

172 assertions covering the weather mapping and its curve breakpoints, the in-game-clock
timeline, the antipode transform, the extreme-location table, and the Open-Meteo client against
the **real** API. Exit code is non-zero on failure.

```bash
.\run-tests.ps1 -Offline
```

Skips the 47 live-API assertions when there is no connection.

The test project links the mod's own source files rather than referencing the built assembly,
because the mod targets `net48` against the game while the tests run on `net8.0`. **Only files
free of game types can be linked** — that constraint is precisely why `WeatherMapper`,
`WeatherTimeline`, `LocationResult` and the models are kept game-free. If adding a file to the
test project breaks the build with a missing `Game.*` reference, the fix is to move the game
dependency out of that file, not to reference the game from the tests.

The suite is deliberately **not** part of `RealWeatherSync.sln`, so the release path stays
exactly as it is and tests never run as a side effect of shipping.

These tests say nothing about in-game behaviour — that is what the manual checklist below is for.

### Logging

Logs go to
`%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Logs\RealWeatherSync.log`.

Logged: mod loaded / disposed, settings loaded, city resolution started / resolved / failed,
weather refresh started / received / failed, mapped target values, overrides activated /
deactivated, incompatible mod detected, interval changes. Nothing is logged per frame.

---

## Manual test checklist

`run-tests.ps1` covers everything that does not need the game. This checklist covers everything
that does. **Run it before every release** — tick what you actually ran rather than assuming an
item still holds after a change.

Keep it in step with the features: a new option that is not on this list is a feature nobody will
ever regression-test.

### A. Basics

- [ ] A1. Launch with the mod **disabled**; confirm it appears in the mod list and the weather is
      the game's own.
- [ ] A2. Enable it, load a city, and confirm `Logs\RealWeatherSync.log` shows the expected
      version.
- [ ] A3. Disable it mid-game; confirm all overrides are released immediately.
- [ ] A4. Press **Reset to Game Weather**; confirm the game's weather resumes, then confirm real
      weather comes back after an apply or refresh.
- [ ] A5. Return to the main menu and reload; confirm the stored city is still there and the
      weather re-applies without touching anything.
- [ ] A6. Open the map editor; confirm **no** overrides are applied there.
- [ ] A7. Save, exit, reopen: confirm the save is unaffected and no weather state was written
      into it.
- [ ] A8. Inspect `RealWeatherSync.log` and `Player.log` for exceptions.

### B. Choosing a city

- [ ] B1. Type `Lyon`, press **Search**; confirm the results dropdown fills **without reopening
      the options page**, and shows region, country and coordinates.
- [ ] B2. Pick a result; confirm it applies immediately and *Resolved location* updates.
- [ ] B3. Type `Springfield`, search; confirm several distinct candidates appear.
- [ ] B4. Type `Springfield, United States`; confirm only US results are listed.
- [ ] B5. Search `Zzzqqqxyz`; confirm *Error resolving city*, the previous location is **kept**,
      and nothing crashes.
- [ ] B6. Use **Apply City** without searching; confirm it takes the best match directly.
- [ ] B7. Switch cities a few times, then open **Recent cities**; confirm they are listed newest
      first and picking one switches instantly with no lookup.
- [ ] B8. Restart the game; confirm the recent list survived.

### C. Weather accuracy

- [ ] C1. Compare the *Current weather* block with https://open-meteo.com for the same city:
      the "Observed" line should match, and "Applied" should follow the mapping tables above.
- [ ] C2. Confirm the conditions are **named** ("Light drizzle", "Overcast"), not just a number.
- [ ] C3. Change the game's temperature unit (Options -> Interface); confirm the status panel
      follows it — Celsius, Fahrenheit and Kelvin.

### D. Transitions

- [ ] D1. With smoothing on, watch a refresh; confirm the change is gradual.
- [ ] D2. Pause mid-transition; confirm it keeps progressing smoothly and nothing flickers.
- [ ] D3. Change simulation speed (1x / 2x / 3x); confirm pacing is unaffected.
- [ ] D4. Move the **Transition length** slider to 0 and to 600; confirm both behave.
- [ ] D5. Turn smoothing off; confirm the slider greys out and changes are instant.
- [ ] D6. Press **Apply Immediately** mid-transition; confirm it snaps to the new weather at once.

### E. Follow the in-game clock

- [ ] E1. Turn it on; confirm the transition slider and **Time shift** both grey out.
- [ ] E2. Let the in-game clock run; confirm the weather **changes with the hour** and moves
      continuously, with no visible steps at each hour boundary.
- [ ] E3. Confirm the in-game **time, date and season do not move** because of the mod.
- [ ] E4. Note the in-game hour and compare the applied weather with that city's real weather at
      the most recent occurrence of that hour.
- [ ] E5. Turn it off again; confirm it returns to the current reading without a jarring jump.

### F. Options nobody asked for

- [ ] F1. **Time shift** to −24; confirm the weather changes and the status block labels it as a
      past reading. Then +24 and confirm it says forecast.
- [ ] F2. **Antipode mode** on a European city; confirm the status labels it and the weather looks
      like open ocean rather than the city itself.
- [ ] F3. **Take me somewhere awful** → Ushuaia or Yakutsk; confirm it applies with no lookup
      delay, and that snow is drawn if it is snowing there.
- [ ] F4. Restart; confirm the preset dropdown is back to **None** but the city it applied is kept.
- [ ] F5. **Opposite day**; confirm warm/cold and clear/overcast invert and that fog is **not**
      inverted.

### F2. Simulation impact (1.3.0)

- [ ] F2a. Turn **Synchronise temperature** off; confirm the in-game temperature returns to the
      game's own while clouds, rain and fog still follow the real city.
- [ ] F2b. Confirm **Show snow when it is really snowing** greys out while it is off.
- [ ] F2c. Turn it back on; confirm the temperature override returns without a restart.
- [ ] F2d. Confirm the **What the game reads back** note is visible in the Advanced options.

### G. Resilience

- [ ] G1. Disconnect the internet and wait for a refresh; confirm *Offline - using last valid
      weather*, that the weather does **not** jump to zero or an extreme, and that no exception
      is logged. Reconnect and confirm recovery.
- [ ] G2. Change city while a request is in flight; confirm no crash and the newest choice wins.

### H. Compatibility

- [ ] H1. With Time & Weather Anarchy **installed but disabled**, confirm Real Weather Sync works
      normally. *(It is installed on this machine, subscribed as `88893`.)*
- [ ] H2. Enable Time & Weather Anarchy and reload; confirm the status reports *Incompatible
      weather mod active*, overrides are released, and a warning is logged.
- [ ] H3. Enable `Ignore mod conflicts`; confirm that overrides the check.

### Open, unverified

- [ ] X1. **Snow accumulation.** Jump to a snowing preset, let snow settle, then jump somewhere
      warm. Does the ground snow melt, or does it persist? Currently **assumed fine but never
      verified** — Time & Weather Anarchy ships a "Remove Snow" button, which suggests it may be
      a real problem.

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
