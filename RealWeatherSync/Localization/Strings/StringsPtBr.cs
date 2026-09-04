namespace RealWeatherSync.Localization.Strings
{
    /// <summary>pt-BR. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsPtBr
    {
        public const string LocaleId = "pt-BR";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Principal" },

            { "group.GeneralGroup", "Geral" },
            { "group.SearchGroup", "Cidade" },
            { "group.ActionsGroup", "Ações" },
            { "group.StatusGroup", "Status" },
            { "group.AdvancedGroup", "Avançado" },
            { "group.SillyGroup", "Opções que ninguém pediu" },
            { "group.AboutGroup", "Sobre" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Ativar clima real" },
            { "desc.EnableRealWeather",
                "Faz o clima visível da sua cidade acompanhar o clima real atual da cidade escolhida " +
                "abaixo. O relógio, a data e a estação nunca são alterados, mas o jogo realmente lê " +
                "os valores climáticos de volta: veja \"O que o jogo lê de volta\" em Avançado. " +
                "Ao desativar, o clima volta imediatamente para o controle do jogo." },

            { "label.FollowGameClock", "Seguir o relógio do jogo" },
            { "desc.FollowGameClock",
                "Em vez de uma única leitura fixa, percorre as últimas 24 horas de clima real da " +
                "cidade escolhida seguindo a hora do jogo. Se são 15:00 na sua cidade, você recebe o " +
                "clima real que a cidade escolhida teve às suas últimas 15:00 - ou seja, o clima " +
                "muda conforme o dia passa. O relógio do jogo é apenas lido, nunca alterado: hora, " +
                "data e estação continuam exatamente como o jogo definiu. Enquanto ativo, tem " +
                "prioridade sobre o deslocamento de tempo manual." },

            { "label.SmoothTransitions", "Transições climáticas suaves" },
            { "desc.SmoothTransitions",
                "Faz uma transição gradual para cada nova leitura em vez de mudar de uma vez. " +
                "A transição usa tempo real, portanto não é afetada por pausa nem pela velocidade " +
                "de simulação." },

            { "label.TransitionSeconds", "Duração da transição (segundos)" },
            { "desc.TransitionSeconds",
                "Quanto tempo dura a transição entre duas leituras climáticas, em segundos reais. " +
                "Usado apenas com as transições suaves ativadas." },

            { "label.UpdateInterval", "Intervalo de atualização" },
            { "desc.UpdateInterval",
                "Com que frequência pedir condições novas ao Open-Meteo. O Open-Meteo atualiza os " +
                "dados a cada 15 minutos, mais ou menos, então intervalos menores ajudam pouco." },

            { "enum.UpdateInterval.FifteenMinutes", "15 minutos" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 minutos" },
            { "enum.UpdateInterval.SixtyMinutes", "60 minutos" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "Cidade" },
            { "desc.CityQuery",
                "A cidade real de onde o clima é copiado. Exemplos: Lyon - Lyon, França - Milazzo, " +
                "Itália - Nova York, Estados Unidos. Acrescente um país ou uma região depois de uma " +
                "vírgula para restringir a busca." },

            { "label.SearchCity", "Pesquisar" },
            { "desc.SearchCity",
                "Procura o nome e lista abaixo todas as cidades correspondentes, para você confirmar " +
                "a certa em vez de confiar em um único palpite." },

            { "label.SelectedSearchResult", "Resultados da pesquisa" },
            { "desc.SelectedSearchResult",
                "Cidades que correspondem à sua busca, a melhor primeiro, com região, país e " +
                "coordenadas. Escolher uma aplica na hora." },

            { "label.SelectedFavourite", "Cidades recentes" },
            { "desc.SelectedFavourite",
                "Cidades que você já usou. Escolher uma alterna imediatamente, sem nova consulta." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Aplicar cidade" },
            { "desc.ApplyCity",
                "Usa a melhor correspondência para o nome digitado acima, sem escolher na lista. " +
                "Se a cidade não for encontrada, o local resolvido anteriormente é mantido." },

            { "label.RefreshWeatherNow", "Atualizar o clima agora" },
            { "desc.RefreshWeatherNow",
                "Busca as condições atuais imediatamente, em vez de esperar a próxima atualização." },

            { "label.ApplyImmediately", "Aplicar imediatamente" },
            { "desc.ApplyImmediately",
                "Atualiza e pula direto para o novo clima, sem transição. Também interrompe uma " +
                "transição já em andamento." },

            { "label.ResetToGameWeather", "Restaurar o clima do jogo" },
            { "desc.ResetToGameWeather",
                "Libera todas as substituições climáticas e devolve o controle do clima ao jogo. " +
                "O clima real retorna quando você aplicar uma cidade ou forçar uma atualização." },
            { "warn.ResetToGameWeather",
                "Liberar todas as substituições de clima e devolver o controle ao jogo?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "Status" },
            { "label.ResolvedLocationText", "Local resolvido" },
            { "label.LastUpdateText", "Última atualização" },
            { "label.CurrentWeatherText", "Clima atual" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Sincronizar neblina" },
            { "desc.SyncFog",
                "Deriva a neblina dos códigos de neblina informados e da visibilidade. A cobertura " +
                "de nuvens nunca gera neblina. Desative para deixar a neblina do jogo intacta." },

            { "label.SyncTemperature", "Sincronizar temperatura" },
            { "desc.SyncTemperature",
                "Define a temperatura visível a partir da cidade real. A temperatura é o valor lido " +
                "de volta pelo maior número de sistemas do jogo - demanda de aquecimento, " +
                "manutenção, risco de incêndio, turismo, neve no chão - por isso desativar esta " +
                "opção é a maneira mais eficaz de manter a influência do mod no mínimo. O custo é " +
                "que o jogo deixa de distinguir chuva de neve, e a temperatura exibida passa a ser " +
                "a do próprio jogo." },

            { "label.SimulationImpactNote", "O que o jogo lê de volta" },

            { "label.ForceSnowAppearance", "Mostrar neve quando estiver realmente nevando" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II decide entre chuva e neve pela temperatura visível. Quando o " +
                "clima real é neve mas a temperatura real está acima de zero, a temperatura visível " +
                "é reduzida para logo abaixo de zero para que a neve seja desenhada. A temperatura " +
                "exibida passará então a diferir da real. A estação e a data continuam nunca sendo " +
                "alteradas." },

            { "label.IgnoreModConflicts", "Ignorar conflitos entre mods" },
            { "desc.IgnoreModConflicts",
                "Por padrão, o Real Weather Sync se desativa quando outro mod de clima conhecido " +
                "está carregado, porque dois mods gravando os mesmos valores climáticos brigam entre " +
                "si. Ative isto apenas se tiver certeza de que o outro mod não está substituindo o " +
                "clima." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Deslocamento de tempo (horas)" },
            { "desc.TimeShiftHours",
                "Lê o clima de outra hora: negativo para o passado, positivo para a previsão. " +
                "Em -24 sua cidade vive o clima de ontem; em +24 recebe o de amanhã com um dia de " +
                "antecedência. Apenas a leitura climática se desloca - relógio, data e estação do " +
                "jogo permanecem intocados." },

            { "label.AntipodeMode", "Modo antípoda" },
            { "desc.AntipodeMode",
                "Pega o clima do ponto exatamente oposto do planeta em relação à cidade escolhida. " +
                "Para boa parte da Europa isso é o meio do Pacífico Sul, então espere bastante " +
                "garoa cinzenta oceânica. Sua cidade continua sendo a que você escolheu - do outro " +
                "lado do mundo vem só o clima." },

            { "label.ExtremeLocation", "Me leve para um lugar horrível" },
            { "desc.ExtremeLocation",
                "Pula direto para um lugar de má fama. É também o jeito mais rápido de ver neve, " +
                "neblina ou um temporal sem esperar o clima de casa colaborar. Volta para Nenhum ao " +
                "reiniciar, mas a cidade escolhida é mantida como qualquer outra." },

            { "label.OppositeDay", "Dia ao contrário" },
            { "desc.OppositeDay",
                "Espelha o clima. Calor vira frio, céu limpo vira encoberto, seco vira encharcado. " +
                "A neblina fica de fora, porque neblina permanente esconde a cidade e deixa de ter " +
                "graça na hora." },

            { "enum.ExtremeLocation.None", "Fique onde está" },
            { "enum.ExtremeLocation.Yakutsk", "Yakutsk - a cidade mais fria da Terra" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - noite polar, dia polar, ursos-polares" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaia - o fim do mundo, e dá para perceber" },
            { "enum.ExtremeLocation.Reykjavik", "Reykjavík - vento e chuva, na horizontal" },
            { "enum.ExtremeLocation.MountWashington", "Monte Washington - neblina, e o pior clima da América" },
            { "enum.ExtremeLocation.DeathValley", "Vale da Morte - o lugar mais quente já registrado" },
            { "enum.ExtremeLocation.Cherrapunji", "Cherrapunji - um dos lugares mais chuvosos da Terra" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "Sobre" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Dados climáticos do Open-Meteo (open-meteo.com), sob licença CC BY 4.0. Não é preciso conta nem chave de API.\n" +
                "Só o nome da cidade que você digita e as coordenadas obtidas a partir dele são enviados ao Open-Meteo." },

            { "key.About.SimulationImpact",
                "O Real Weather Sync grava os mesmos quatro valores climáticos que as ferramentas de " +
                "clima de desenvolvimento do próprio jogo gravam, e parte do jogo lê esses valores " +
                "de volta. Demanda de aquecimento e resfriamento, manutenção de edifícios, risco de " +
                "incêndio, lazer, turismo, neve no chão, umidade das superfícies e eventos " +
                "climáticos respondem todos à temperatura e à precipitação - exatamente como " +
                "respondem ao clima do próprio jogo.\n" +
                "O mod não acrescenta sistemas, não muda regras e não grava nada no seu save.\n" +
                "Desativar \"Sincronizar temperatura\" remove a maior parte disso, ao custo da " +
                "precisão entre chuva e neve. Geração solar e água subterrânea nunca são afetadas, " +
                "e a neblina não influencia nada além do visual." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Desativado" },
            { "key.Status.CityNotConfigured", "Cidade não configurada" },
            { "key.Status.ResolvingLocation", "Resolvendo o local" },
            { "key.Status.Refreshing", "Atualizando o clima" },
            { "key.Status.CandidatesReady", "Escolha uma cidade nos resultados da pesquisa" },
            { "key.Status.Connected", "Conectado" },
            { "key.Status.Offline", "Offline - usando o último clima válido" },
            { "key.Status.ErrorResolvingCity", "Erro ao resolver a cidade" },
            { "key.Status.IncompatibleMod", "Mod de clima incompatível ativo" },
            { "key.Status.Released", "Substituições liberadas - usando o clima do jogo" },
            { "key.Status.WaitingForGame", "Aguardando o carregamento de uma cidade" },

            { "key.Location.NotResolved", "Nenhum local resolvido ainda" },

            { "key.LastUpdate.Never", "Nunca" },
            { "key.LastUpdate.JustNow", "agora mesmo" },
            { "key.LastUpdate.MinutesAgo", "há {0} min" },

            { "key.Weather.NoData", "Nenhum dado climático recebido ainda." },
            { "key.Weather.Observed", "Observado" },
            { "key.Weather.Applied", "Aplicado" },
            { "key.Weather.Clouds", "nuvens" },
            { "key.Weather.Precipitation", "precipitação" },
            { "key.Weather.Snow", "neve" },
            { "key.Weather.Code", "código WMO" },
            { "key.Weather.Visibility", "visibilidade" },
            { "key.Weather.Fog", "neblina" },
            { "key.Weather.Conditions", "Condições" },
            { "key.Weather.TimeShiftPast", "{0} h no passado" },
            { "key.Weather.TimeShiftFuture", "{0} h à frente - previsão" },
            { "key.Weather.OppositeDay", "Dia ao contrário" },
            { "key.Weather.Antipode", "Antípoda" },

            { "key.Search.NoResults", "Sem resultados - toque em Pesquisar" },
            { "key.Search.PickOne", "Selecione uma cidade..." },
            { "key.Favourites.Empty", "Ainda não há cidades recentes" },

            { "key.Overrides.Active", "As substituições climáticas estão ativas." },
            { "key.Overrides.Inactive", "As substituições climáticas não estão ativas." },

            { "key.Error.CityNotFound", "Nenhuma cidade correspondente encontrada" },
            { "key.Error.EmptyCity", "Digite primeiro o nome de uma cidade" },
            { "key.Error.Network", "Não foi possível acessar o Open-Meteo" },
            { "key.Error.RateLimited", "O Open-Meteo está limitando as requisições" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Céu limpo" },
            { "wmo.MainlyClear", "Predominantemente limpo" },
            { "wmo.PartlyCloudy", "Parcialmente nublado" },
            { "wmo.Overcast", "Encoberto" },
            { "wmo.Fog", "Neblina" },
            { "wmo.RimeFog", "Neblina com sincelo" },
            { "wmo.DrizzleLight", "Garoa fraca" },
            { "wmo.DrizzleModerate", "Garoa moderada" },
            { "wmo.DrizzleDense", "Garoa densa" },
            { "wmo.FreezingDrizzleLight", "Garoa congelante fraca" },
            { "wmo.FreezingDrizzleDense", "Garoa congelante densa" },
            { "wmo.RainSlight", "Chuva fraca" },
            { "wmo.RainModerate", "Chuva moderada" },
            { "wmo.RainHeavy", "Chuva forte" },
            { "wmo.FreezingRainLight", "Chuva congelante fraca" },
            { "wmo.FreezingRainHeavy", "Chuva congelante forte" },
            { "wmo.SnowSlight", "Neve fraca" },
            { "wmo.SnowModerate", "Neve moderada" },
            { "wmo.SnowHeavy", "Neve forte" },
            { "wmo.SnowGrains", "Grãos de neve" },
            { "wmo.ShowersSlight", "Pancadas de chuva fracas" },
            { "wmo.ShowersModerate", "Pancadas de chuva moderadas" },
            { "wmo.ShowersViolent", "Pancadas de chuva violentas" },
            { "wmo.SnowShowersSlight", "Pancadas de neve fracas" },
            { "wmo.SnowShowersHeavy", "Pancadas de neve fortes" },
            { "wmo.Thunderstorm", "Tempestade" },
            { "wmo.ThunderstormHailSlight", "Tempestade com granizo fraco" },
            { "wmo.ThunderstormHailHeavy", "Tempestade com granizo forte" },
            { "wmo.Unknown", "Condições desconhecidas" }
        };
    }
}
