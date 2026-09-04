namespace RealWeatherSync.Localization.Strings
{
    /// <summary>
    /// zh-HANT. Slot set must match <see cref="StringsEn"/> exactly.
    ///
    /// Not a character conversion of <see cref="StringsZhHans"/>: the wording follows
    /// Taiwan/Hong Kong usage, which differs from mainland usage in more than the glyphs
    /// (the game's own title, weather terminology and several UI verbs are not the same).
    /// </summary>
    public static class StringsZhHant
    {
        public const string LocaleId = "zh-HANT";

        public static readonly string[,] Table =
        {
            { "mod.name", "真實天氣同步" },

            { "tab.Main", "主要" },

            { "group.GeneralGroup", "一般" },
            { "group.SearchGroup", "城市" },
            { "group.ActionsGroup", "動作" },
            { "group.StatusGroup", "狀態" },
            { "group.AdvancedGroup", "進階" },
            { "group.SillyGroup", "沒有人要求的選項" },
            { "group.AboutGroup", "關於" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "啟用真實天氣" },
            { "desc.EnableRealWeather",
                "讓城市看得見的天氣，與下方所選城市當前的真實天氣一致。時鐘、日期與季節永遠不會被更動，" +
                "但遊戲確實會回頭讀取這些天氣數值，詳見「進階」中的「遊戲會讀回什麼」。" +
                "關閉後，天氣立刻交還給遊戲。" },

            { "label.FollowGameClock", "跟隨遊戲內時鐘" },
            { "desc.FollowGameClock",
                "不使用單一固定的觀測值，而是依照遊戲內的時刻，走過所選城市最近 24 小時的真實天氣。" +
                "若你的城市是 15:00，你會得到所選城市最近一次 15:00 的真實天氣，因此天氣會隨著一天推移而改變。" +
                "遊戲時鐘只被讀取，絕不會被更動：時間、日期與季節完全維持遊戲設定的樣子。" +
                "啟用期間會蓋過手動時間位移。" },

            { "label.SmoothTransitions", "平滑的天氣轉場" },
            { "desc.SmoothTransitions",
                "逐漸淡入每一次新的天氣觀測值，而不是瞬間切換。淡入以真實時間計算，" +
                "因此不受暫停或模擬速度影響。" },

            { "label.TransitionSeconds", "轉場長度（秒）" },
            { "desc.TransitionSeconds",
                "兩次天氣觀測值之間淡入所需的真實秒數。僅在啟用平滑轉場時生效。" },

            { "label.UpdateInterval", "更新間隔" },
            { "desc.UpdateInterval",
                "多久向 Open-Meteo 索取一次最新天氣。Open-Meteo 大約每 15 分鐘更新一次資料，" +
                "因此更短的間隔幫助有限。" },

            { "enum.UpdateInterval.FifteenMinutes", "15 分鐘" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 分鐘" },
            { "enum.UpdateInterval.SixtyMinutes", "60 分鐘" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "城市" },
            { "desc.CityQuery",
                "要複製天氣的真實城市。例如：里昂 - 里昂，法國 - 米拉佐，義大利 - 紐約，美國。" +
                "在逗號後加上國家或地區可以縮小範圍。" },

            { "label.SearchCity", "搜尋" },
            { "desc.SearchCity",
                "查詢該名稱並在下方列出所有符合的城市，讓你確認正確的那一個，而不是只憑一次猜測。" },

            { "label.SelectedSearchResult", "搜尋結果" },
            { "desc.SelectedSearchResult",
                "符合搜尋的城市，最相符的排在前面，並附上所屬地區、國家與座標。選取後立即套用。" },

            { "label.SelectedFavourite", "最近使用的城市" },
            { "desc.SelectedFavourite",
                "你先前用過的城市。選取即可立即切換，無須重新查詢。" },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "套用城市" },
            { "desc.ApplyCity",
                "直接採用上方所輸入名稱的最佳結果，不必從清單挑選。若找不到該城市，則保留先前已確定的位置。" },

            { "label.RefreshWeatherNow", "立即重新整理天氣" },
            { "desc.RefreshWeatherNow",
                "立刻取得目前的天氣，不必等待下一次更新。" },

            { "label.ApplyImmediately", "立即套用" },
            { "desc.ApplyImmediately",
                "重新整理並直接跳到新的天氣，略過淡入。同時也會中止正在進行的轉場。" },

            { "label.ResetToGameWeather", "還原為遊戲天氣" },
            { "desc.ResetToGameWeather",
                "釋放所有氣候覆寫，把天氣交還給遊戲控制。當你套用城市或強制重新整理時，真實天氣會恢復。" },
            { "warn.ResetToGameWeather",
                "要釋放全部天氣覆寫，並把控制權交還給遊戲嗎？" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "狀態" },
            { "label.ResolvedLocationText", "已確定的位置" },
            { "label.LastUpdateText", "上次更新" },
            { "label.CurrentWeatherText", "目前天氣" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "同步霧" },
            { "desc.SyncFog",
                "依據回報的霧代碼與能見度推導出霧。雲量永遠不會產生霧。關閉此項可保留遊戲本身的霧。" },

            { "label.SyncTemperature", "同步氣溫" },
            { "desc.SyncTemperature",
                "以真實城市的氣溫驅動看得見的氣溫。氣溫是被最多遊戲系統讀回的數值 —— 暖氣需求、維護費、" +
                "火災風險、觀光、地面積雪 —— 因此關閉此項是把模組影響降到最低最有效的方式。" +
                "代價是遊戲無法再分辨雨和雪，而且你看到的氣溫是遊戲自己的氣溫。" },

            { "label.SimulationImpactNote", "遊戲會讀回什麼" },

            { "label.ForceSnowAppearance", "真的下雪時就顯示雪" },
            { "desc.ForceSnowAppearance",
                "《都市：天際線 2》會依看得見的氣溫判斷下雨還是下雪。當真實天氣是雪、但真實氣溫在冰點以上時，" +
                "會把看得見的氣溫略微壓到冰點以下，好讓雪能被繪製出來。此時顯示的氣溫會與真實氣溫不同。" +
                "季節與日期依然絕不會被更動。" },

            { "label.IgnoreModConflicts", "忽略模組衝突" },
            { "desc.IgnoreModConflicts",
                "預設情況下，只要偵測到另一個已知的天氣模組，真實天氣同步就會自動關閉，" +
                "因為兩個模組寫入相同的氣候數值會互相搶奪。只有在你確定另一個模組不會覆寫天氣時才啟用此項。" },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "時間位移（小時）" },
            { "desc.TimeShiftHours",
                "讀取另一個時刻的天氣：負值代表過去，正值代表預報。設為 -24 時，你的城市過著昨天的天氣；" +
                "設為 +24 時，則提早一天拿到明天的天氣。位移的只有天氣讀數 —— " +
                "遊戲時鐘、日期與季節都不受影響。" },

            { "label.AntipodeMode", "對蹠點模式" },
            { "desc.AntipodeMode",
                "取用地球上與所選城市正好相對那一點的天氣。對歐洲大部分地區而言那是南太平洋正中央，" +
                "所以請準備好迎接大量灰濛濛的海上毛毛雨。你的城市仍是你挑的那一座 —— 只有天氣來自地球另一端。" },

            { "label.ExtremeLocation", "帶我去個糟糕的地方" },
            { "desc.ExtremeLocation",
                "直接跳到一個以惡劣天氣聞名的地方。這也是最快看到雪、霧或傾盆大雨的辦法，" +
                "不必等家鄉的天氣配合。重新啟動後會重設為「無」，但它挑選的城市會像其他城市一樣保留下來。" },

            { "label.OppositeDay", "顛倒日" },
            { "desc.OppositeDay",
                "把天氣鏡射反轉。暖變冷，晴變陰，乾燥變濕透。霧不參與反轉，" +
                "因為永久的霧會遮住城市，而且馬上就不好笑了。" },

            { "enum.ExtremeLocation.None", "留在原地" },
            { "enum.ExtremeLocation.Yakutsk", "雅庫次克 —— 地球上最冷的城市" },
            { "enum.ExtremeLocation.Longyearbyen", "朗伊爾城 —— 極夜、極晝、北極熊" },
            { "enum.ExtremeLocation.Ushuaia", "烏斯懷亞 —— 世界盡頭，而且看得出來" },
            { "enum.ExtremeLocation.Reykjavik", "雷克雅維克 —— 風和雨，橫著下" },
            { "enum.ExtremeLocation.MountWashington", "華盛頓山 —— 霧，以及全美最糟的天氣" },
            { "enum.ExtremeLocation.DeathValley", "死亡谷 —— 有紀錄以來最熱的地方" },
            { "enum.ExtremeLocation.Cherrapunji", "乞拉朋吉 —— 地球上最潮濕的地方之一" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "關於" },
            { "key.About.Text",
                "真實天氣同步 {VERSION}\n" +
                "天氣資料由 Open-Meteo（open-meteo.com）提供，採 CC BY 4.0 授權。不需要帳號，也不需要 API 金鑰。\n" +
                "只有你輸入的城市名稱，以及由此確定的座標會被送往 Open-Meteo。" },

            { "key.About.SimulationImpact",
                "真實天氣同步寫入的，正是遊戲內建開發者天氣工具所寫入的那四個氣候數值，" +
                "而遊戲的一部分會把這些數值讀回去。暖氣與冷氣需求、建築維護、火災風險、休閒、觀光、" +
                "地面積雪、地表濕度與天氣事件，全都會對氣溫和降水產生反應 —— " +
                "跟它們對遊戲本身天氣的反應完全相同。\n" +
                "本模組不新增任何系統，不更動任何規則，也不會在存檔中寫入任何東西。\n" +
                "關閉「同步氣溫」可以消除其中最大的一部分影響，代價是雨雪判斷的準確度。" +
                "太陽能發電與地下水永遠不受影響，而霧除了畫面之外不影響任何東西。" },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "已停用" },
            { "key.Status.CityNotConfigured", "尚未設定城市" },
            { "key.Status.ResolvingLocation", "正在確定位置" },
            { "key.Status.Refreshing", "正在重新整理天氣" },
            { "key.Status.CandidatesReady", "請從搜尋結果中選擇一座城市" },
            { "key.Status.Connected", "已連線" },
            { "key.Status.Offline", "離線 —— 正在使用上次有效的天氣" },
            { "key.Status.ErrorResolvingCity", "確定城市時發生錯誤" },
            { "key.Status.IncompatibleMod", "偵測到不相容的天氣模組" },
            { "key.Status.Released", "已釋放覆寫 —— 正在使用遊戲天氣" },
            { "key.Status.WaitingForGame", "等待載入城市" },

            { "key.Location.NotResolved", "尚未確定任何位置" },

            { "key.LastUpdate.Never", "從未" },
            { "key.LastUpdate.JustNow", "剛剛" },
            { "key.LastUpdate.MinutesAgo", "{0} 分鐘前" },

            { "key.Weather.NoData", "尚未收到天氣資料。" },
            { "key.Weather.Observed", "觀測值" },
            { "key.Weather.Applied", "已套用" },
            { "key.Weather.Clouds", "雲量" },
            { "key.Weather.Precipitation", "降水" },
            { "key.Weather.Snow", "雪" },
            { "key.Weather.Code", "WMO 代碼" },
            { "key.Weather.Visibility", "能見度" },
            { "key.Weather.Fog", "霧" },
            { "key.Weather.Conditions", "天氣狀況" },
            { "key.Weather.TimeShiftPast", "{0} 小時前" },
            { "key.Weather.TimeShiftFuture", "{0} 小時後 —— 預報" },
            { "key.Weather.OppositeDay", "顛倒日" },
            { "key.Weather.Antipode", "對蹠點" },

            { "key.Search.NoResults", "沒有結果 —— 請按下搜尋" },
            { "key.Search.PickOne", "選擇一座城市…" },
            { "key.Favourites.Empty", "尚無最近使用的城市" },

            { "key.Overrides.Active", "氣候覆寫已生效。" },
            { "key.Overrides.Inactive", "氣候覆寫未生效。" },

            { "key.Error.CityNotFound", "找不到符合的城市" },
            { "key.Error.EmptyCity", "請先輸入城市名稱" },
            { "key.Error.Network", "無法連線至 Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo 正在限制請求頻率" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "晴朗" },
            { "wmo.MainlyClear", "大致晴朗" },
            { "wmo.PartlyCloudy", "多雲時晴" },
            { "wmo.Overcast", "陰天" },
            { "wmo.Fog", "霧" },
            { "wmo.RimeFog", "霧淞" },
            { "wmo.DrizzleLight", "小毛毛雨" },
            { "wmo.DrizzleModerate", "中等毛毛雨" },
            { "wmo.DrizzleDense", "濃密毛毛雨" },
            { "wmo.FreezingDrizzleLight", "輕度凍毛毛雨" },
            { "wmo.FreezingDrizzleDense", "濃密凍毛毛雨" },
            { "wmo.RainSlight", "小雨" },
            { "wmo.RainModerate", "中雨" },
            { "wmo.RainHeavy", "大雨" },
            { "wmo.FreezingRainLight", "輕度凍雨" },
            { "wmo.FreezingRainHeavy", "強烈凍雨" },
            { "wmo.SnowSlight", "小雪" },
            { "wmo.SnowModerate", "中雪" },
            { "wmo.SnowHeavy", "大雪" },
            { "wmo.SnowGrains", "米雪" },
            { "wmo.ShowersSlight", "小陣雨" },
            { "wmo.ShowersModerate", "中陣雨" },
            { "wmo.ShowersViolent", "強烈陣雨" },
            { "wmo.SnowShowersSlight", "小陣雪" },
            { "wmo.SnowShowersHeavy", "強烈陣雪" },
            { "wmo.Thunderstorm", "雷雨" },
            { "wmo.ThunderstormHailSlight", "雷雨伴隨小冰雹" },
            { "wmo.ThunderstormHailHeavy", "雷雨伴隨大冰雹" },
            { "wmo.Unknown", "未知天氣" }
        };
    }
}
