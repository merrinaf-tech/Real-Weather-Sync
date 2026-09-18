using System;
using System.Collections.Generic;
using Colossal.Atmosphere;
using Colossal.Serialization.Entities;
using Game;
using Game.Rendering;
using Game.SceneFlow;
using Game.Settings;
using Game.Simulation;
using RealWeatherSync.Compatibility;
using Unity.Mathematics;

namespace RealWeatherSync.Systems
{
    /// <summary>
    /// Replaces only the rendered sun transform with the real position above the selected
    /// weather location. It runs after <see cref="PlanetarySystem"/> has produced the normal
    /// frame and before <see cref="LightingSystem"/> consumes the lights.
    ///
    /// No PlanetarySystem property is assigned: the game clock, calendar, latitude, longitude
    /// and serialised state remain the game's own. When the option stops applying,
    /// PlanetarySystem naturally restores its sun on the next frame.
    /// </summary>
    public partial class RealSunSystem : GameSystemBase
    {
        private const double RecalculateSeconds = 1.0;

        private PlanetarySystem _planetarySystem;
        private SunMoonData _sunMoonData;
        private bool _isGame;
        private bool _conflictChecked;
        private bool _conflictDetected;
        private bool _faultLogged;

        private bool _hasCalculation;
        private DateTime _calculatedAtUtc;
        private double _calculatedLatitude;
        private double _calculatedLongitude;
        private double2 _calculatedSunLimit;
        private float3 _sunPosition;
        private quaternion _sunRotation;
        private float _sunIntensityFactor;

        protected override void OnCreate()
        {
            base.OnCreate();
            _planetarySystem = World.GetOrCreateSystemManaged<PlanetarySystem>();
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            base.OnGamePreload(purpose, mode);
            _isGame = false;
            _conflictChecked = false;
            _hasCalculation = false;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            _isGame = mode.IsGame();
            _conflictChecked = false;
            _hasCalculation = false;
        }

        protected override void OnUpdate()
        {
            var settings = Mod.Settings;
            var coordinator = Mod.Coordinator;
            if (!_isGame || _planetarySystem == null || settings == null || coordinator == null ||
                !settings.EnableRealWeather || !settings.SyncSunPosition ||
                Mod.OverridesSuspended || !coordinator.HasLocation)
            {
                _hasCalculation = false;
                return;
            }

            if (!settings.IgnoreModConflicts && HasKnownConflict())
            {
                _hasCalculation = false;
                return;
            }

            var gameplay = SharedSettings.instance != null ? SharedSettings.instance.gameplay : null;
            if (gameplay != null && !gameplay.dayNightVisual)
            {
                _hasCalculation = false;
                return;
            }

            var location = coordinator.Location;
            if (location == null)
            {
                _hasCalculation = false;
                return;
            }

            var point = settings.AntipodeMode ? location.CreateAntipode() : location;

            try
            {
                var sun = _planetarySystem.SunLight;
                if (!sun.isValid || sun.transform == null || sun.additionalData == null)
                {
                    _hasCalculation = false;
                    return;
                }

                var now = DateTime.UtcNow;
                var sunLimit = _planetarySystem.sunLimit;
                if (NeedsCalculation(now, point.Latitude, point.Longitude, sunLimit))
                {
                    Calculate(now, point.Latitude, point.Longitude, sunLimit);
                }

                sun.transform.position = _sunPosition;
                sun.transform.rotation = _sunRotation;
                sun.additionalData.intensity = sun.initialIntensity * _sunIntensityFactor;
                _faultLogged = false;
            }
            catch (Exception e)
            {
                _hasCalculation = false;
                if (!_faultLogged)
                {
                    Mod.Log.Error(e, "Real sun positioning failed; the game will keep control of the sun.");
                    _faultLogged = true;
                }
            }
        }

        private bool HasKnownConflict()
        {
            if (!_conflictChecked)
            {
                _conflictChecked = true;
                List<string> conflicts;
                _conflictDetected = WeatherModCompatibility.TryDetectConflicts(out conflicts);
            }

            return _conflictDetected;
        }

        private bool NeedsCalculation(DateTime now, double latitude, double longitude, double2 sunLimit)
        {
            return !_hasCalculation ||
                   Math.Abs((now - _calculatedAtUtc).TotalSeconds) >= RecalculateSeconds ||
                   Math.Abs(latitude - _calculatedLatitude) > 0.000001 ||
                   Math.Abs(longitude - _calculatedLongitude) > 0.000001 ||
                   !sunLimit.Equals(_calculatedSunLimit);
        }

        private void Calculate(DateTime now, double latitude, double longitude, double2 sunLimit)
        {
            JulianDateTime julian = now;
            var coordinates = _sunMoonData.GetLimitedSunPosition(julian, latitude, longitude, sunLimit);

            float distance;
            var position = coordinates.ToLocalCoordinates(out distance);
            var matrix = float4x4.LookAt(position, float3.zero, new float3(0f, 1f, 0f));
            var lightDirection = math.rotate(matrix, new float3(0f, 0f, 1f));

            _sunPosition = position;
            _sunRotation = new quaternion(matrix);
            _sunIntensityFactor = math.smoothstep(0f, 0.3f, math.abs(math.min(0f, lightDirection.y)));
            _calculatedAtUtc = now;
            _calculatedLatitude = latitude;
            _calculatedLongitude = longitude;
            _calculatedSunLimit = sunLimit;
            _hasCalculation = true;
        }
    }
}
