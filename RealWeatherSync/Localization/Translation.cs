using System;
using Game.SceneFlow;

namespace RealWeatherSync.Localization
{
    /// <summary>
    /// Looks a locale key up in the active dictionary at runtime.
    ///
    /// The options page renders read-only string properties as literal values
    /// (Game.UI.Localization.LocalizedString.Value), not as locale ids, so dynamic
    /// status text has to be translated by us before it is handed over. Every call
    /// site supplies the en-US string as the fallback, which is also what
    /// <see cref="LocaleEN"/> registers.
    /// </summary>
    internal static class Translation
    {
        public static string Get(string key, string fallback)
        {
            if (string.IsNullOrEmpty(key))
            {
                return fallback;
            }

            try
            {
                var manager = GameManager.instance != null ? GameManager.instance.localizationManager : null;
                var dictionary = manager != null ? manager.activeDictionary : null;
                if (dictionary != null)
                {
                    string value;
                    if (dictionary.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }
            catch (Exception)
            {
                // Localisation is best effort; never let it break the options page.
            }

            return fallback;
        }
    }
}
