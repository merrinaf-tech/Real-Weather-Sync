namespace RealWeatherSync.Localization.Strings
{
    /// <summary>de-DE. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsDe
    {
        public const string LocaleId = "de-DE";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Allgemein" },

            { "group.GeneralGroup", "Grundeinstellungen" },
            { "group.SearchGroup", "Stadt" },
            { "group.ActionsGroup", "Aktionen" },
            { "group.StatusGroup", "Status" },
            { "group.AdvancedGroup", "Erweitert" },
            { "group.SillyGroup", "Optionen, nach denen niemand gefragt hat" },
            { "group.AboutGroup", "Über" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Echtes Wetter aktivieren" },
            { "desc.EnableRealWeather",
                "Passt das sichtbare Wetter deiner Stadt an das aktuelle echte Wetter der unten " +
                "gewählten Stadt an. Uhrzeit, Datum und Jahreszeit werden nie verändert - das Spiel " +
                "liest die Wetterwerte jedoch zurück, siehe \"Was das Spiel zurückliest\" unter " +
                "Erweitert. Beim Ausschalten übernimmt sofort wieder das Spiel." },

            { "label.FollowGameClock", "Der Spielzeit folgen" },
            { "desc.FollowGameClock",
                "Statt eines einzelnen festen Messwerts durchläuft das Wetter die letzten 24 Stunden " +
                "der gewählten Stadt, gesteuert von der Spieluhr. Ist es in deiner Stadt 15:00 Uhr, " +
                "bekommst du das echte Wetter, das die gewählte Stadt zuletzt um 15:00 Uhr hatte - " +
                "das Wetter ändert sich also im Lauf des Tages. Die Spieluhr wird nur gelesen, nie " +
                "verändert: Zeit, Datum und Jahreszeit bleiben genau so, wie das Spiel sie gesetzt " +
                "hat. Überschreibt die manuelle Zeitverschiebung, solange die Option aktiv ist." },

            { "label.SmoothTransitions", "Weiche Wetterübergänge" },
            { "desc.SmoothTransitions",
                "Blendet allmählich zum neuen Wetter über, statt sofort umzuschalten. Die Überblendung " +
                "läuft in Echtzeit und ist daher unabhängig von Pause und Simulationsgeschwindigkeit." },

            { "label.TransitionSeconds", "Übergangsdauer (Sekunden)" },
            { "desc.TransitionSeconds",
                "Wie lange eine Überblendung zwischen zwei Wettermessungen dauert, in echten Sekunden. " +
                "Wird nur verwendet, wenn weiche Übergänge aktiviert sind." },

            { "label.UpdateInterval", "Aktualisierungsintervall" },
            { "desc.UpdateInterval",
                "Wie oft Open-Meteo nach aktuellen Bedingungen gefragt wird. Open-Meteo aktualisiert " +
                "seine Daten etwa alle 15 Minuten, kürzere Intervalle bringen daher wenig." },

            { "enum.UpdateInterval.FifteenMinutes", "15 Minuten" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 Minuten" },
            { "enum.UpdateInterval.SixtyMinutes", "60 Minuten" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "Stadt" },
            { "desc.CityQuery",
                "Die echte Stadt, von der das Wetter übernommen wird. Beispiele: Lyon - Lyon, " +
                "Frankreich - Milazzo, Italien - New York, Vereinigte Staaten. Ergänze nach einem " +
                "Komma ein Land oder eine Region, um die Suche einzugrenzen." },

            { "label.SearchCity", "Suchen" },
            { "desc.SearchCity",
                "Schlägt den Namen nach und listet unten jede passende Stadt auf, damit du die " +
                "richtige bestätigen kannst, statt einer einzelnen Vermutung zu vertrauen." },

            { "label.SelectedSearchResult", "Suchergebnisse" },
            { "desc.SelectedSearchResult",
                "Städte, die zu deiner Suche passen, beste Übereinstimmung zuerst, mit Region, Land " +
                "und Koordinaten. Eine Auswahl wird sofort übernommen." },

            { "label.SelectedFavourite", "Zuletzt verwendete Städte" },
            { "desc.SelectedFavourite",
                "Städte, die du bereits verwendet hast. Eine Auswahl wechselt sofort dorthin, ganz " +
                "ohne erneute Suche." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Stadt übernehmen" },
            { "desc.ApplyCity",
                "Verwendet die beste Übereinstimmung für den oben eingegebenen Namen, ohne aus der " +
                "Liste zu wählen. Wird die Stadt nicht gefunden, bleibt der zuletzt ermittelte Ort " +
                "erhalten." },

            { "label.RefreshWeatherNow", "Wetter jetzt aktualisieren" },
            { "desc.RefreshWeatherNow",
                "Ruft die aktuellen Bedingungen sofort ab, statt auf die nächste Aktualisierung zu " +
                "warten." },

            { "label.ApplyImmediately", "Sofort anwenden" },
            { "desc.ApplyImmediately",
                "Aktualisiert und springt ohne Überblendung direkt zum neuen Wetter. Bricht außerdem " +
                "einen bereits laufenden Übergang ab." },

            { "label.ResetToGameWeather", "Auf Spielwetter zurücksetzen" },
            { "desc.ResetToGameWeather",
                "Gibt alle Klima-Überschreibungen frei, sodass wieder das Spiel das Wetter steuert. " +
                "Das echte Wetter kehrt zurück, sobald du eine Stadt übernimmst oder manuell " +
                "aktualisierst." },
            { "warn.ResetToGameWeather",
                "Alle Wetter-Überschreibungen freigeben und die Kontrolle an das Spiel zurückgeben?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "Status" },
            { "label.ResolvedLocationText", "Ermittelter Ort" },
            { "label.LastUpdateText", "Letzte Aktualisierung" },
            { "label.CurrentWeatherText", "Aktuelles Wetter" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Nebel synchronisieren" },
            { "desc.SyncFog",
                "Leitet Nebel aus den gemeldeten Nebelcodes und aus der Sichtweite ab. Bewölkung " +
                "erzeugt nie Nebel. Ausschalten, um den Nebel des Spiels unangetastet zu lassen." },

            { "label.SyncTemperature", "Temperatur synchronisieren" },
            { "desc.SyncTemperature",
                "Steuert die sichtbare Temperatur anhand der echten Stadt. Die Temperatur ist der " +
                "Wert, den die meisten Spielsysteme zurücklesen - Heizbedarf, Unterhaltskosten, " +
                "Brandrisiko, Tourismus, Schnee auf dem Boden. Diese Option auszuschalten ist damit " +
                "der wirksamste Weg, den Einfluss der Mod so gering wie möglich zu halten. Der Preis: " +
                "Das Spiel kann Regen nicht mehr von Schnee unterscheiden, und die angezeigte " +
                "Temperatur ist die des Spiels." },

            { "label.SimulationImpactNote", "Was das Spiel zurückliest" },

            { "label.ForceSnowAppearance", "Schnee zeigen, wenn es wirklich schneit" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II entscheidet anhand der sichtbaren Temperatur zwischen Regen und " +
                "Schnee. Wenn das echte Wetter Schnee meldet, die echte Temperatur aber über dem " +
                "Gefrierpunkt liegt, wird die sichtbare Temperatur knapp unter null gesetzt, damit " +
                "Schnee dargestellt wird. Die angezeigte Temperatur weicht dann von der echten ab. " +
                "Jahreszeit und Datum bleiben weiterhin unangetastet." },

            { "label.IgnoreModConflicts", "Mod-Konflikte ignorieren" },
            { "desc.IgnoreModConflicts",
                "Standardmäßig schaltet sich Real Weather Sync ab, sobald eine andere bekannte " +
                "Wetter-Mod geladen ist, denn zwei Mods, die dieselben Klimawerte schreiben, geraten " +
                "sich in die Quere. Aktiviere dies nur, wenn du sicher bist, dass die andere Mod das " +
                "Wetter nicht überschreibt." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Zeitverschiebung (Stunden)" },
            { "desc.TimeShiftHours",
                "Liest das Wetter einer anderen Stunde: negativ für die Vergangenheit, positiv für " +
                "die Vorhersage. Bei -24 lebt deine Stadt im Wetter von gestern, bei +24 bekommt sie " +
                "das von morgen einen Tag früher. Nur der Wettermesswert verschiebt sich - Spieluhr, " +
                "Datum und Jahreszeit bleiben unberührt." },

            { "label.AntipodeMode", "Antipoden-Modus" },
            { "desc.AntipodeMode",
                "Nimmt das Wetter von genau der gegenüberliegenden Seite des Planeten. Für den " +
                "größten Teil Europas ist das mitten im Südpazifik, erwarte also viel grauen " +
                "Nieselregen über dem Ozean. Deine Stadt bleibt die gewählte - nur das Wetter kommt " +
                "von der anderen Seite." },

            { "label.ExtremeLocation", "Bring mich an einen schrecklichen Ort" },
            { "desc.ExtremeLocation",
                "Springt direkt an einen berüchtigt ungemütlichen Ort. Zugleich der schnellste Weg, " +
                "Schnee, Nebel oder Wolkenbruch zu sehen, ohne auf das Wetter zu Hause zu warten. " +
                "Springt beim Neustart auf Keine zurück, die gewählte Stadt bleibt aber wie jede " +
                "andere erhalten." },

            { "label.OppositeDay", "Umgekehrter Tag" },
            { "desc.OppositeDay",
                "Spiegelt das Wetter. Warm wird kalt, klar wird bedeckt, trocken wird klatschnass. " +
                "Nebel bleibt unangetastet, denn Dauernebel verdeckt die Stadt und ist sofort nicht " +
                "mehr lustig." },

            { "enum.ExtremeLocation.None", "Bleib, wo du bist" },
            { "enum.ExtremeLocation.Yakutsk", "Jakutsk - die kälteste Stadt der Welt" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - Polarnacht, Polartag, Eisbären" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaia - das Ende der Welt, und man sieht es" },
            { "enum.ExtremeLocation.Reykjavik", "Reykjavík - Wind und Regen, waagerecht" },
            { "enum.ExtremeLocation.MountWashington", "Mount Washington - Nebel und das schlechteste Wetter Amerikas" },
            { "enum.ExtremeLocation.DeathValley", "Death Valley - der heißeste je gemessene Ort" },
            { "enum.ExtremeLocation.Cherrapunji", "Cherrapunji - einer der nassesten Orte der Erde" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "Über" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Wetterdaten von Open-Meteo (open-meteo.com), lizenziert unter CC BY 4.0. Kein Konto und kein API-Schlüssel nötig.\n" +
                "An Open-Meteo werden ausschließlich der eingegebene Stadtname und die daraus ermittelten Koordinaten gesendet." },

            { "key.About.SimulationImpact",
                "Real Weather Sync schreibt dieselben vier Klimawerte, die auch die spieleigenen " +
                "Entwickler-Wetterwerkzeuge schreiben, und Teile des Spiels lesen diese Werte zurück. " +
                "Heiz- und Kühlbedarf, Gebäudeunterhalt, Brandrisiko, Freizeit, Tourismus, Schnee auf " +
                "dem Boden, Nässe der Oberflächen und Wetterereignisse reagieren alle auf Temperatur " +
                "und Niederschlag - genau so, wie sie auf das spieleigene Wetter reagieren.\n" +
                "Die Mod fügt keine Systeme hinzu, ändert keine Regeln und schreibt nichts in deinen " +
                "Spielstand.\n" +
                "\"Temperatur synchronisieren\" auszuschalten entfernt den größten Teil davon, auf " +
                "Kosten der Genauigkeit bei Regen und Schnee. Solarertrag und Grundwasser sind nie " +
                "betroffen, und Nebel wirkt sich außerhalb der Darstellung auf nichts aus." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Deaktiviert" },
            { "key.Status.CityNotConfigured", "Keine Stadt eingestellt" },
            { "key.Status.ResolvingLocation", "Ort wird ermittelt" },
            { "key.Status.Refreshing", "Wetter wird aktualisiert" },
            { "key.Status.CandidatesReady", "Wähle eine Stadt aus den Suchergebnissen" },
            { "key.Status.Connected", "Verbunden" },
            { "key.Status.Offline", "Offline - letztes gültiges Wetter wird verwendet" },
            { "key.Status.ErrorResolvingCity", "Fehler beim Ermitteln der Stadt" },
            { "key.Status.IncompatibleMod", "Inkompatible Wetter-Mod aktiv" },
            { "key.Status.Released", "Überschreibungen freigegeben - Spielwetter aktiv" },
            { "key.Status.WaitingForGame", "Warte auf das Laden einer Stadt" },

            { "key.Location.NotResolved", "Noch kein Ort ermittelt" },

            { "key.LastUpdate.Never", "Nie" },
            { "key.LastUpdate.JustNow", "gerade eben" },
            { "key.LastUpdate.MinutesAgo", "vor {0} Min." },

            { "key.Weather.NoData", "Noch keine Wetterdaten empfangen." },
            { "key.Weather.Observed", "Gemessen" },
            { "key.Weather.Applied", "Angewendet" },
            { "key.Weather.Clouds", "Bewölkung" },
            { "key.Weather.Precipitation", "Niederschlag" },
            { "key.Weather.Snow", "Schnee" },
            { "key.Weather.Code", "WMO-Code" },
            { "key.Weather.Visibility", "Sichtweite" },
            { "key.Weather.Fog", "Nebel" },
            { "key.Weather.Conditions", "Bedingungen" },
            { "key.Weather.TimeShiftPast", "{0} h in der Vergangenheit" },
            { "key.Weather.TimeShiftFuture", "{0} h voraus - Vorhersage" },
            { "key.Weather.OppositeDay", "Umgekehrter Tag" },
            { "key.Weather.Antipode", "Antipode" },

            { "key.Search.NoResults", "Keine Ergebnisse - drücke Suchen" },
            { "key.Search.PickOne", "Stadt auswählen ..." },
            { "key.Favourites.Empty", "Noch keine zuletzt verwendeten Städte" },

            { "key.Overrides.Active", "Klima-Überschreibungen sind aktiv." },
            { "key.Overrides.Inactive", "Klima-Überschreibungen sind nicht aktiv." },

            { "key.Error.CityNotFound", "Keine passende Stadt gefunden" },
            { "key.Error.EmptyCity", "Gib zuerst einen Stadtnamen ein" },
            { "key.Error.Network", "Open-Meteo nicht erreichbar" },
            { "key.Error.RateLimited", "Open-Meteo drosselt die Anfragen" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Klarer Himmel" },
            { "wmo.MainlyClear", "Überwiegend klar" },
            { "wmo.PartlyCloudy", "Teilweise bewölkt" },
            { "wmo.Overcast", "Bedeckt" },
            { "wmo.Fog", "Nebel" },
            { "wmo.RimeFog", "Reifnebel" },
            { "wmo.DrizzleLight", "Leichter Nieselregen" },
            { "wmo.DrizzleModerate", "Mäßiger Nieselregen" },
            { "wmo.DrizzleDense", "Dichter Nieselregen" },
            { "wmo.FreezingDrizzleLight", "Leichter gefrierender Nieselregen" },
            { "wmo.FreezingDrizzleDense", "Dichter gefrierender Nieselregen" },
            { "wmo.RainSlight", "Leichter Regen" },
            { "wmo.RainModerate", "Mäßiger Regen" },
            { "wmo.RainHeavy", "Starker Regen" },
            { "wmo.FreezingRainLight", "Leichter gefrierender Regen" },
            { "wmo.FreezingRainHeavy", "Starker gefrierender Regen" },
            { "wmo.SnowSlight", "Leichter Schneefall" },
            { "wmo.SnowModerate", "Mäßiger Schneefall" },
            { "wmo.SnowHeavy", "Starker Schneefall" },
            { "wmo.SnowGrains", "Schneegriesel" },
            { "wmo.ShowersSlight", "Leichte Regenschauer" },
            { "wmo.ShowersModerate", "Mäßige Regenschauer" },
            { "wmo.ShowersViolent", "Heftige Regenschauer" },
            { "wmo.SnowShowersSlight", "Leichte Schneeschauer" },
            { "wmo.SnowShowersHeavy", "Starke Schneeschauer" },
            { "wmo.Thunderstorm", "Gewitter" },
            { "wmo.ThunderstormHailSlight", "Gewitter mit leichtem Hagel" },
            { "wmo.ThunderstormHailHeavy", "Gewitter mit starkem Hagel" },
            { "wmo.Unknown", "Unbekannte Bedingungen" }
        };
    }
}
