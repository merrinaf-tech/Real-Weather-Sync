using System.Collections.Generic;

namespace RealWeatherSync.Localization.Strings
{
    /// <summary>
    /// Every language the mod ships, in one place.
    ///
    /// The list is exactly the twelve locales Cities: Skylines II itself supports - the game
    /// offers no other locale id for the player to select, so a thirteenth table could never
    /// be reached. Adding a language means adding its table here and nowhere else.
    ///
    /// Game-free on purpose: the offline test suite walks this list to check every table
    /// against <see cref="StringsEn"/>.
    /// </summary>
    public static class LocaleTables
    {
        /// <summary>The reference language. Every other table must carry the same slots.</summary>
        public static LocaleTable Reference()
        {
            return new LocaleTable(StringsEn.LocaleId, StringsEn.Table);
        }

        /// <summary>Every table, English first.</summary>
        public static IEnumerable<LocaleTable> All()
        {
            yield return Reference();
            yield return new LocaleTable(StringsDe.LocaleId, StringsDe.Table);
            yield return new LocaleTable(StringsEs.LocaleId, StringsEs.Table);
            yield return new LocaleTable(StringsFr.LocaleId, StringsFr.Table);
            yield return new LocaleTable(StringsIt.LocaleId, StringsIt.Table);
            yield return new LocaleTable(StringsJa.LocaleId, StringsJa.Table);
            yield return new LocaleTable(StringsKo.LocaleId, StringsKo.Table);
            yield return new LocaleTable(StringsPl.LocaleId, StringsPl.Table);
            yield return new LocaleTable(StringsPtBr.LocaleId, StringsPtBr.Table);
            yield return new LocaleTable(StringsRu.LocaleId, StringsRu.Table);
            yield return new LocaleTable(StringsZhHans.LocaleId, StringsZhHans.Table);
            yield return new LocaleTable(StringsZhHant.LocaleId, StringsZhHant.Table);
        }
    }
}
