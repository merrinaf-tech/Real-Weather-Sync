namespace RealWeatherSync.Localization.Strings
{
    /// <summary>zh-HANS. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsZhHans
    {
        public const string LocaleId = "zh-HANS";

        public static readonly string[,] Table =
        {
            { "mod.name", "真实天气同步" },

            { "tab.Main", "主要" },

            { "group.GeneralGroup", "常规" },
            { "group.SearchGroup", "城市" },
            { "group.ActionsGroup", "操作" },
            { "group.StatusGroup", "状态" },
            { "group.AdvancedGroup", "高级" },
            { "group.SillyGroup", "没人要求的选项" },
            { "group.AboutGroup", "关于" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "启用真实天气" },
            { "desc.EnableRealWeather",
                "让城市的可见天气与下方所选城市的当前真实天气保持一致。时钟、日期和季节永远不会被改变，" +
                "但游戏确实会回读这些天气数值，详见“高级”中的“游戏会回读什么”。关闭后天气立即交还给游戏。" },

            { "label.FollowGameClock", "跟随游戏内时钟" },
            { "desc.FollowGameClock",
                "不再使用单一固定的观测值，而是按照游戏内的钟点，走过所选城市最近 24 小时的真实天气。" +
                "如果你的城市是 15:00，你会得到所选城市最近一次 15:00 的真实天气，因此天气会随着一天的推移而变化。" +
                "游戏时钟只被读取，绝不被修改：时间、日期和季节完全保持游戏设定的样子。" +
                "启用时会覆盖手动时间偏移。" },

            { "label.SmoothTransitions", "平滑天气过渡" },
            { "desc.SmoothTransitions",
                "逐渐淡入每次新的天气观测值，而不是瞬间切换。淡入按现实时间计算，因此不受暂停或模拟速度影响。" },

            { "label.TransitionSeconds", "过渡时长（秒）" },
            { "desc.TransitionSeconds",
                "两次天气观测值之间淡入所需的现实秒数。仅在启用平滑过渡时生效。" },

            { "label.UpdateInterval", "更新间隔" },
            { "desc.UpdateInterval",
                "多久向 Open-Meteo 请求一次最新天气。Open-Meteo 大约每 15 分钟更新一次数据，" +
                "因此更短的间隔收益有限。" },

            { "enum.UpdateInterval.FifteenMinutes", "15 分钟" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 分钟" },
            { "enum.UpdateInterval.SixtyMinutes", "60 分钟" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "城市" },
            { "desc.CityQuery",
                "要复制天气的真实城市。例如：里昂 - 里昂，法国 - 米拉佐，意大利 - 纽约，美国。" +
                "在逗号后加上国家或地区可以缩小范围。" },

            { "label.SearchCity", "搜索" },
            { "desc.SearchCity",
                "查询该名称并在下方列出所有匹配的城市，让你确认正确的那一个，而不是只相信一次猜测。" },

            { "label.SelectedSearchResult", "搜索结果" },
            { "desc.SelectedSearchResult",
                "与搜索匹配的城市，最佳匹配在前，并附有所属地区、国家和坐标。选中后立即应用。" },

            { "label.SelectedFavourite", "最近使用的城市" },
            { "desc.SelectedFavourite",
                "你此前用过的城市。选中即可立即切换，无需重新查询。" },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "应用城市" },
            { "desc.ApplyCity",
                "直接使用上方所输入名称的最佳匹配，无需从列表中挑选。若找不到该城市，则保留此前已解析的位置。" },

            { "label.RefreshWeatherNow", "立即刷新天气" },
            { "desc.RefreshWeatherNow",
                "立刻获取当前天气，而不必等待下一次更新。" },

            { "label.ApplyImmediately", "立即应用" },
            { "desc.ApplyImmediately",
                "刷新并直接跳到新天气，跳过淡入。同时也会中断正在进行的过渡。" },

            { "label.ResetToGameWeather", "恢复游戏天气" },
            { "desc.ResetToGameWeather",
                "释放所有气候覆盖，把天气交还给游戏控制。当你应用城市或强制刷新时，真实天气会恢复。" },
            { "warn.ResetToGameWeather",
                "释放全部天气覆盖并把控制权交还给游戏？" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "状态" },
            { "label.ResolvedLocationText", "已解析的位置" },
            { "label.LastUpdateText", "上次更新" },
            { "label.CurrentWeatherText", "当前天气" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "同步雾" },
            { "desc.SyncFog",
                "根据上报的雾代码和能见度推导出雾。云量永远不会产生雾。关闭此项可保留游戏自身的雾。" },

            { "label.SyncTemperature", "同步温度" },
            { "desc.SyncTemperature",
                "用真实城市的温度驱动可见温度。温度是被最多游戏系统回读的数值 —— 供暖需求、维护费、" +
                "火灾风险、旅游、地面积雪 —— 因此关闭此项是把模组影响降到最低的最有效办法。" +
                "代价是游戏无法再区分雨和雪，而且你看到的温度是游戏自己的温度。" },

            { "label.SimulationImpactNote", "游戏会回读什么" },

            { "label.ForceSnowAppearance", "真的下雪时就显示雪" },
            { "desc.ForceSnowAppearance",
                "《城市：天际线 2》根据可见温度来判断下雨还是下雪。当真实天气是雪、但真实温度在冰点以上时，" +
                "把可见温度略微降到冰点以下，好让雪能被绘制出来。此时显示的温度会与真实温度不同。" +
                "季节和日期依然绝不会被改变。" },

            { "label.IgnoreModConflicts", "忽略模组冲突" },
            { "desc.IgnoreModConflicts",
                "默认情况下，当检测到另一个已知的天气模组时，真实天气同步会自动关闭，" +
                "因为两个模组写入相同的气候数值会互相冲突。只有在你确定另一个模组不会覆盖天气时才启用此项。" },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "时间偏移（小时）" },
            { "desc.TimeShiftHours",
                "读取另一个钟点的天气：负值表示过去，正值表示预报。设为 -24 时，你的城市过着昨天的天气；" +
                "设为 +24 时，则提前一天拿到明天的天气。只有天气观测值发生位移 —— " +
                "游戏时钟、日期和季节都不受影响。" },

            { "label.AntipodeMode", "对跖点模式" },
            { "desc.AntipodeMode",
                "取用地球上与所选城市正好相对的那一点的天气。对欧洲大部分地区来说那是南太平洋中央，" +
                "所以请做好迎接大量灰蒙蒙海上细雨的准备。你的城市仍是你选的那座 —— 只有天气来自地球另一端。" },

            { "label.ExtremeLocation", "带我去个糟糕的地方" },
            { "desc.ExtremeLocation",
                "直接跳到一个以恶劣天气闻名的地方。这也是最快看到雪、雾或暴雨的办法，" +
                "不必等家乡的天气配合。重启后会重置为“无”，但它选定的城市会像其他城市一样保留。" },

            { "label.OppositeDay", "反转日" },
            { "desc.OppositeDay",
                "把天气镜像反转。暖变冷，晴变阴，干燥变湿透。雾不参与反转，" +
                "因为永久的雾会遮住城市，而且马上就不好笑了。" },

            { "enum.ExtremeLocation.None", "留在原地" },
            { "enum.ExtremeLocation.Yakutsk", "雅库茨克 —— 地球上最冷的城市" },
            { "enum.ExtremeLocation.Longyearbyen", "朗伊尔城 —— 极夜、极昼、北极熊" },
            { "enum.ExtremeLocation.Ushuaia", "乌斯怀亚 —— 世界尽头，看得出来" },
            { "enum.ExtremeLocation.Reykjavik", "雷克雅未克 —— 风和雨，横着下" },
            { "enum.ExtremeLocation.MountWashington", "华盛顿山 —— 雾，以及全美最糟的天气" },
            { "enum.ExtremeLocation.DeathValley", "死亡谷 —— 有记录以来最热的地方" },
            { "enum.ExtremeLocation.Cherrapunji", "乞拉朋齐 —— 地球上最湿的地方之一" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "关于" },
            { "key.About.Text",
                "真实天气同步 {VERSION}\n" +
                "天气数据来自 Open-Meteo（open-meteo.com），以 CC BY 4.0 授权。无需账号，也无需 API 密钥。\n" +
                "只有你输入的城市名称和由此解析出的坐标会被发送到 Open-Meteo。" },

            { "key.About.SimulationImpact",
                "真实天气同步写入的，正是游戏自带开发者天气工具所写入的那四个气候数值，" +
                "而游戏的一部分会回读这些数值。供暖与制冷需求、建筑维护、火灾风险、休闲、旅游、" +
                "地面积雪、地表湿度和天气事件，全都会对温度和降水做出反应 —— " +
                "与它们对游戏自身天气的反应完全一样。\n" +
                "本模组不添加任何系统，不修改任何规则，也不向存档写入任何内容。\n" +
                "关闭“同步温度”可以消除其中最大的一部分影响，代价是雨雪判断的准确度。" +
                "太阳能发电和地下水永远不受影响，而雾除了视觉之外不影响任何东西。" },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "已禁用" },
            { "key.Status.CityNotConfigured", "未设置城市" },
            { "key.Status.ResolvingLocation", "正在解析位置" },
            { "key.Status.Refreshing", "正在刷新天气" },
            { "key.Status.CandidatesReady", "请从搜索结果中选择一座城市" },
            { "key.Status.Connected", "已连接" },
            { "key.Status.Offline", "离线 —— 正在使用上次有效的天气" },
            { "key.Status.ErrorResolvingCity", "解析城市时出错" },
            { "key.Status.IncompatibleMod", "检测到不兼容的天气模组" },
            { "key.Status.Released", "已释放覆盖 —— 正在使用游戏天气" },
            { "key.Status.WaitingForGame", "等待载入城市" },

            { "key.Location.NotResolved", "尚未解析出位置" },

            { "key.LastUpdate.Never", "从未" },
            { "key.LastUpdate.JustNow", "刚刚" },
            { "key.LastUpdate.MinutesAgo", "{0} 分钟前" },

            { "key.Weather.NoData", "尚未收到天气数据。" },
            { "key.Weather.Observed", "观测值" },
            { "key.Weather.Applied", "已应用" },
            { "key.Weather.Clouds", "云量" },
            { "key.Weather.Precipitation", "降水" },
            { "key.Weather.Snow", "雪" },
            { "key.Weather.Code", "WMO 代码" },
            { "key.Weather.Visibility", "能见度" },
            { "key.Weather.Fog", "雾" },
            { "key.Weather.Conditions", "天气状况" },
            { "key.Weather.TimeShiftPast", "{0} 小时前" },
            { "key.Weather.TimeShiftFuture", "{0} 小时后 —— 预报" },
            { "key.Weather.OppositeDay", "反转日" },
            { "key.Weather.Antipode", "对跖点" },

            { "key.Search.NoResults", "没有结果 —— 请点击搜索" },
            { "key.Search.PickOne", "选择一座城市…" },
            { "key.Favourites.Empty", "暂无最近使用的城市" },

            { "key.Overrides.Active", "气候覆盖已生效。" },
            { "key.Overrides.Inactive", "气候覆盖未生效。" },

            { "key.Error.CityNotFound", "未找到匹配的城市" },
            { "key.Error.EmptyCity", "请先输入城市名称" },
            { "key.Error.Network", "无法连接 Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo 正在限制请求频率" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "晴朗" },
            { "wmo.MainlyClear", "大致晴朗" },
            { "wmo.PartlyCloudy", "局部多云" },
            { "wmo.Overcast", "阴天" },
            { "wmo.Fog", "雾" },
            { "wmo.RimeFog", "雾凇" },
            { "wmo.DrizzleLight", "小毛毛雨" },
            { "wmo.DrizzleModerate", "中等毛毛雨" },
            { "wmo.DrizzleDense", "浓密毛毛雨" },
            { "wmo.FreezingDrizzleLight", "轻度冻毛毛雨" },
            { "wmo.FreezingDrizzleDense", "浓密冻毛毛雨" },
            { "wmo.RainSlight", "小雨" },
            { "wmo.RainModerate", "中雨" },
            { "wmo.RainHeavy", "大雨" },
            { "wmo.FreezingRainLight", "轻度冻雨" },
            { "wmo.FreezingRainHeavy", "强冻雨" },
            { "wmo.SnowSlight", "小雪" },
            { "wmo.SnowModerate", "中雪" },
            { "wmo.SnowHeavy", "大雪" },
            { "wmo.SnowGrains", "米雪" },
            { "wmo.ShowersSlight", "小阵雨" },
            { "wmo.ShowersModerate", "中阵雨" },
            { "wmo.ShowersViolent", "强阵雨" },
            { "wmo.SnowShowersSlight", "小阵雪" },
            { "wmo.SnowShowersHeavy", "强阵雪" },
            { "wmo.Thunderstorm", "雷暴" },
            { "wmo.ThunderstormHailSlight", "雷暴伴有小冰雹" },
            { "wmo.ThunderstormHailHeavy", "雷暴伴有大冰雹" },
            { "wmo.Unknown", "未知天气" }
        };
    }
}
