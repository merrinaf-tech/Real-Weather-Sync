namespace RealWeatherSync.Localization.Strings
{
    /// <summary>fr-FR. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsFr
    {
        public const string LocaleId = "fr-FR";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Principal" },

            { "group.GeneralGroup", "Général" },
            { "group.SearchGroup", "Ville" },
            { "group.ActionsGroup", "Actions" },
            { "group.StatusGroup", "État" },
            { "group.AdvancedGroup", "Avancé" },
            { "group.SillyGroup", "Options que personne n'a demandées" },
            { "group.AboutGroup", "À propos" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Activer la météo réelle" },
            { "desc.EnableRealWeather",
                "Aligne la météo visible de votre ville sur la météo réelle actuelle de la ville " +
                "choisie ci-dessous. L'horloge, la date et la saison ne sont jamais modifiées, mais " +
                "le jeu relit bien les valeurs météo : voir « Ce que le jeu relit » dans Avancé. " +
                "Désactiver cette option rend immédiatement la main au jeu." },

            { "label.FollowGameClock", "Suivre l'horloge du jeu" },
            { "desc.FollowGameClock",
                "Au lieu d'un relevé figé, parcourt les 24 dernières heures de météo réelle de la " +
                "ville choisie en suivant l'heure du jeu. S'il est 15h00 dans votre ville, vous " +
                "obtenez la météo réelle qu'avait la ville choisie à son dernier 15h00 : la météo " +
                "évolue donc au fil de la journée. L'horloge du jeu est seulement lue, jamais " +
                "modifiée : l'heure, la date et la saison restent exactement telles que le jeu les a " +
                "définies. Prend le pas sur le décalage horaire manuel tant qu'elle est active." },

            { "label.SmoothTransitions", "Transitions météo progressives" },
            { "desc.SmoothTransitions",
                "Fond enchaîné vers chaque nouveau relevé au lieu d'un basculement instantané. " +
                "La transition suit le temps réel : la pause et la vitesse de simulation n'ont aucun effet." },

            { "label.TransitionSeconds", "Durée de transition (secondes)" },
            { "desc.TransitionSeconds",
                "Durée, en secondes réelles, d'un fondu entre deux relevés météo. " +
                "Utilisée uniquement si les transitions progressives sont activées." },

            { "label.UpdateInterval", "Intervalle de mise à jour" },
            { "desc.UpdateInterval",
                "Fréquence des requêtes vers Open-Meteo. Open-Meteo actualise ses données environ " +
                "toutes les 15 minutes : des intervalles plus courts n'apportent presque rien." },

            { "enum.UpdateInterval.FifteenMinutes", "15 minutes" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 minutes" },
            { "enum.UpdateInterval.SixtyMinutes", "60 minutes" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "Ville" },
            { "desc.CityQuery",
                "La ville réelle dont la météo est copiée. Exemples : Lyon - Lyon, France - Milazzo, " +
                "Italie - New York, États-Unis. Ajoutez un pays ou une région après une virgule pour " +
                "affiner la recherche." },

            { "label.SearchCity", "Rechercher" },
            { "desc.SearchCity",
                "Recherche le nom et liste ci-dessous toutes les villes correspondantes, pour que " +
                "vous confirmiez la bonne au lieu de vous fier à une seule supposition." },

            { "label.SelectedSearchResult", "Résultats de recherche" },
            { "desc.SelectedSearchResult",
                "Villes correspondant à votre recherche, meilleure correspondance en premier, avec " +
                "région, pays et coordonnées. En choisir une l'applique aussitôt." },

            { "label.SelectedFavourite", "Villes récentes" },
            { "desc.SelectedFavourite",
                "Villes que vous avez déjà utilisées. En choisir une bascule immédiatement, sans " +
                "nouvelle recherche." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Appliquer la ville" },
            { "desc.ApplyCity",
                "Utilise la meilleure correspondance pour le nom saisi ci-dessus, sans passer par la " +
                "liste. Si la ville reste introuvable, le lieu précédemment résolu est conservé." },

            { "label.RefreshWeatherNow", "Actualiser la météo maintenant" },
            { "desc.RefreshWeatherNow",
                "Récupère immédiatement les conditions actuelles au lieu d'attendre la prochaine " +
                "mise à jour." },

            { "label.ApplyImmediately", "Appliquer immédiatement" },
            { "desc.ApplyImmediately",
                "Actualise et passe directement à la nouvelle météo, sans fondu. Interrompt aussi " +
                "une transition déjà en cours." },

            { "label.ResetToGameWeather", "Rétablir la météo du jeu" },
            { "desc.ResetToGameWeather",
                "Libère toutes les surcharges climatiques et rend le contrôle de la météo au jeu. " +
                "La météo réelle reprend dès que vous appliquez une ville ou forcez une actualisation." },
            { "warn.ResetToGameWeather",
                "Libérer toutes les surcharges météo et rendre le contrôle au jeu ?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "État" },
            { "label.ResolvedLocationText", "Lieu résolu" },
            { "label.LastUpdateText", "Dernière mise à jour" },
            { "label.CurrentWeatherText", "Météo actuelle" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Synchroniser le brouillard" },
            { "desc.SyncFog",
                "Déduit le brouillard des codes de brouillard signalés et de la visibilité. La " +
                "couverture nuageuse ne produit jamais de brouillard. Désactivez pour laisser le " +
                "brouillard du jeu intact." },

            { "label.SyncTemperature", "Synchroniser la température" },
            { "desc.SyncTemperature",
                "Pilote la température visible d'après la ville réelle. La température est la valeur " +
                "relue par le plus grand nombre de systèmes du jeu : besoin de chauffage, entretien, " +
                "risque d'incendie, tourisme, neige au sol. Désactiver cette option est donc le moyen " +
                "le plus efficace de réduire au minimum l'influence du mod. En contrepartie, le jeu " +
                "ne distingue plus la pluie de la neige, et la température affichée est celle du jeu." },

            { "label.SimulationImpactNote", "Ce que le jeu relit" },

            { "label.ForceSnowAppearance", "Afficher la neige quand il neige vraiment" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II choisit entre pluie et neige d'après la température visible. " +
                "Quand la météo réelle donne de la neige mais que la température réelle est " +
                "au-dessus de zéro, la température visible est abaissée juste sous zéro pour que la " +
                "neige s'affiche. La température affichée diffère alors de la température réelle. " +
                "La saison et la date, elles, ne sont toujours jamais modifiées." },

            { "label.IgnoreModConflicts", "Ignorer les conflits entre mods" },
            { "desc.IgnoreModConflicts",
                "Par défaut, Real Weather Sync se désactive lorsqu'un autre mod météo connu est " +
                "chargé, car deux mods écrivant les mêmes valeurs climatiques se disputent. " +
                "N'activez ceci que si vous êtes certain que l'autre mod ne surcharge pas la météo." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Décalage horaire (heures)" },
            { "desc.TimeShiftHours",
                "Lit la météo d'une autre heure : négatif pour le passé, positif pour les " +
                "prévisions. À -24, votre ville vit la météo d'hier ; à +24, elle reçoit celle de " +
                "demain avec un jour d'avance. Seul le relevé météo se déplace : l'horloge, la date " +
                "et la saison du jeu restent intactes." },

            { "label.AntipodeMode", "Mode antipode" },
            { "desc.AntipodeMode",
                "Prend la météo du point exactement opposé de la planète par rapport à la ville " +
                "choisie. Pour la majeure partie de l'Europe, c'est le milieu du Pacifique Sud : " +
                "attendez-vous à beaucoup de bruine grise sur l'océan. Votre ville reste celle que " +
                "vous avez choisie ; seule la météo vient de l'autre bout du monde." },

            { "label.ExtremeLocation", "Emmène-moi dans un endroit affreux" },
            { "desc.ExtremeLocation",
                "Saute directement vers un lieu tristement célèbre. C'est aussi le moyen le plus " +
                "rapide de voir de la neige, du brouillard ou une averse sans attendre le bon " +
                "vouloir de la météo locale. Revient à Aucun au redémarrage, mais la ville choisie " +
                "est conservée comme n'importe quelle autre." },

            { "label.OppositeDay", "Jour inversé" },
            { "desc.OppositeDay",
                "Inverse la météo. Le chaud devient froid, le ciel dégagé devient couvert, le sec " +
                "devient trempé. Le brouillard est épargné : un brouillard permanent masque la ville " +
                "et cesse d'être drôle immédiatement." },

            { "enum.ExtremeLocation.None", "Restez où vous êtes" },
            { "enum.ExtremeLocation.Yakutsk", "Iakoutsk - la ville la plus froide du monde" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - nuit polaire, jour polaire, ours polaires" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaïa - le bout du monde, et ça se voit" },
            { "enum.ExtremeLocation.Reykjavik", "Reykjavík - du vent et de la pluie, à l'horizontale" },
            { "enum.ExtremeLocation.MountWashington", "Mont Washington - le brouillard, et la pire météo d'Amérique" },
            { "enum.ExtremeLocation.DeathValley", "Vallée de la Mort - l'endroit le plus chaud jamais mesuré" },
            { "enum.ExtremeLocation.Cherrapunji", "Cherrapunji - l'un des endroits les plus arrosés de la Terre" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "À propos" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Données météo fournies par Open-Meteo (open-meteo.com), sous licence CC BY 4.0. Aucun compte ni clé d'API nécessaire.\n" +
                "Seuls le nom de ville que vous saisissez et les coordonnées qui en découlent sont envoyés à Open-Meteo." },

            { "key.About.SimulationImpact",
                "Real Weather Sync écrit les quatre mêmes valeurs climatiques que les outils météo " +
                "de développement du jeu, et une partie du jeu relit ces valeurs. Besoins de " +
                "chauffage et de climatisation, entretien des bâtiments, risque d'incendie, loisirs, " +
                "tourisme, neige au sol, humidité des surfaces et événements météo réagissent tous à " +
                "la température et aux précipitations, exactement comme ils réagissent à la météo du " +
                "jeu.\n" +
                "Le mod n'ajoute aucun système, ne change aucune règle et n'écrit rien dans votre " +
                "sauvegarde.\n" +
                "Désactiver « Synchroniser la température » en supprime la plus grande part, au prix " +
                "de la justesse pluie/neige. La production solaire et les nappes phréatiques ne sont " +
                "jamais affectées, et le brouillard n'agit que sur le rendu." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Désactivé" },
            { "key.Status.CityNotConfigured", "Aucune ville configurée" },
            { "key.Status.ResolvingLocation", "Résolution du lieu" },
            { "key.Status.Refreshing", "Actualisation de la météo" },
            { "key.Status.CandidatesReady", "Choisissez une ville dans les résultats" },
            { "key.Status.Connected", "Connecté" },
            { "key.Status.Offline", "Hors ligne - dernière météo valide utilisée" },
            { "key.Status.ErrorResolvingCity", "Erreur lors de la résolution de la ville" },
            { "key.Status.IncompatibleMod", "Mod météo incompatible actif" },
            { "key.Status.Released", "Surcharges libérées - météo du jeu active" },
            { "key.Status.WaitingForGame", "En attente du chargement d'une ville" },

            { "key.Location.NotResolved", "Aucun lieu résolu pour l'instant" },

            { "key.LastUpdate.Never", "Jamais" },
            { "key.LastUpdate.JustNow", "à l'instant" },
            { "key.LastUpdate.MinutesAgo", "il y a {0} min" },

            { "key.Weather.NoData", "Aucune donnée météo reçue pour l'instant." },
            { "key.Weather.Observed", "Observé" },
            { "key.Weather.Applied", "Appliqué" },
            { "key.Weather.Clouds", "nuages" },
            { "key.Weather.Precipitation", "précipitations" },
            { "key.Weather.Snow", "neige" },
            { "key.Weather.Code", "code WMO" },
            { "key.Weather.Visibility", "visibilité" },
            { "key.Weather.Fog", "brouillard" },
            { "key.Weather.Conditions", "Conditions" },
            { "key.Weather.TimeShiftPast", "il y a {0} h" },
            { "key.Weather.TimeShiftFuture", "{0} h à l'avance - prévision" },
            { "key.Weather.OppositeDay", "Jour inversé" },
            { "key.Weather.Antipode", "Antipode" },

            { "key.Search.NoResults", "Aucun résultat - appuyez sur Rechercher" },
            { "key.Search.PickOne", "Choisir une ville..." },
            { "key.Favourites.Empty", "Aucune ville récente" },

            { "key.Overrides.Active", "Les surcharges climatiques sont actives." },
            { "key.Overrides.Inactive", "Les surcharges climatiques ne sont pas actives." },

            { "key.Error.CityNotFound", "Aucune ville correspondante" },
            { "key.Error.EmptyCity", "Saisissez d'abord un nom de ville" },
            { "key.Error.Network", "Impossible de joindre Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo limite le débit des requêtes" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Ciel dégagé" },
            { "wmo.MainlyClear", "Plutôt dégagé" },
            { "wmo.PartlyCloudy", "Partiellement nuageux" },
            { "wmo.Overcast", "Couvert" },
            { "wmo.Fog", "Brouillard" },
            { "wmo.RimeFog", "Brouillard givrant" },
            { "wmo.DrizzleLight", "Bruine légère" },
            { "wmo.DrizzleModerate", "Bruine modérée" },
            { "wmo.DrizzleDense", "Bruine dense" },
            { "wmo.FreezingDrizzleLight", "Bruine verglaçante légère" },
            { "wmo.FreezingDrizzleDense", "Bruine verglaçante dense" },
            { "wmo.RainSlight", "Pluie faible" },
            { "wmo.RainModerate", "Pluie modérée" },
            { "wmo.RainHeavy", "Forte pluie" },
            { "wmo.FreezingRainLight", "Pluie verglaçante faible" },
            { "wmo.FreezingRainHeavy", "Forte pluie verglaçante" },
            { "wmo.SnowSlight", "Neige faible" },
            { "wmo.SnowModerate", "Neige modérée" },
            { "wmo.SnowHeavy", "Neige forte" },
            { "wmo.SnowGrains", "Neige en grains" },
            { "wmo.ShowersSlight", "Faibles averses de pluie" },
            { "wmo.ShowersModerate", "Averses de pluie modérées" },
            { "wmo.ShowersViolent", "Averses de pluie violentes" },
            { "wmo.SnowShowersSlight", "Faibles averses de neige" },
            { "wmo.SnowShowersHeavy", "Fortes averses de neige" },
            { "wmo.Thunderstorm", "Orage" },
            { "wmo.ThunderstormHailSlight", "Orage avec faible grêle" },
            { "wmo.ThunderstormHailHeavy", "Orage avec forte grêle" },
            { "wmo.Unknown", "Conditions inconnues" }
        };
    }
}
