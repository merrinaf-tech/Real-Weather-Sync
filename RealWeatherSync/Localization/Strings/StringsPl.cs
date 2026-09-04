namespace RealWeatherSync.Localization.Strings
{
    /// <summary>pl-PL. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsPl
    {
        public const string LocaleId = "pl-PL";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Główne" },

            { "group.GeneralGroup", "Ogólne" },
            { "group.SearchGroup", "Miasto" },
            { "group.ActionsGroup", "Akcje" },
            { "group.StatusGroup", "Stan" },
            { "group.AdvancedGroup", "Zaawansowane" },
            { "group.SillyGroup", "Opcje, o które nikt nie prosił" },
            { "group.AboutGroup", "O modzie" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Włącz prawdziwą pogodę" },
            { "desc.EnableRealWeather",
                "Dopasowuje widoczną pogodę w Twoim mieście do aktualnej prawdziwej pogody miasta " +
                "wybranego poniżej. Zegar, data i pora roku nigdy nie są zmieniane, ale gra " +
                "rzeczywiście odczytuje wartości pogodowe z powrotem - zobacz „Co gra odczytuje z " +
                "powrotem” w sekcji Zaawansowane. Wyłączenie natychmiast oddaje pogodę grze." },

            { "label.FollowGameClock", "Podążaj za zegarem gry" },
            { "desc.FollowGameClock",
                "Zamiast jednego zamrożonego odczytu przechodzi przez ostatnie 24 godziny " +
                "prawdziwej pogody wybranego miasta, kierując się godziną w grze. Jeśli w Twoim " +
                "mieście jest 15:00, dostajesz prawdziwą pogodę, jaką wybrane miasto miało o swojej " +
                "ostatniej 15:00 - pogoda zmienia się więc wraz z upływem dnia. Zegar gry jest " +
                "tylko odczytywany, nigdy zmieniany: czas, data i pora roku pozostają dokładnie " +
                "takie, jakie ustawiła gra. Gdy opcja jest włączona, ma pierwszeństwo przed ręcznym " +
                "przesunięciem czasu." },

            { "label.SmoothTransitions", "Płynne przejścia pogodowe" },
            { "desc.SmoothTransitions",
                "Stopniowo przechodzi do każdego nowego odczytu zamiast przełączać się " +
                "natychmiast. Przejście liczone jest w czasie rzeczywistym, więc pauza i szybkość " +
                "symulacji nie mają na nie wpływu." },

            { "label.TransitionSeconds", "Długość przejścia (sekundy)" },
            { "desc.TransitionSeconds",
                "Ile trwa przejście między dwoma odczytami pogody, w rzeczywistych sekundach. " +
                "Używane tylko przy włączonych płynnych przejściach." },

            { "label.UpdateInterval", "Częstotliwość aktualizacji" },
            { "desc.UpdateInterval",
                "Jak często pytać Open-Meteo o świeże warunki. Open-Meteo odświeża dane mniej " +
                "więcej co 15 minut, więc krótsze odstępy niewiele dają." },

            { "enum.UpdateInterval.FifteenMinutes", "15 minut" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 minut" },
            { "enum.UpdateInterval.SixtyMinutes", "60 minut" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "Miasto" },
            { "desc.CityQuery",
                "Prawdziwe miasto, z którego kopiowana jest pogoda. Przykłady: Lyon - Lyon, " +
                "Francja - Milazzo, Włochy - Nowy Jork, Stany Zjednoczone. Dodaj po przecinku kraj " +
                "lub region, aby zawęzić wyszukiwanie." },

            { "label.SearchCity", "Szukaj" },
            { "desc.SearchCity",
                "Wyszukuje nazwę i wypisuje poniżej wszystkie pasujące miasta, żebyś mógł " +
                "potwierdzić to właściwe zamiast ufać jednemu strzałowi." },

            { "label.SelectedSearchResult", "Wyniki wyszukiwania" },
            { "desc.SelectedSearchResult",
                "Miasta pasujące do zapytania, najlepsze dopasowanie na początku, wraz z regionem, " +
                "krajem i współrzędnymi. Wybór stosowany jest od razu." },

            { "label.SelectedFavourite", "Ostatnie miasta" },
            { "desc.SelectedFavourite",
                "Miasta, których już używałeś. Wybór przełącza na nie natychmiast, bez ponownego " +
                "wyszukiwania." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Zastosuj miasto" },
            { "desc.ApplyCity",
                "Używa najlepszego dopasowania dla nazwy wpisanej powyżej, bez wybierania z listy. " +
                "Jeśli miasta nie da się znaleźć, zachowana zostaje poprzednio ustalona lokalizacja." },

            { "label.RefreshWeatherNow", "Odśwież pogodę teraz" },
            { "desc.RefreshWeatherNow",
                "Natychmiast pobiera aktualne warunki zamiast czekać na następną aktualizację." },

            { "label.ApplyImmediately", "Zastosuj natychmiast" },
            { "desc.ApplyImmediately",
                "Odświeża i przeskakuje prosto do nowej pogody, pomijając przejście. Przerywa też " +
                "przejście, które już trwa." },

            { "label.ResetToGameWeather", "Przywróć pogodę gry" },
            { "desc.ResetToGameWeather",
                "Zwalnia wszystkie nadpisania klimatu i oddaje sterowanie pogodą grze. Prawdziwa " +
                "pogoda wraca, gdy zastosujesz miasto lub wymusisz odświeżenie." },
            { "warn.ResetToGameWeather",
                "Zwolnić wszystkie nadpisania pogody i oddać sterowanie grze?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "Stan" },
            { "label.ResolvedLocationText", "Ustalona lokalizacja" },
            { "label.LastUpdateText", "Ostatnia aktualizacja" },
            { "label.CurrentWeatherText", "Aktualna pogoda" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Synchronizuj mgłę" },
            { "desc.SyncFog",
                "Wyprowadza mgłę z raportowanych kodów mgły oraz z widzialności. Zachmurzenie nigdy " +
                "nie tworzy mgły. Wyłącz, aby zostawić w spokoju mgłę samej gry." },

            { "label.SyncTemperature", "Synchronizuj temperaturę" },
            { "desc.SyncTemperature",
                "Ustala widoczną temperaturę na podstawie prawdziwego miasta. Temperatura to " +
                "wartość odczytywana przez największą liczbę systemów gry - zapotrzebowanie na " +
                "ogrzewanie, utrzymanie, ryzyko pożaru, turystyka, śnieg na ziemi - dlatego " +
                "wyłączenie tej opcji jest najskuteczniejszym sposobem ograniczenia wpływu moda do " +
                "minimum. Ceną jest to, że gra nie odróżnia już deszczu od śniegu, a wyświetlana " +
                "temperatura jest temperaturą samej gry." },

            { "label.SimulationImpactNote", "Co gra odczytuje z powrotem" },

            { "label.ForceSnowAppearance", "Pokazuj śnieg, gdy naprawdę pada śnieg" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II rozstrzyga między deszczem a śniegiem na podstawie widocznej " +
                "temperatury. Gdy prawdziwa pogoda to śnieg, ale prawdziwa temperatura jest powyżej " +
                "zera, widoczna temperatura zostaje obniżona tuż poniżej zera, żeby śnieg został " +
                "narysowany. Wyświetlana temperatura będzie się wtedy różnić od prawdziwej. Pora " +
                "roku i data nadal nigdy nie są zmieniane." },

            { "label.IgnoreModConflicts", "Ignoruj konflikty modów" },
            { "desc.IgnoreModConflicts",
                "Domyślnie Real Weather Sync wyłącza się, gdy wczytany jest inny znany mod " +
                "pogodowy, ponieważ dwa mody zapisujące te same wartości klimatu będą ze sobą " +
                "walczyć. Włącz to tylko wtedy, gdy masz pewność, że drugi mod nie nadpisuje pogody." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Przesunięcie czasu (godziny)" },
            { "desc.TimeShiftHours",
                "Odczytuje pogodę z innej godziny: wartości ujemne to przeszłość, dodatnie to " +
                "prognoza. Przy -24 Twoje miasto żyje wczorajszą pogodą, przy +24 dostaje jutrzejszą " +
                "dzień wcześniej. Przesuwa się wyłącznie odczyt pogody - zegar gry, data i pora roku " +
                "pozostają nietknięte." },

            { "label.AntipodeMode", "Tryb antypodów" },
            { "desc.AntipodeMode",
                "Bierze pogodę z punktu dokładnie po przeciwnej stronie planety niż wybrane miasto. " +
                "Dla większości Europy jest to środek południowego Pacyfiku, więc spodziewaj się " +
                "mnóstwa szarej mżawki nad oceanem. Twoje miasto pozostaje tym, które wybrałeś - z " +
                "drugiej strony globu przychodzi tylko pogoda." },

            { "label.ExtremeLocation", "Zabierz mnie w jakieś okropne miejsce" },
            { "desc.ExtremeLocation",
                "Przeskakuje prosto do miejsca słynącego z fatalnej pogody. To także najszybszy " +
                "sposób, by zobaczyć śnieg, mgłę albo ulewę bez czekania, aż pogoda w domu " +
                "współpracuje. Po restarcie wraca do Brak, ale wybrane miasto zostaje zachowane jak " +
                "każde inne." },

            { "label.OppositeDay", "Dzień na opak" },
            { "desc.OppositeDay",
                "Odwraca pogodę. Ciepło staje się zimnem, bezchmurnie staje się pochmurnie, sucho " +
                "staje się przemoczone. Mgła zostaje nietknięta, bo stała mgła zasłania miasto i " +
                "natychmiast przestaje być zabawna." },

            { "enum.ExtremeLocation.None", "Zostań tam, gdzie jesteś" },
            { "enum.ExtremeLocation.Yakutsk", "Jakuck - najzimniejsze miasto na Ziemi" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - noc polarna, dzień polarny, niedźwiedzie polarne" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaia - koniec świata, i widać to" },
            { "enum.ExtremeLocation.Reykjavik", "Reykjavík - wiatr i deszcz, poziomo" },
            { "enum.ExtremeLocation.MountWashington", "Mount Washington - mgła i najgorsza pogoda w Ameryce" },
            { "enum.ExtremeLocation.DeathValley", "Dolina Śmierci - najgorętsze miejsce w historii pomiarów" },
            { "enum.ExtremeLocation.Cherrapunji", "Czerapundżi - jedno z najbardziej mokrych miejsc na Ziemi" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "O modzie" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Dane pogodowe od Open-Meteo (open-meteo.com), na licencji CC BY 4.0. Nie trzeba konta ani klucza API.\n" +
                "Do Open-Meteo wysyłane są wyłącznie wpisana przez Ciebie nazwa miasta i ustalone na jej podstawie współrzędne." },

            { "key.About.SimulationImpact",
                "Real Weather Sync zapisuje te same cztery wartości klimatu, które zapisują " +
                "deweloperskie narzędzia pogodowe samej gry, a część gry odczytuje te wartości z " +
                "powrotem. Zapotrzebowanie na ogrzewanie i chłodzenie, utrzymanie budynków, ryzyko " +
                "pożaru, rozrywka, turystyka, śnieg na ziemi, wilgoć powierzchni oraz zdarzenia " +
                "pogodowe - wszystko to reaguje na temperaturę i opady dokładnie tak samo, jak " +
                "reaguje na własną pogodę gry.\n" +
                "Mod nie dodaje żadnych systemów, nie zmienia żadnych zasad i nie zapisuje niczego " +
                "w Twoim stanie gry.\n" +
                "Wyłączenie opcji „Synchronizuj temperaturę” usuwa największą część tego wpływu, " +
                "kosztem dokładności rozróżniania deszczu i śniegu. Produkcja energii słonecznej i " +
                "wody gruntowe nigdy nie są dotknięte, a mgła nie wpływa na nic poza obrazem." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Wyłączone" },
            { "key.Status.CityNotConfigured", "Nie ustawiono miasta" },
            { "key.Status.ResolvingLocation", "Ustalanie lokalizacji" },
            { "key.Status.Refreshing", "Odświeżanie pogody" },
            { "key.Status.CandidatesReady", "Wybierz miasto z wyników wyszukiwania" },
            { "key.Status.Connected", "Połączono" },
            { "key.Status.Offline", "Offline - używana ostatnia poprawna pogoda" },
            { "key.Status.ErrorResolvingCity", "Błąd podczas ustalania miasta" },
            { "key.Status.IncompatibleMod", "Aktywny niezgodny mod pogodowy" },
            { "key.Status.Released", "Nadpisania zwolnione - używana pogoda gry" },
            { "key.Status.WaitingForGame", "Oczekiwanie na wczytanie miasta" },

            { "key.Location.NotResolved", "Nie ustalono jeszcze żadnej lokalizacji" },

            { "key.LastUpdate.Never", "Nigdy" },
            { "key.LastUpdate.JustNow", "przed chwilą" },
            { "key.LastUpdate.MinutesAgo", "{0} min temu" },

            { "key.Weather.NoData", "Nie otrzymano jeszcze danych pogodowych." },
            { "key.Weather.Observed", "Zaobserwowano" },
            { "key.Weather.Applied", "Zastosowano" },
            { "key.Weather.Clouds", "zachmurzenie" },
            { "key.Weather.Precipitation", "opady" },
            { "key.Weather.Snow", "śnieg" },
            { "key.Weather.Code", "kod WMO" },
            { "key.Weather.Visibility", "widzialność" },
            { "key.Weather.Fog", "mgła" },
            { "key.Weather.Conditions", "Warunki" },
            { "key.Weather.TimeShiftPast", "{0} godz. w przeszłości" },
            { "key.Weather.TimeShiftFuture", "{0} godz. do przodu - prognoza" },
            { "key.Weather.OppositeDay", "Dzień na opak" },
            { "key.Weather.Antipode", "Antypody" },

            { "key.Search.NoResults", "Brak wyników - naciśnij Szukaj" },
            { "key.Search.PickOne", "Wybierz miasto..." },
            { "key.Favourites.Empty", "Brak ostatnich miast" },

            { "key.Overrides.Active", "Nadpisania klimatu są aktywne." },
            { "key.Overrides.Inactive", "Nadpisania klimatu nie są aktywne." },

            { "key.Error.CityNotFound", "Nie znaleziono pasującego miasta" },
            { "key.Error.EmptyCity", "Najpierw wpisz nazwę miasta" },
            { "key.Error.Network", "Nie udało się połączyć z Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo ogranicza liczbę zapytań" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Bezchmurnie" },
            { "wmo.MainlyClear", "Przeważnie bezchmurnie" },
            { "wmo.PartlyCloudy", "Częściowe zachmurzenie" },
            { "wmo.Overcast", "Pochmurno" },
            { "wmo.Fog", "Mgła" },
            { "wmo.RimeFog", "Mgła osadzająca szadź" },
            { "wmo.DrizzleLight", "Słaba mżawka" },
            { "wmo.DrizzleModerate", "Umiarkowana mżawka" },
            { "wmo.DrizzleDense", "Gęsta mżawka" },
            { "wmo.FreezingDrizzleLight", "Słaba marznąca mżawka" },
            { "wmo.FreezingDrizzleDense", "Gęsta marznąca mżawka" },
            { "wmo.RainSlight", "Słaby deszcz" },
            { "wmo.RainModerate", "Umiarkowany deszcz" },
            { "wmo.RainHeavy", "Silny deszcz" },
            { "wmo.FreezingRainLight", "Słaby marznący deszcz" },
            { "wmo.FreezingRainHeavy", "Silny marznący deszcz" },
            { "wmo.SnowSlight", "Słaby śnieg" },
            { "wmo.SnowModerate", "Umiarkowany śnieg" },
            { "wmo.SnowHeavy", "Silny śnieg" },
            { "wmo.SnowGrains", "Śnieg ziarnisty" },
            { "wmo.ShowersSlight", "Słabe przelotne opady deszczu" },
            { "wmo.ShowersModerate", "Umiarkowane przelotne opady deszczu" },
            { "wmo.ShowersViolent", "Gwałtowne przelotne opady deszczu" },
            { "wmo.SnowShowersSlight", "Słabe przelotne opady śniegu" },
            { "wmo.SnowShowersHeavy", "Silne przelotne opady śniegu" },
            { "wmo.Thunderstorm", "Burza" },
            { "wmo.ThunderstormHailSlight", "Burza ze słabym gradem" },
            { "wmo.ThunderstormHailHeavy", "Burza z silnym gradem" },
            { "wmo.Unknown", "Nieznane warunki" }
        };
    }
}
