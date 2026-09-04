namespace RealWeatherSync.Localization.Strings
{
    /// <summary>ko-KR. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsKo
    {
        public const string LocaleId = "ko-KR";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "기본" },

            { "group.GeneralGroup", "일반" },
            { "group.SearchGroup", "도시" },
            { "group.ActionsGroup", "동작" },
            { "group.StatusGroup", "상태" },
            { "group.AdvancedGroup", "고급" },
            { "group.SillyGroup", "아무도 요청하지 않은 옵션" },
            { "group.AboutGroup", "정보" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "실제 날씨 사용" },
            { "desc.EnableRealWeather",
                "도시에 보이는 날씨를 아래에서 고른 도시의 현재 실제 날씨에 맞춥니다. 시계와 날짜, 계절은 " +
                "절대 바뀌지 않지만 게임은 날씨 값을 실제로 다시 읽습니다. 고급 항목의 " +
                "'게임이 다시 읽는 값'을 참고하세요. 끄면 날씨는 즉시 게임에 넘어갑니다." },

            { "label.FollowGameClock", "게임 내 시계 따라가기" },
            { "desc.FollowGameClock",
                "고정된 하나의 관측값 대신, 게임 내 시각에 맞춰 선택한 도시의 최근 24시간 실제 날씨를 " +
                "따라갑니다. 도시가 15:00라면 선택한 도시의 가장 최근 15:00 실제 날씨를 받습니다. " +
                "즉 하루가 흐르면서 날씨도 함께 바뀝니다. 게임 시계는 읽기만 할 뿐 절대 바꾸지 않으며, " +
                "시각과 날짜, 계절은 게임이 정한 그대로 유지됩니다. 켜져 있는 동안에는 수동 시간 " +
                "이동보다 우선합니다." },

            { "label.SmoothTransitions", "부드러운 날씨 전환" },
            { "desc.SmoothTransitions",
                "새 관측값으로 즉시 전환하지 않고 서서히 바꿉니다. 전환은 실제 시간을 기준으로 진행되므로 " +
                "일시정지나 시뮬레이션 속도의 영향을 받지 않습니다." },

            { "label.TransitionSeconds", "전환 길이(초)" },
            { "desc.TransitionSeconds",
                "두 관측값 사이의 전환에 걸리는 실제 시간(초)입니다. 부드러운 전환이 켜져 있을 때만 " +
                "사용됩니다." },

            { "label.UpdateInterval", "갱신 주기" },
            { "desc.UpdateInterval",
                "Open-Meteo에 최신 날씨를 요청하는 주기입니다. Open-Meteo는 약 15분마다 데이터를 " +
                "갱신하므로 더 짧게 잡아도 얻는 것이 거의 없습니다." },

            { "enum.UpdateInterval.FifteenMinutes", "15분" },
            { "enum.UpdateInterval.ThirtyMinutes", "30분" },
            { "enum.UpdateInterval.SixtyMinutes", "60분" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "도시" },
            { "desc.CityQuery",
                "날씨를 가져올 실제 도시입니다. 예: 리옹 - 리옹, 프랑스 - 밀라초, 이탈리아 - 뉴욕, " +
                "미국. 쉼표 뒤에 국가나 지역을 덧붙이면 범위를 좁힐 수 있습니다." },

            { "label.SearchCity", "검색" },
            { "desc.SearchCity",
                "이름을 조회해 일치하는 도시를 모두 아래에 보여 줍니다. 한 번의 추측에 의존하지 않고 " +
                "올바른 도시를 직접 확인할 수 있습니다." },

            { "label.SelectedSearchResult", "검색 결과" },
            { "desc.SelectedSearchResult",
                "검색과 일치하는 도시를 가장 근접한 것부터, 지역·국가·좌표와 함께 보여 줍니다. " +
                "하나를 고르면 바로 적용됩니다." },

            { "label.SelectedFavourite", "최근 도시" },
            { "desc.SelectedFavourite",
                "이전에 사용한 도시입니다. 고르면 조회 없이 즉시 전환됩니다." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "도시 적용" },
            { "desc.ApplyCity",
                "목록에서 고르지 않고, 위에 입력한 이름과 가장 잘 맞는 결과를 그대로 사용합니다. " +
                "도시를 찾지 못하면 이전에 확정된 위치가 그대로 유지됩니다." },

            { "label.RefreshWeatherNow", "지금 날씨 갱신" },
            { "desc.RefreshWeatherNow",
                "다음 갱신을 기다리지 않고 현재 날씨를 즉시 가져옵니다." },

            { "label.ApplyImmediately", "즉시 적용" },
            { "desc.ApplyImmediately",
                "갱신한 뒤 전환 없이 곧바로 새 날씨로 바꿉니다. 이미 진행 중인 전환도 중단합니다." },

            { "label.ResetToGameWeather", "게임 날씨로 되돌리기" },
            { "desc.ResetToGameWeather",
                "모든 기후 재정의를 해제하고 날씨 제어를 게임에 돌려줍니다. 도시를 적용하거나 갱신을 " +
                "강제하면 실제 날씨가 다시 시작됩니다." },
            { "warn.ResetToGameWeather",
                "모든 날씨 재정의를 해제하고 제어를 게임에 돌려줄까요?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "상태" },
            { "label.ResolvedLocationText", "확정된 위치" },
            { "label.LastUpdateText", "마지막 갱신" },
            { "label.CurrentWeatherText", "현재 날씨" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "안개 동기화" },
            { "desc.SyncFog",
                "보고된 안개 코드와 시정에서 안개를 이끌어 냅니다. 구름량이 안개를 만들어 내는 일은 " +
                "없습니다. 끄면 게임 자체의 안개를 그대로 둡니다." },

            { "label.SyncTemperature", "기온 동기화" },
            { "desc.SyncTemperature",
                "보이는 기온을 실제 도시에 맞춥니다. 기온은 가장 많은 게임 시스템이 다시 읽는 값입니다. " +
                "난방 수요, 유지비, 화재 위험, 관광, 지면의 눈이 여기에 해당합니다. 따라서 이 옵션을 " +
                "끄는 것이 모드의 영향을 최소로 줄이는 가장 효과적인 방법입니다. 대신 게임이 비와 눈을 " +
                "구분하지 못하게 되고, 표시되는 기온은 게임 자체의 기온이 됩니다." },

            { "label.SimulationImpactNote", "게임이 다시 읽는 값" },

            { "label.ForceSnowAppearance", "실제로 눈이 올 때 눈 표시" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II는 보이는 기온으로 비와 눈을 판단합니다. 실제 날씨가 눈인데 실제 " +
                "기온이 영상이면, 눈이 그려지도록 보이는 기온을 영하 바로 아래로 낮춥니다. 이때 표시되는 " +
                "기온은 실제 기온과 달라집니다. 계절과 날짜는 여전히 바뀌지 않습니다." },

            { "label.IgnoreModConflicts", "모드 충돌 무시" },
            { "desc.IgnoreModConflicts",
                "기본적으로 알려진 다른 날씨 모드가 불러와지면 Real Weather Sync는 스스로 꺼집니다. 같은 " +
                "기후 값을 쓰는 모드가 둘이면 서로 다투기 때문입니다. 다른 모드가 날씨를 덮어쓰지 " +
                "않는다고 확신할 때만 켜세요." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "시간 이동(시간)" },
            { "desc.TimeShiftHours",
                "다른 시각의 날씨를 읽습니다. 음수는 과거, 양수는 예보입니다. -24면 도시가 어제의 날씨를 " +
                "살고, +24면 내일의 날씨를 하루 먼저 받습니다. 움직이는 것은 날씨 관측값뿐이며 게임의 " +
                "시계와 날짜, 계절은 그대로입니다." },

            { "label.AntipodeMode", "대척점 모드" },
            { "desc.AntipodeMode",
                "선택한 도시에서 지구 정반대편에 있는 지점의 날씨를 가져옵니다. 유럽 대부분에서는 남태평양 " +
                "한가운데가 되므로, 잿빛 바다의 이슬비를 실컷 보게 될 것입니다. 도시는 고른 그대로 " +
                "남고, 반대편에서 오는 것은 날씨뿐입니다." },

            { "label.ExtremeLocation", "끔찍한 곳으로 데려가 줘" },
            { "desc.ExtremeLocation",
                "악명 높은 곳으로 곧바로 이동합니다. 집 날씨가 협조해 주기를 기다리지 않고 눈이나 안개, " +
                "폭우를 보는 가장 빠른 방법이기도 합니다. 재시작하면 '없음'으로 돌아가지만, 적용된 도시는 " +
                "다른 도시와 마찬가지로 유지됩니다." },

            { "label.OppositeDay", "거꾸로 가는 날" },
            { "desc.OppositeDay",
                "날씨를 뒤집습니다. 더위는 추위로, 맑음은 흐림으로, 건조함은 흠뻑 젖은 상태로 바뀝니다. " +
                "안개는 그대로 둡니다. 안개가 계속되면 도시가 가려져 금세 재미가 없어지기 때문입니다." },

            { "enum.ExtremeLocation.None", "있던 곳에 머무르기" },
            { "enum.ExtremeLocation.Yakutsk", "야쿠츠크 - 지구에서 가장 추운 도시" },
            { "enum.ExtremeLocation.Longyearbyen", "롱위에아르뷔엔 - 극야, 백야, 북극곰" },
            { "enum.ExtremeLocation.Ushuaia", "우수아이아 - 세상의 끝, 보면 안다" },
            { "enum.ExtremeLocation.Reykjavik", "레이캬비크 - 바람과 비, 그것도 옆으로" },
            { "enum.ExtremeLocation.MountWashington", "워싱턴산 - 안개, 그리고 미국 최악의 날씨" },
            { "enum.ExtremeLocation.DeathValley", "데스밸리 - 관측 사상 가장 더운 곳" },
            { "enum.ExtremeLocation.Cherrapunji", "체라푼지 - 지구에서 가장 비가 많은 곳 중 하나" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "정보" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "날씨 데이터 제공: Open-Meteo(open-meteo.com), CC BY 4.0 라이선스. 계정도 API 키도 필요 없습니다.\n" +
                "Open-Meteo로 전송되는 것은 입력한 도시 이름과 그로부터 확정된 좌표뿐입니다." },

            { "key.About.SimulationImpact",
                "Real Weather Sync는 게임 자체의 개발자용 날씨 도구가 쓰는 것과 같은 네 가지 기후 값을 " +
                "쓰며, 게임의 일부는 그 값을 다시 읽습니다. 냉난방 수요, 건물 유지비, 화재 위험, 여가, " +
                "관광, 지면의 눈, 표면의 젖음, 기상 이벤트가 모두 기온과 강수에 반응합니다. 게임 자체의 " +
                "날씨에 반응하는 것과 똑같습니다.\n" +
                "이 모드는 시스템을 추가하지 않고, 규칙을 바꾸지 않으며, 저장 파일에 아무것도 쓰지 " +
                "않습니다.\n" +
                "'기온 동기화'를 끄면 그 영향의 대부분이 사라지지만, 비와 눈을 구분하는 정확도를 " +
                "잃습니다. 태양광 발전과 지하수는 결코 영향을 받지 않으며, 안개는 시각적인 것 외에 " +
                "아무것에도 영향을 주지 않습니다." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "사용 안 함" },
            { "key.Status.CityNotConfigured", "도시가 설정되지 않음" },
            { "key.Status.ResolvingLocation", "위치 확인 중" },
            { "key.Status.Refreshing", "날씨 갱신 중" },
            { "key.Status.CandidatesReady", "검색 결과에서 도시를 고르세요" },
            { "key.Status.Connected", "연결됨" },
            { "key.Status.Offline", "오프라인 - 마지막 유효 날씨 사용 중" },
            { "key.Status.ErrorResolvingCity", "도시 확인 중 오류" },
            { "key.Status.IncompatibleMod", "호환되지 않는 날씨 모드가 활성" },
            { "key.Status.Released", "재정의 해제됨 - 게임 날씨 사용 중" },
            { "key.Status.WaitingForGame", "도시가 불러와지기를 기다리는 중" },

            { "key.Location.NotResolved", "아직 확정된 위치가 없습니다" },

            { "key.LastUpdate.Never", "없음" },
            { "key.LastUpdate.JustNow", "방금" },
            { "key.LastUpdate.MinutesAgo", "{0}분 전" },

            { "key.Weather.NoData", "아직 받은 날씨 데이터가 없습니다." },
            { "key.Weather.Observed", "관측값" },
            { "key.Weather.Applied", "적용값" },
            { "key.Weather.Clouds", "구름" },
            { "key.Weather.Precipitation", "강수" },
            { "key.Weather.Snow", "눈" },
            { "key.Weather.Code", "WMO 코드" },
            { "key.Weather.Visibility", "시정" },
            { "key.Weather.Fog", "안개" },
            { "key.Weather.Conditions", "날씨 상태" },
            { "key.Weather.TimeShiftPast", "{0}시간 전" },
            { "key.Weather.TimeShiftFuture", "{0}시간 후 - 예보" },
            { "key.Weather.OppositeDay", "거꾸로 가는 날" },
            { "key.Weather.Antipode", "대척점" },

            { "key.Search.NoResults", "결과 없음 - 검색을 누르세요" },
            { "key.Search.PickOne", "도시를 선택하세요..." },
            { "key.Favourites.Empty", "최근 도시가 아직 없습니다" },

            { "key.Overrides.Active", "기후 재정의가 활성 상태입니다." },
            { "key.Overrides.Inactive", "기후 재정의가 활성 상태가 아닙니다." },

            { "key.Error.CityNotFound", "일치하는 도시를 찾지 못했습니다" },
            { "key.Error.EmptyCity", "먼저 도시 이름을 입력하세요" },
            { "key.Error.Network", "Open-Meteo에 연결할 수 없습니다" },
            { "key.Error.RateLimited", "Open-Meteo가 요청을 제한하고 있습니다" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "맑음" },
            { "wmo.MainlyClear", "대체로 맑음" },
            { "wmo.PartlyCloudy", "부분적으로 흐림" },
            { "wmo.Overcast", "흐림" },
            { "wmo.Fog", "안개" },
            { "wmo.RimeFog", "상고대를 남기는 안개" },
            { "wmo.DrizzleLight", "약한 이슬비" },
            { "wmo.DrizzleModerate", "보통 이슬비" },
            { "wmo.DrizzleDense", "짙은 이슬비" },
            { "wmo.FreezingDrizzleLight", "약한 어는 이슬비" },
            { "wmo.FreezingDrizzleDense", "짙은 어는 이슬비" },
            { "wmo.RainSlight", "약한 비" },
            { "wmo.RainModerate", "보통 비" },
            { "wmo.RainHeavy", "강한 비" },
            { "wmo.FreezingRainLight", "약한 어는 비" },
            { "wmo.FreezingRainHeavy", "강한 어는 비" },
            { "wmo.SnowSlight", "약한 눈" },
            { "wmo.SnowModerate", "보통 눈" },
            { "wmo.SnowHeavy", "강한 눈" },
            { "wmo.SnowGrains", "쌀알눈" },
            { "wmo.ShowersSlight", "약한 소나기" },
            { "wmo.ShowersModerate", "보통 소나기" },
            { "wmo.ShowersViolent", "매우 강한 소나기" },
            { "wmo.SnowShowersSlight", "약한 소낙눈" },
            { "wmo.SnowShowersHeavy", "강한 소낙눈" },
            { "wmo.Thunderstorm", "뇌우" },
            { "wmo.ThunderstormHailSlight", "약한 우박을 동반한 뇌우" },
            { "wmo.ThunderstormHailHeavy", "강한 우박을 동반한 뇌우" },
            { "wmo.Unknown", "알 수 없는 날씨" }
        };
    }
}
