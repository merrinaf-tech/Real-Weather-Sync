namespace RealWeatherSync.Localization.Strings
{
    /// <summary>it-IT. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsIt
    {
        public const string LocaleId = "it-IT";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Principale" },

            { "group.GeneralGroup", "Generale" },
            { "group.SearchGroup", "Città" },
            { "group.ActionsGroup", "Azioni" },
            { "group.StatusGroup", "Stato" },
            { "group.AdvancedGroup", "Avanzate" },
            { "group.SillyGroup", "Opzioni che nessuno ha chiesto" },
            { "group.AboutGroup", "Informazioni" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Attiva il meteo reale" },
            { "desc.EnableRealWeather",
                "Allinea il meteo visibile della tua città al meteo reale attuale della città scelta " +
                "qui sotto. Orologio, data e stagione non vengono mai modificati, ma il gioco legge " +
                "davvero i valori meteo: vedi \"Cosa rilegge il gioco\" nella sezione Avanzate. " +
                "Disattivando l'opzione il meteo torna subito sotto il controllo del gioco." },

            { "label.FollowGameClock", "Segui l'orologio di gioco" },
            { "desc.FollowGameClock",
                "Invece di un singolo valore fisso, percorre le ultime 24 ore di meteo reale della " +
                "città scelta seguendo l'ora di gioco. Se nella tua città sono le 15:00, ottieni il " +
                "meteo reale che la città scelta aveva alle sue ultime 15:00: il meteo cambia quindi " +
                "con il passare della giornata. L'orologio di gioco viene solo letto, mai modificato: " +
                "ora, data e stagione restano esattamente come le ha impostate il gioco. Finché è " +
                "attiva, ha la precedenza sullo sfasamento orario manuale." },

            { "label.SmoothTransitions", "Transizioni meteo graduali" },
            { "desc.SmoothTransitions",
                "Sfuma gradualmente verso ogni nuovo valore invece di cambiare di colpo. " +
                "La dissolvenza usa il tempo reale, quindi non risente della pausa né della velocità " +
                "di simulazione." },

            { "label.TransitionSeconds", "Durata della transizione (secondi)" },
            { "desc.TransitionSeconds",
                "Quanto dura una dissolvenza tra due letture meteo, in secondi reali. " +
                "Usata solo con le transizioni graduali attive." },

            { "label.UpdateInterval", "Intervallo di aggiornamento" },
            { "desc.UpdateInterval",
                "Ogni quanto chiedere a Open-Meteo condizioni aggiornate. Open-Meteo aggiorna i dati " +
                "circa ogni 15 minuti, quindi intervalli più brevi servono a poco." },

            { "enum.UpdateInterval.FifteenMinutes", "15 minuti" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 minuti" },
            { "enum.UpdateInterval.SixtyMinutes", "60 minuti" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "Città" },
            { "desc.CityQuery",
                "La città reale da cui copiare il meteo. Esempi: Lione - Lione, Francia - Milazzo, " +
                "Italia - New York, Stati Uniti. Aggiungi uno stato o una regione dopo la virgola " +
                "per restringere la ricerca." },

            { "label.SearchCity", "Cerca" },
            { "desc.SearchCity",
                "Cerca il nome ed elenca qui sotto tutte le città corrispondenti, così puoi " +
                "confermare quella giusta invece di fidarti di una singola ipotesi." },

            { "label.SelectedSearchResult", "Risultati della ricerca" },
            { "desc.SelectedSearchResult",
                "Città corrispondenti alla ricerca, dalla più pertinente, con regione, stato e " +
                "coordinate. Sceglierne una la applica subito." },

            { "label.SelectedFavourite", "Città recenti" },
            { "desc.SelectedFavourite",
                "Città che hai già usato. Sceglierne una passa subito a quella, senza nuova ricerca." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Applica città" },
            { "desc.ApplyCity",
                "Usa la corrispondenza migliore per il nome digitato sopra, senza passare " +
                "dall'elenco. Se la città non viene trovata, resta il luogo risolto in precedenza." },

            { "label.RefreshWeatherNow", "Aggiorna il meteo ora" },
            { "desc.RefreshWeatherNow",
                "Scarica subito le condizioni attuali invece di aspettare il prossimo aggiornamento." },

            { "label.ApplyImmediately", "Applica subito" },
            { "desc.ApplyImmediately",
                "Aggiorna e passa direttamente al nuovo meteo, saltando la dissolvenza. Interrompe " +
                "anche una transizione già in corso." },

            { "label.ResetToGameWeather", "Ripristina il meteo del gioco" },
            { "desc.ResetToGameWeather",
                "Rilascia tutte le sovrascritture climatiche e restituisce il meteo al gioco. " +
                "Il meteo reale riprende quando applichi una città o forzi un aggiornamento." },
            { "warn.ResetToGameWeather",
                "Rilasciare tutte le sovrascritture meteo e restituire il controllo al gioco?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "Stato" },
            { "label.ResolvedLocationText", "Luogo risolto" },
            { "label.LastUpdateText", "Ultimo aggiornamento" },
            { "label.CurrentWeatherText", "Meteo attuale" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Sincronizza la nebbia" },
            { "desc.SyncFog",
                "Ricava la nebbia dai codici di nebbia segnalati e dalla visibilità. La copertura " +
                "nuvolosa non genera mai nebbia. Disattiva per lasciare in pace la nebbia del gioco." },

            { "label.SyncTemperature", "Sincronizza la temperatura" },
            { "desc.SyncTemperature",
                "Imposta la temperatura visibile in base alla città reale. La temperatura è il valore " +
                "riletto dal maggior numero di sistemi di gioco: fabbisogno di riscaldamento, " +
                "manutenzione, rischio incendi, turismo, neve al suolo. Disattivare questa opzione è " +
                "quindi il modo più efficace per ridurre al minimo l'influenza della mod. Il prezzo è " +
                "che il gioco non distingue più la pioggia dalla neve, e la temperatura mostrata è " +
                "quella del gioco." },

            { "label.SimulationImpactNote", "Cosa rilegge il gioco" },

            { "label.ForceSnowAppearance", "Mostra la neve quando nevica davvero" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II decide tra pioggia e neve in base alla temperatura visibile. " +
                "Quando il meteo reale è neve ma la temperatura reale è sopra lo zero, la temperatura " +
                "visibile viene abbassata appena sotto lo zero perché la neve venga disegnata. " +
                "La temperatura mostrata risulterà quindi diversa da quella reale. Stagione e data " +
                "restano comunque intoccate." },

            { "label.IgnoreModConflicts", "Ignora i conflitti tra mod" },
            { "desc.IgnoreModConflicts",
                "Per impostazione predefinita Real Weather Sync si disattiva quando è caricata " +
                "un'altra mod meteo nota, perché due mod che scrivono gli stessi valori climatici " +
                "finiscono per contendersi il controllo. Attiva questa opzione solo se sei certo che " +
                "l'altra mod non stia sovrascrivendo il meteo." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Sfasamento orario (ore)" },
            { "desc.TimeShiftHours",
                "Legge il meteo di un'altra ora: valori negativi per il passato, positivi per le " +
                "previsioni. A -24 la tua città vive il meteo di ieri; a +24 riceve quello di domani " +
                "con un giorno di anticipo. Si sposta solo la lettura meteo: orologio, data e " +
                "stagione del gioco restano intatti." },

            { "label.AntipodeMode", "Modalità antipodi" },
            { "desc.AntipodeMode",
                "Prende il meteo dal punto esattamente opposto del pianeta rispetto alla città " +
                "scelta. Per gran parte dell'Europa è il centro del Pacifico meridionale, quindi " +
                "aspettati parecchia pioggerella grigia sull'oceano. La tua città resta quella che " +
                "hai scelto: dall'altra parte del mondo arriva solo il meteo." },

            { "label.ExtremeLocation", "Portami in un posto orribile" },
            { "desc.ExtremeLocation",
                "Salta direttamente in un luogo famigerato. È anche il modo più rapido per vedere " +
                "neve, nebbia o un acquazzone senza aspettare che il meteo di casa collabori. " +
                "Torna a Nessuno al riavvio, ma la città che ha impostato viene conservata come " +
                "qualsiasi altra." },

            { "label.OppositeDay", "Giorno al contrario" },
            { "desc.OppositeDay",
                "Ribalta il meteo. Il caldo diventa freddo, il sereno diventa coperto, l'asciutto " +
                "diventa fradicio. La nebbia resta esclusa, perché una nebbia permanente nasconde la " +
                "città e smette subito di essere divertente." },

            { "enum.ExtremeLocation.None", "Resta dove sei" },
            { "enum.ExtremeLocation.Yakutsk", "Jakutsk - la città più fredda della Terra" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - notte polare, giorno polare, orsi polari" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaia - la fine del mondo, e si vede" },
            { "enum.ExtremeLocation.Reykjavik", "Reykjavík - vento e pioggia, in orizzontale" },
            { "enum.ExtremeLocation.MountWashington", "Mount Washington - nebbia, e il peggior meteo d'America" },
            { "enum.ExtremeLocation.DeathValley", "Valle della Morte - il luogo più caldo mai registrato" },
            { "enum.ExtremeLocation.Cherrapunji", "Cherrapunji - uno dei posti più piovosi della Terra" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "Informazioni" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Dati meteo forniti da Open-Meteo (open-meteo.com), con licenza CC BY 4.0. Non servono account né chiavi API.\n" +
                "A Open-Meteo vengono inviati soltanto il nome della città che digiti e le coordinate ricavate da esso." },

            { "key.About.SimulationImpact",
                "Real Weather Sync scrive gli stessi quattro valori climatici che scrivono gli " +
                "strumenti meteo di sviluppo del gioco, e una parte del gioco li rilegge. Fabbisogno " +
                "di riscaldamento e raffrescamento, manutenzione degli edifici, rischio incendi, " +
                "svago, turismo, neve al suolo, bagnato delle superfici ed eventi meteo rispondono " +
                "tutti a temperatura e precipitazioni, esattamente come rispondono al meteo del " +
                "gioco.\n" +
                "La mod non aggiunge sistemi, non cambia regole e non scrive nulla nel salvataggio.\n" +
                "Disattivare \"Sincronizza la temperatura\" elimina la parte più consistente di " +
                "questo effetto, al prezzo della precisione tra pioggia e neve. Resa solare e falda " +
                "acquifera non vengono mai toccate, e la nebbia non influisce su nulla al di fuori " +
                "della resa visiva." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Disattivato" },
            { "key.Status.CityNotConfigured", "Nessuna città impostata" },
            { "key.Status.ResolvingLocation", "Risoluzione del luogo in corso" },
            { "key.Status.Refreshing", "Aggiornamento del meteo" },
            { "key.Status.CandidatesReady", "Scegli una città dai risultati della ricerca" },
            { "key.Status.Connected", "Connesso" },
            { "key.Status.Offline", "Offline - in uso l'ultimo meteo valido" },
            { "key.Status.ErrorResolvingCity", "Errore nella risoluzione della città" },
            { "key.Status.IncompatibleMod", "Mod meteo incompatibile attiva" },
            { "key.Status.Released", "Sovrascritture rilasciate - in uso il meteo del gioco" },
            { "key.Status.WaitingForGame", "In attesa del caricamento di una città" },

            { "key.Location.NotResolved", "Nessun luogo ancora risolto" },

            { "key.LastUpdate.Never", "Mai" },
            { "key.LastUpdate.JustNow", "proprio ora" },
            { "key.LastUpdate.MinutesAgo", "{0} min fa" },

            { "key.Weather.NoData", "Nessun dato meteo ricevuto finora." },
            { "key.Weather.Observed", "Osservato" },
            { "key.Weather.Applied", "Applicato" },
            { "key.Weather.Clouds", "nuvole" },
            { "key.Weather.Precipitation", "precipitazioni" },
            { "key.Weather.Snow", "neve" },
            { "key.Weather.Code", "codice WMO" },
            { "key.Weather.Visibility", "visibilità" },
            { "key.Weather.Fog", "nebbia" },
            { "key.Weather.Conditions", "Condizioni" },
            { "key.Weather.TimeShiftPast", "{0} h nel passato" },
            { "key.Weather.TimeShiftFuture", "{0} h in avanti - previsione" },
            { "key.Weather.OppositeDay", "Giorno al contrario" },
            { "key.Weather.Antipode", "Antipodi" },

            { "key.Search.NoResults", "Nessun risultato - premi Cerca" },
            { "key.Search.PickOne", "Seleziona una città..." },
            { "key.Favourites.Empty", "Ancora nessuna città recente" },

            { "key.Overrides.Active", "Le sovrascritture climatiche sono attive." },
            { "key.Overrides.Inactive", "Le sovrascritture climatiche non sono attive." },

            { "key.Error.CityNotFound", "Nessuna città corrispondente" },
            { "key.Error.EmptyCity", "Inserisci prima il nome di una città" },
            { "key.Error.Network", "Impossibile raggiungere Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo sta limitando le richieste" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Cielo sereno" },
            { "wmo.MainlyClear", "Prevalentemente sereno" },
            { "wmo.PartlyCloudy", "Parzialmente nuvoloso" },
            { "wmo.Overcast", "Coperto" },
            { "wmo.Fog", "Nebbia" },
            { "wmo.RimeFog", "Nebbia con galaverna" },
            { "wmo.DrizzleLight", "Pioggerella leggera" },
            { "wmo.DrizzleModerate", "Pioggerella moderata" },
            { "wmo.DrizzleDense", "Pioggerella fitta" },
            { "wmo.FreezingDrizzleLight", "Pioggerella gelata leggera" },
            { "wmo.FreezingDrizzleDense", "Pioggerella gelata fitta" },
            { "wmo.RainSlight", "Pioggia debole" },
            { "wmo.RainModerate", "Pioggia moderata" },
            { "wmo.RainHeavy", "Pioggia intensa" },
            { "wmo.FreezingRainLight", "Pioggia gelata debole" },
            { "wmo.FreezingRainHeavy", "Pioggia gelata intensa" },
            { "wmo.SnowSlight", "Neve debole" },
            { "wmo.SnowModerate", "Neve moderata" },
            { "wmo.SnowHeavy", "Neve intensa" },
            { "wmo.SnowGrains", "Granelli di neve" },
            { "wmo.ShowersSlight", "Rovesci deboli" },
            { "wmo.ShowersModerate", "Rovesci moderati" },
            { "wmo.ShowersViolent", "Rovesci violenti" },
            { "wmo.SnowShowersSlight", "Rovesci di neve deboli" },
            { "wmo.SnowShowersHeavy", "Rovesci di neve intensi" },
            { "wmo.Thunderstorm", "Temporale" },
            { "wmo.ThunderstormHailSlight", "Temporale con grandine debole" },
            { "wmo.ThunderstormHailHeavy", "Temporale con grandine intensa" },
            { "wmo.Unknown", "Condizioni sconosciute" }
        };
    }
}
