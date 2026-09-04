# Real Weather Sync — working rules for AI assistants

Read this before touching the code. It exists so you do not have to re-derive the project's
constraints, the verified game API facts, or the release pipeline by reading everything.

Everything below was verified against the installed game assemblies and the official toolchain,
not assumed. Where something is unverified it says so.

---

## 1. What this mod is, and its one inviolable rule

Real Weather Sync makes the *visual* weather of a Cities: Skylines II city match the current
real-world weather of a city the player names, using Open-Meteo.

**It writes only visual climate values. This is the whole product promise — do not erode it.**

> **Correction, 1.3.0 — read this before repeating the old claim.** Through 1.2.0 the README,
> the store listing and this file all said the mod "does not touch the simulation". **That was
> wrong**, and it was published to a thousand users. `OverridableProperty.op_Implicit` returns
> `m_OverrideValue` whenever the override is active, so *every* consumer that reads the property
> through the implicit conversion sees the mod's value, not the game's. Ten simulation systems do
> exactly that — see section 3a. The mod adds no systems and changes no rules, but it does feed
> the ones that already exist. Never restore the old wording.

It may write **only** these, via `Game.Simulation.ClimateSystem`:

| Property | Unit / range |
|---|---|
| `temperature` | degrees Celsius, clamp to `-50 .. 50` |
| `cloudiness` | `0 .. 1` |
| `precipitation` | `0 .. 1` |
| `fog` | `0 .. 1` |

(each via its `overrideValue` + `overrideState` pair.) It also *reads* `freezingTemperature`.

**Never WRITE, and never add code that could write:** the simulation (citizens, traffic, energy,
economy, services, growth), the game clock, date, season, day/night cycle, `PlanetarySystem`
(time, latitude, longitude), `currentDate`, `aurora`, `thunder`, `rainbow`, `hail`, `wind`,
disasters, or anything serialised into a savegame.

**Reading is allowed where it serves a cosmetic decision.** Since v1.1 the mod reads
`PlanetarySystem.time` (the in-game hour) to pick which hour of real weather to show in
"follow the in-game clock" mode. Reading the clock changes nothing; setting it would break the
core promise. The distinction is deliberate — do not "simplify" it back into a blanket ban, and
do not let it drift into a write.

There is a guard for this: a Cecil scan of the built DLL. Run it after any change that touches
`Systems/`, and check specifically that no `PlanetarySystem::set_*` appears. As of v1.1 the only
PlanetarySystem member referenced is `get_time()`.

---

## 2. Architecture map

```
Mod.cs                                IMod entry point; owns settings, coordinator, options-page callbacks
Settings/RealWeatherSettings.cs       the options page (ModSetting)
Settings/UpdateIntervalOption.cs
Localization/LocaleSource.cs          the ONLY game-aware localisation code: slot -> locale id
Localization/LocaleKeys.cs            ids for strings built at runtime
Localization/Translation.cs           runtime lookup for dynamic strings
Localization/Strings/LocaleTable.cs   one language's slot -> text map. No game dependencies.
Localization/Strings/LocaleTables.cs  the registry of all twelve languages
Localization/Strings/Strings*.cs      the twelve translation tables. No game dependencies.
Mapping/WeatherMapper.cs              ALL mapping constants. No game dependencies. Unit-testable.
Models/                               ClimateTarget, LocationResult, WeatherSnapshot, Open-Meteo DTOs
Models/WeatherTimeline.cs             the 24h hourly series + in-game-hour -> real-hour resolution
Services/ILocationService.cs          geocoding interface
Services/IWeatherService.cs           forecast interface
Services/OpenMeteoClient.cs           the ONLY Open-Meteo-specific code
Services/WeatherCoordinator.cs        async orchestration: single-flight, cancellation, backoff
Systems/ClimateOverrideController.cs  the ONLY writer of ClimateSystem
Systems/RealWeatherSystem.cs          MainLoop system: transitions, lifecycle, conflict guard
Compatibility/WeatherModCompatibility.cs
Diagnostics/StatusReport.cs           thread-safe status shown in the options page
```

**Invariants to preserve:**

- Mapping constants live in `WeatherMapper` only. Never scatter magic numbers into systems.
- `ClimateOverrideController` is the only file that writes `ClimateSystem`.
- `WeatherMapper`, `Models/` and `Localization/Strings/` must stay free of game types so they can
  be compiled standalone for tests. Putting a game type in a translation table would take all
  twelve languages out of the offline suite's reach.
- A new player-facing string goes into `StringsEn` first, then into the other eleven tables. The
  offline suite fails until every table carries the same slot set.
- The twelve locale ids are the complete set the game supports (verified in its `Locale.cok`);
  a thirteenth could never be selected by a player.
- Swapping weather provider must mean writing one class implementing both service interfaces.

**Threading contract:** HTTP and JSON happen on thread-pool threads; the ECS system *polls*
results (`TryTakeSnapshot` / `TryTakeLocation`). No game API is ever touched off the main thread.
No `.Result`, no `.Wait()`, no per-frame requests, no unbounded task creation.

**Transitions** use `System.Diagnostics.Stopwatch` (real wall-clock), never simulation time, so
pause and speed changes do not affect them. A new reading mid-transition starts from the current
interpolated values, never from the old start values.

**"Follow the in-game clock" mode** (opt-in, off by default) bypasses the fade machinery entirely:
the in-game hour selects an hour out of the last 24 hours of real weather and the value is
interpolated continuously between the two bracketing hours, so there is nothing to fade. The
hourly series costs no extra traffic — the existing request already returns
`past_days=1 & forecast_days=2`. The manual time shift is disabled while this mode is on, because
both decide "which hour" and stacking them would only confuse.

---

## 3a. What the game reads back from the overridden values

Verified by scanning every consumer in `Game.dll` and classifying each read as `op_Implicit`
(sees the mod's value) or `.value` (sees the game's own). Re-run that scan if the game updates.

`isRaining`, `isSnowing` and `isPrecipitating` are **computed from** the overridden temperature
and precipitation, so any system reading those flags is affected too.

**Simulation systems that see the mod's values**

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
| `WeatherHazardSystem` | temperature, precipitation, cloudiness | **creates weather events** |
| `WetnessSystem` | temperature, precipitation | surface wetness |

`WeatherHazardSystem.WeatherHazardJob.CreateWeatherEvent` is gated by `m_NaturalDisasters`
(from `CityConfigurationSystem.naturalDisasters`), so players with natural disasters switched
off are not exposed to that one.

**Verified NOT affected** — these read the base value:
`PowerPlantAISystem` (solar output, cloudiness), `SoilWaterSystem` (groundwater, precipitation),
and `LeisureSystem`'s precipitation input.

**Fog is read by nothing outside rendering**, which is why it is the safest value to drive.

Outside the simulation the values also reach rendering, the in-game weather panel, rain audio,
`Creatures.InitializeSystem` (citizen initialisation) and Discord rich presence.

Since 1.3.0 the player can switch off `SyncTemperature`, which drops the temperature override
entirely and removes 8 of the 10 couplings above. Precipitation coupling remains — there is no
way to show weather without something reading it.

## 3. Verified game API facts

Saves you re-decompiling `Game.dll`.

**Options UI** — widgets are chosen by reflection in `Game.UI.Menu.AutomaticSettings.GetWidgetType`:

| Property shape | Widget |
|---|---|
| `bool` + `[SettingsUIButton]` | button (set-only is fine); add `[SettingsUIConfirmation]` for a confirm dialog |
| `bool` read+write | toggle |
| `string` read+write + `[SettingsUITextInput]` | text input |
| `string` read+write + `[SettingsUIDropdown(type, method)]` | dropdown |
| `string` **get-only** | read-only field; add `[SettingsUIMultilineText]` for a text block |
| `int`/`float` + `[SettingsUISlider]` | slider (`min`, `max`, `step`, `unit`, `scalarMultiplier`) |
| `enum` | dropdown |

- Get-only string properties **auto-refresh** while the page is open: `ReadonlyField.Update()`
  calls `GetValue()` every UI tick when no `valueVersion` is set. That is how the live status
  panel works with no extra plumbing.
- Read-only string values are rendered with `LocalizedString.Value(...)`, i.e. **verbatim**, not
  as a locale id. Dynamic status text must therefore be translated by our own code — that is what
  `Translation.Get(key, fallback)` is for.
- `DropdownField<T>` exposes `set_itemsVersion(Func<int>)`, fed by `[SettingsUIValueVersion]`.
  **This means a dropdown's item list can be refreshed at runtime** (needed for the city picker).
- `DropdownItem<T>` has `value`, `displayName` (LocalizedString), `disabled`.
- Condition methods for `[SettingsUIDisableByCondition]` / `[SettingsUIHideByCondition]`:
  zero parameters, `bool` return, public or private, static or instance on the settings type.
- Mark computed/display properties with `Colossal.Json.Exclude` so they are not serialised.

**There is no filtering combobox / type-ahead widget.** True autocomplete would require a custom
React/TypeScript UI module with its own npm+webpack pipeline. Do not promise it inside the
options page.

**Other:**
- `OverridableProperty<T>` has `value`, `overrideValue`, `overrideState`, plus an implicit
  conversion to `T` that yields the *effective* value.
- `GameSystemBase` overrides: `OnCreate`, `OnDestroy`, `OnGamePreload(Purpose, GameMode)`,
  `OnGameLoaded(Context)`, `OnGameLoadingComplete(Purpose, GameMode)`, `OnUpdate`.
- Gate gameplay work on `mode.IsGame()` — never apply overrides in the editor or main menu.
- Mod enumeration for conflict detection: `foreach (var m in GameManager.instance.modManager)`
  gives `ModManager.ModInfo` with `name`, `assemblyFullName`, `isLoaded`.

---

## 4. Weather mapping rules

Full tables are in `README.md` and the constants are in `WeatherMapper.cs`. Key rules:

- Open-Meteo reports precipitation/rain/showers in **mm**, but **snowfall in cm**. Water
  equivalent ≈ `snow_cm * 0.7`.
- Rain and snow have separate piecewise curves; the stronger result wins.
- The WMO `weather_code` supplies a *floor*, so a reported shower is never drawn dry when the
  hourly accumulation bucket is still empty. Measured amounts win when larger.
- Fog comes from fog codes (45, 48) and visibility only. **Cloud cover must never produce fog.**
  Low visibility during precipitation is capped, because heavy rain is not fog.
- `visibility` is an **hourly** Open-Meteo variable, not a current one; we request
  `past_hours=1&forecast_hours=1` and match the sample to `current.time`.

**The rain-vs-snow compromise:** the engine picks rain or snow from the *visual temperature*
against `freezingTemperature`; mods cannot set it directly. When it is really snowing above
freezing, `ForceSnowAppearance` (default on) pulls the visual temperature 1.5 °C below freezing.
The displayed temperature then differs from reality. This is documented in the README and the
listing — **keep it documented if you change it**.

---

## 5. Build and release

**Environment:** no .NET SDK on PATH — prepend `C:\Users\merri\.dotnet`. The official CS2
toolchain is installed; `CSII_*` variables live in the **user** registry hive and may be missing
from an already-open shell, so mirror them into the process or rely on the csproj reading the
user target directly.

```bash
dotnet build RealWeatherSync.sln -c Release
```

```bash
dotnet publish -c Release /p:ModPublisherCommand=NewVersion
```

`DOTNET_ROLL_FORWARD=LatestMajor` is required for the publisher/post-processor (they target .NET 6).

**Hard-won build facts — do not "clean these up":**

1. **`DebugType` must not be `none` in Release.** Unity's `EntitiesILPostProcessors` reads
   `InMemoryAssembly.PdbData` and throws `ArgumentNullException` without a PDB, failing the whole
   ModPostProcessor step. The csproj emits a PDB and deletes it again before deploy.
2. `PathMap` keeps local absolute paths out of the assembly. Keep it.
3. `Mod.targets`' `DeployWIP` uploads **everything** in `$(OutDir)`. `VerifyPackageContents`
   fails the build on anything unexpected. The legitimate package is exactly 4 files: the DLL
   plus three Burst AOT stubs (`_win_x86_64.dll`, `_linux_x86_64.so`, `_mac_x86_64.bundle`).
4. Those Burst stubs embed the build machine's temp path, including the Windows user name. This
   is true of every published CS2 mod; accepted deliberately.

**Publishing — `PublishConfiguration.xml` is destructive.** `PrepareMetadata` calls
`WithForumLinks`, `WithExternalLinks`, `WithDependencies` and `WithTags` **unconditionally**, so
anything not declared in the file is sent as an empty set and **wiped from the live listing**,
including values added through the website. This already destroyed the forum link once.
Screenshots are the only exception (`WithScreenshots` is skipped on an empty set), so
website-uploaded screenshots survive.

**The public listing lags behind an update.** `LAST UPDATED` changes immediately but link cards
can take much longer to appear. Do not conclude a field was lost by re-checking minutes later.

**Commands:** `Publish` only works with `ModId` absent or 0 — it is now set, so first-publish is
correctly blocked. Use `Update` for metadata only, `NewVersion` to ship a new build.
`NewVersion` requires a non-empty `<ChangeLog>` and does a full rebuild.

**Identity:** ModId `154183` · listing `https://mods.paradoxplaza.com/mods/154183/Windows` ·
source `https://github.com/merrinaf-tech/Real-Weather-Sync` (branch `main`) ·
forum thread `.../mod-real-weather-sync.1937641/` · author display name **Fabiozsche** ·
commit identity `Fabiozsche <merrinaf-tech@users.noreply.github.com>` (the noreply address must
match the GitHub account or commits are not attributed).

Authentication for publishing uses the launcher's Paradox session. **Never put a password or
token in the project, on a command line, or in chat.**

---

## 6. Testing — and the one thing an AI cannot do

```bash
.\run-tests.ps1            # 172 assertions, includes live Open-Meteo calls
.\run-tests.ps1 -Offline   # 125 assertions, no network
```

- The suite lives in `tests/RealWeatherSync.Tests` and is **not** part of `RealWeatherSync.sln`,
  so shipping never depends on it and the release path stays untouched.
- It **links the mod's source files** rather than referencing the built assembly (net48 vs net8).
  Only game-free files can be linked — that is exactly why `WeatherMapper`, `WeatherTimeline`,
  `LocationResult` and the models must stay free of game types. If adding a file breaks the test
  build with a missing `Game.*` reference, move the game dependency out of that file; do not
  reference the game from the tests.
- Coverage: mapping curves and clamps, opposite day, WMO names, the in-game-clock timeline
  (including the exact-hour bracket regression), the antipode transform, the extreme-location
  table, and the Open-Meteo client against the **real** API.
- After any change under `Systems/`, also re-run the Cecil scan of the built DLL to prove the
  forbidden-API list is still clean.
- Keep `README.md`'s manual checklist in step with new features: an option that is not on that
  list will never be regression-tested.

**An AI assistant cannot verify in-game behaviour.** Not "has not" — cannot. The game is not
launchable from here and the result is *visual*. So:

> Always distinguish "compiles / static checks pass / standalone tests pass" from "works in
> game". Never claim the second. The human runs the game; their confirmation is the gate before
> anything is published.

A Release build deploys into `%LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Mods\RealWeatherSync\`,
which **shadows the subscribed PDX copy**. That is how the human tests a change before release —
but it must be removed again to test the subscribed package. CS2 keeps only **one log per mod**
and overwrites it each launch, so `Logs/RealWeatherSync.log` is never evidence of an earlier
session.

---

## 7. Design decisions, and ideas deliberately rejected

Do not re-propose these without new information.

| Rejected | Why |
|---|---|
| Detecting the player's location by IP | Destroys the mod's clean privacy story: today we can truthfully say only the typed city name and its coordinates ever leave the machine. |
| Anything touching time, date, season or day/night | The core promise. Not negotiable. |
| Chirper messages about the source city | Breaks immersion: the player's city is *their* city, it merely shares Lyon's weather — it is not Lyon. The game already produces citizen chirps about weather, so this would add noise, not meaning. |
| True type-ahead autocomplete in the options page | No such widget exists; would need a separate React UI module. Use search-then-pick instead. |
| Harmony patching | Public game APIs have been sufficient. Adding Harmony raises the conflict surface for a cosmetic mod. |

**Privacy rule:** exactly two things may leave the machine — the city name the player types, and
the coordinates returned for it. Adding any other outbound call requires updating the README, the
listing text and this file.

---

## 8. Roadmap

**v1.1 — delivered.** City search with a results dropdown, recent-cities quick switch,
"Apply Immediately", transition-length slider, WMO condition names, temperature-unit awareness,
plus the first two "Options nobody asked for" (time shift −24…+24 h, opposite day).

**v1.2 — delivered.** Antipode mode (`LocationResult.CreateAntipode`, applied only to the request
coordinates in `WeatherCoordinator.RefreshAsync`) and the "Take me somewhere awful" presets
(`Settings/ExtremeLocations.cs`, hardcoded coordinates so a preset costs no geocoding).

**Still on the shelf** (same group; must stay clearly labelled so it never looks like core
behaviour)
- Random city roulette — would reuse the `ExtremeLocations` table pattern, so it is now cheap.
- Drama mode (intensity multiplier) and a "+N °C" slider, both single-line changes in the mapper.

**v1.4 — delivered.** All twelve game locales translated (`Localization/Strings/`), with the
tables kept game-free so the offline suite can check them against `en-US`. Also swept up three
places where the claim retracted in 1.3.0 was still standing: the in-game *Enable Real Weather*
description, the listing's short description, and two lines in the README.

**Known gaps worth doing before more features**
- The twelve translations have **never been seen rendered in game** — checklist items F3a–F3d.
  The offline suite proves the tables agree with each other, not that the game font draws the
  glyphs or that the panel fits the longer German and Russian strings.
- Snow accumulation: **parked by the author's decision, assumed fine for now — NOT verified.**
  If the mod makes it snow, ground snow may persist after the weather clears; Time & Weather
  Anarchy ships a "Remove Snow" button, which suggests the problem is real. The extreme-location
  presets (Ushuaia, Yakutsk) make this testable on demand. Treat it as an open assumption, never
  report it as working.
- The listing has only one screenshot, which is the biggest marketing gap for a visual mod.

Anything new must remain cosmetic and must not weaken section 1.
