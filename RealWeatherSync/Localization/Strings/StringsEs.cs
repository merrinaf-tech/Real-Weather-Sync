namespace RealWeatherSync.Localization.Strings
{
    /// <summary>es-ES. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsEs
    {
        public const string LocaleId = "es-ES";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Principal" },

            { "group.GeneralGroup", "General" },
            { "group.SearchGroup", "Ciudad" },
            { "group.ActionsGroup", "Acciones" },
            { "group.StatusGroup", "Estado" },
            { "group.AdvancedGroup", "Avanzado" },
            { "group.SillyGroup", "Opciones que nadie pidió" },
            { "group.AboutGroup", "Acerca de" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Activar el tiempo real" },
            { "desc.EnableRealWeather",
                "Ajusta el tiempo visible de tu ciudad al tiempo real actual de la ciudad elegida " +
                "abajo. El reloj, la fecha y la estación nunca se modifican, pero el juego sí vuelve " +
                "a leer los valores meteorológicos: consulta «Qué vuelve a leer el juego» en " +
                "Avanzado. Al desactivarlo, el tiempo vuelve de inmediato al control del juego." },

            { "label.FollowGameClock", "Seguir el reloj del juego" },
            { "desc.FollowGameClock",
                "En lugar de una única lectura fija, recorre las últimas 24 horas de tiempo real de " +
                "la ciudad elegida siguiendo la hora del juego. Si en tu ciudad son las 15:00, " +
                "obtienes el tiempo real que tuvo la ciudad elegida en sus últimas 15:00, de modo " +
                "que el tiempo cambia a lo largo del día. El reloj del juego solo se lee, nunca se " +
                "modifica: la hora, la fecha y la estación quedan exactamente como las fijó el " +
                "juego. Mientras está activo, prevalece sobre el desfase horario manual." },

            { "label.SmoothTransitions", "Transiciones meteorológicas suaves" },
            { "desc.SmoothTransitions",
                "Funde gradualmente hacia cada nueva lectura en vez de cambiar de golpe. El fundido " +
                "usa tiempo real, así que no le afectan la pausa ni la velocidad de simulación." },

            { "label.TransitionSeconds", "Duración de la transición (segundos)" },
            { "desc.TransitionSeconds",
                "Cuánto dura un fundido entre dos lecturas meteorológicas, en segundos reales. " +
                "Solo se usa con las transiciones suaves activadas." },

            { "label.UpdateInterval", "Intervalo de actualización" },
            { "desc.UpdateInterval",
                "Con qué frecuencia se piden condiciones nuevas a Open-Meteo. Open-Meteo actualiza " +
                "sus datos aproximadamente cada 15 minutos, así que intervalos más cortos aportan poco." },

            { "enum.UpdateInterval.FifteenMinutes", "15 minutos" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 minutos" },
            { "enum.UpdateInterval.SixtyMinutes", "60 minutos" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "Ciudad" },
            { "desc.CityQuery",
                "La ciudad real de la que se copia el tiempo. Ejemplos: Lyon - Lyon, Francia - " +
                "Milazzo, Italia - Nueva York, Estados Unidos. Añade un país o una región tras una " +
                "coma para acotar la búsqueda." },

            { "label.SearchCity", "Buscar" },
            { "desc.SearchCity",
                "Busca el nombre y muestra abajo todas las ciudades coincidentes, para que confirmes " +
                "la correcta en lugar de fiarte de una sola suposición." },

            { "label.SelectedSearchResult", "Resultados de la búsqueda" },
            { "desc.SelectedSearchResult",
                "Ciudades que coinciden con tu búsqueda, la mejor coincidencia primero, con su " +
                "región, país y coordenadas. Elegir una la aplica al instante." },

            { "label.SelectedFavourite", "Ciudades recientes" },
            { "desc.SelectedFavourite",
                "Ciudades que ya has usado. Elegir una cambia a ella de inmediato, sin nueva búsqueda." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Aplicar ciudad" },
            { "desc.ApplyCity",
                "Usa la mejor coincidencia del nombre escrito arriba, sin elegir de la lista. Si no " +
                "se encuentra la ciudad, se conserva la ubicación resuelta anteriormente." },

            { "label.RefreshWeatherNow", "Actualizar el tiempo ahora" },
            { "desc.RefreshWeatherNow",
                "Obtiene las condiciones actuales de inmediato en vez de esperar a la próxima " +
                "actualización." },

            { "label.ApplyImmediately", "Aplicar de inmediato" },
            { "desc.ApplyImmediately",
                "Actualiza y salta directamente al nuevo tiempo, sin fundido. También interrumpe una " +
                "transición que ya esté en curso." },

            { "label.ResetToGameWeather", "Restaurar el tiempo del juego" },
            { "desc.ResetToGameWeather",
                "Libera todas las anulaciones climáticas y devuelve el control del tiempo al juego. " +
                "El tiempo real se reanuda cuando apliques una ciudad o fuerces una actualización." },
            { "warn.ResetToGameWeather",
                "¿Liberar todas las anulaciones meteorológicas y devolver el control al juego?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "Estado" },
            { "label.ResolvedLocationText", "Ubicación resuelta" },
            { "label.LastUpdateText", "Última actualización" },
            { "label.CurrentWeatherText", "Tiempo actual" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Sincronizar la niebla" },
            { "desc.SyncFog",
                "Deduce la niebla de los códigos de niebla notificados y de la visibilidad. La " +
                "nubosidad nunca genera niebla. Desactívalo para dejar intacta la niebla del juego." },

            { "label.SyncTemperature", "Sincronizar la temperatura" },
            { "desc.SyncTemperature",
                "Fija la temperatura visible a partir de la ciudad real. La temperatura es el valor " +
                "que vuelve a leer el mayor número de sistemas del juego: demanda de calefacción, " +
                "mantenimiento, riesgo de incendio, turismo, nieve en el suelo. Desactivar esta " +
                "opción es, por tanto, la forma más eficaz de reducir al mínimo la influencia del " +
                "mod. El precio es que el juego ya no distingue la lluvia de la nieve, y la " +
                "temperatura que ves es la del propio juego." },

            { "label.SimulationImpactNote", "Qué vuelve a leer el juego" },

            { "label.ForceSnowAppearance", "Mostrar nieve cuando de verdad nieva" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II decide entre lluvia y nieve según la temperatura visible. " +
                "Cuando el tiempo real es de nieve pero la temperatura real está por encima de cero, " +
                "la temperatura visible baja justo por debajo de cero para que se dibuje la nieve. " +
                "La temperatura mostrada diferirá entonces de la real. La estación y la fecha siguen " +
                "sin modificarse nunca." },

            { "label.IgnoreModConflicts", "Ignorar conflictos entre mods" },
            { "desc.IgnoreModConflicts",
                "De forma predeterminada, Real Weather Sync se desactiva cuando hay cargado otro mod " +
                "meteorológico conocido, porque dos mods que escriben los mismos valores climáticos " +
                "acaban peleándose. Activa esto solo si estás seguro de que el otro mod no está " +
                "anulando el tiempo." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Desfase horario (horas)" },
            { "desc.TimeShiftHours",
                "Lee el tiempo de otra hora: negativo para el pasado, positivo para la previsión. " +
                "En -24 tu ciudad vive el tiempo de ayer; en +24 recibe el de mañana con un día de " +
                "antelación. Solo se desplaza la lectura meteorológica: el reloj, la fecha y la " +
                "estación del juego quedan intactos." },

            { "label.AntipodeMode", "Modo antípoda" },
            { "desc.AntipodeMode",
                "Toma el tiempo del punto exactamente opuesto del planeta respecto a la ciudad " +
                "elegida. Para casi toda Europa eso es el centro del Pacífico Sur, así que espera " +
                "mucha llovizna gris oceánica. Tu ciudad sigue siendo la que elegiste: del otro lado " +
                "del mundo solo llega el tiempo." },

            { "label.ExtremeLocation", "Llévame a un sitio horrible" },
            { "desc.ExtremeLocation",
                "Salta directamente a un lugar tristemente célebre. También es la forma más rápida " +
                "de ver nieve, niebla o un aguacero sin esperar a que el tiempo de casa colabore. " +
                "Vuelve a Ninguno al reiniciar, pero la ciudad que eligió se conserva como cualquier " +
                "otra." },

            { "label.OppositeDay", "Día al revés" },
            { "desc.OppositeDay",
                "Invierte el tiempo. El calor se vuelve frío, el cielo despejado se vuelve cubierto, " +
                "lo seco se vuelve calado. La niebla se queda como está, porque una niebla permanente " +
                "tapa la ciudad y deja de tener gracia al instante." },

            { "enum.ExtremeLocation.None", "Quédate donde estás" },
            { "enum.ExtremeLocation.Yakutsk", "Yakutsk - la ciudad más fría de la Tierra" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - noche polar, día polar, osos polares" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaia - el fin del mundo, y se nota" },
            { "enum.ExtremeLocation.Reykjavik", "Reikiavik - viento y lluvia, en horizontal" },
            { "enum.ExtremeLocation.MountWashington", "Monte Washington - niebla, y el peor tiempo de América" },
            { "enum.ExtremeLocation.DeathValley", "Valle de la Muerte - el lugar más caluroso jamás registrado" },
            { "enum.ExtremeLocation.Cherrapunji", "Cherrapunji - uno de los lugares más lluviosos de la Tierra" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "Acerca de" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Datos meteorológicos de Open-Meteo (open-meteo.com), con licencia CC BY 4.0. No hacen falta cuenta ni clave de API.\n" +
                "A Open-Meteo solo se envían el nombre de ciudad que escribes y las coordenadas obtenidas a partir de él." },

            { "key.About.SimulationImpact",
                "Real Weather Sync escribe los mismos cuatro valores climáticos que escriben las " +
                "herramientas meteorológicas de desarrollo del propio juego, y parte del juego " +
                "vuelve a leer esos valores. La demanda de calefacción y refrigeración, el " +
                "mantenimiento de edificios, el riesgo de incendio, el ocio, el turismo, la nieve en " +
                "el suelo, la humedad de las superficies y los eventos meteorológicos responden " +
                "todos a la temperatura y a las precipitaciones, exactamente igual que responden al " +
                "tiempo propio del juego.\n" +
                "El mod no añade sistemas, no cambia reglas y no escribe nada en tu partida " +
                "guardada.\n" +
                "Desactivar «Sincronizar la temperatura» elimina la mayor parte de este efecto, a " +
                "costa de la precisión entre lluvia y nieve. La producción solar y las aguas " +
                "subterráneas no se ven afectadas nunca, y la niebla no influye en nada fuera de lo " +
                "visual." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Desactivado" },
            { "key.Status.CityNotConfigured", "Ciudad sin configurar" },
            { "key.Status.ResolvingLocation", "Resolviendo la ubicación" },
            { "key.Status.Refreshing", "Actualizando el tiempo" },
            { "key.Status.CandidatesReady", "Elige una ciudad de los resultados" },
            { "key.Status.Connected", "Conectado" },
            { "key.Status.Offline", "Sin conexión - usando el último tiempo válido" },
            { "key.Status.ErrorResolvingCity", "Error al resolver la ciudad" },
            { "key.Status.IncompatibleMod", "Mod meteorológico incompatible activo" },
            { "key.Status.Released", "Anulaciones liberadas - usando el tiempo del juego" },
            { "key.Status.WaitingForGame", "Esperando a que se cargue una ciudad" },

            { "key.Location.NotResolved", "Todavía no se ha resuelto ninguna ubicación" },

            { "key.LastUpdate.Never", "Nunca" },
            { "key.LastUpdate.JustNow", "ahora mismo" },
            { "key.LastUpdate.MinutesAgo", "hace {0} min" },

            { "key.Weather.NoData", "Aún no se han recibido datos meteorológicos." },
            { "key.Weather.Observed", "Observado" },
            { "key.Weather.Applied", "Aplicado" },
            { "key.Weather.Clouds", "nubes" },
            { "key.Weather.Precipitation", "precipitación" },
            { "key.Weather.Snow", "nieve" },
            { "key.Weather.Code", "código WMO" },
            { "key.Weather.Visibility", "visibilidad" },
            { "key.Weather.Fog", "niebla" },
            { "key.Weather.Conditions", "Condiciones" },
            { "key.Weather.TimeShiftPast", "hace {0} h" },
            { "key.Weather.TimeShiftFuture", "{0} h por delante - previsión" },
            { "key.Weather.OppositeDay", "Día al revés" },
            { "key.Weather.Antipode", "Antípoda" },

            { "key.Search.NoResults", "Sin resultados - pulsa Buscar" },
            { "key.Search.PickOne", "Selecciona una ciudad..." },
            { "key.Favourites.Empty", "Todavía no hay ciudades recientes" },

            { "key.Overrides.Active", "Las anulaciones climáticas están activas." },
            { "key.Overrides.Inactive", "Las anulaciones climáticas no están activas." },

            { "key.Error.CityNotFound", "No se ha encontrado ninguna ciudad" },
            { "key.Error.EmptyCity", "Escribe primero el nombre de una ciudad" },
            { "key.Error.Network", "No se ha podido contactar con Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo está limitando las peticiones" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Cielo despejado" },
            { "wmo.MainlyClear", "Mayormente despejado" },
            { "wmo.PartlyCloudy", "Parcialmente nuboso" },
            { "wmo.Overcast", "Cubierto" },
            { "wmo.Fog", "Niebla" },
            { "wmo.RimeFog", "Niebla engelante" },
            { "wmo.DrizzleLight", "Llovizna débil" },
            { "wmo.DrizzleModerate", "Llovizna moderada" },
            { "wmo.DrizzleDense", "Llovizna densa" },
            { "wmo.FreezingDrizzleLight", "Llovizna engelante débil" },
            { "wmo.FreezingDrizzleDense", "Llovizna engelante densa" },
            { "wmo.RainSlight", "Lluvia débil" },
            { "wmo.RainModerate", "Lluvia moderada" },
            { "wmo.RainHeavy", "Lluvia intensa" },
            { "wmo.FreezingRainLight", "Lluvia engelante débil" },
            { "wmo.FreezingRainHeavy", "Lluvia engelante intensa" },
            { "wmo.SnowSlight", "Nieve débil" },
            { "wmo.SnowModerate", "Nieve moderada" },
            { "wmo.SnowHeavy", "Nieve intensa" },
            { "wmo.SnowGrains", "Cinarra" },
            { "wmo.ShowersSlight", "Chubascos débiles" },
            { "wmo.ShowersModerate", "Chubascos moderados" },
            { "wmo.ShowersViolent", "Chubascos violentos" },
            { "wmo.SnowShowersSlight", "Chubascos de nieve débiles" },
            { "wmo.SnowShowersHeavy", "Chubascos de nieve intensos" },
            { "wmo.Thunderstorm", "Tormenta" },
            { "wmo.ThunderstormHailSlight", "Tormenta con granizo débil" },
            { "wmo.ThunderstormHailHeavy", "Tormenta con granizo intenso" },
            { "wmo.Unknown", "Condiciones desconocidas" }
        };
    }
}
