namespace RealWeatherSync.Localization.Strings
{
    /// <summary>ja-JP. Slot set must match <see cref="StringsEn"/> exactly.</summary>
    public static class StringsJa
    {
        public const string LocaleId = "ja-JP";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "メイン" },

            { "group.GeneralGroup", "全般" },
            { "group.SearchGroup", "都市" },
            { "group.ActionsGroup", "操作" },
            { "group.StatusGroup", "状態" },
            { "group.AdvancedGroup", "詳細設定" },
            { "group.SillyGroup", "誰も頼んでいないオプション" },
            { "group.AboutGroup", "このMODについて" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "実際の天気を有効にする" },
            { "desc.EnableRealWeather",
                "都市の見た目の天気を、下で選んだ都市の現在の実際の天気に合わせます。時計・日付・季節が" +
                "変更されることは一切ありませんが、ゲーム側は天気の値を読み返します。詳細設定の" +
                "「ゲームが読み返すもの」を参照してください。オフにすると天気はただちにゲームへ戻ります。" },

            { "label.FollowGameClock", "ゲーム内時計に追従する" },
            { "desc.FollowGameClock",
                "1つの固定した観測値を使うのではなく、ゲーム内の時刻に合わせて、選んだ都市の直近24時間の" +
                "実際の天気をたどります。あなたの都市が15:00なら、選んだ都市の直近の15:00の実際の天気に" +
                "なります。つまり一日の経過とともに天気が変わります。ゲーム内時計は読み取るだけで、" +
                "変更することはありません。時刻・日付・季節はゲームが設定したままです。" +
                "オンの間は手動の時間シフトより優先されます。" },

            { "label.SmoothTransitions", "天気のなめらかな遷移" },
            { "desc.SmoothTransitions",
                "新しい観測値へ瞬時に切り替えるのではなく、徐々に変化させます。遷移は実時間で進むため、" +
                "一時停止やシミュレーション速度の影響を受けません。" },

            { "label.TransitionSeconds", "遷移の長さ（秒）" },
            { "desc.TransitionSeconds",
                "2つの観測値のあいだの遷移にかかる実時間の秒数です。なめらかな遷移が有効なときだけ使われます。" },

            { "label.UpdateInterval", "更新間隔" },
            { "desc.UpdateInterval",
                "Open-Meteo に最新の気象状況を問い合わせる頻度です。Open-Meteo のデータ更新はおよそ15分ごと" +
                "なので、これより短い間隔にしてもほとんど意味はありません。" },

            { "enum.UpdateInterval.FifteenMinutes", "15分" },
            { "enum.UpdateInterval.ThirtyMinutes", "30分" },
            { "enum.UpdateInterval.SixtyMinutes", "60分" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "都市" },
            { "desc.CityQuery",
                "天気の取得元となる実在の都市です。例：リヨン - リヨン、フランス - ミラッツォ、イタリア - " +
                "ニューヨーク、アメリカ合衆国。カンマのあとに国名や地域名を足すと絞り込めます。" },

            { "label.SearchCity", "検索" },
            { "desc.SearchCity",
                "名前を検索し、一致する都市をすべて下に表示します。1つの推測に頼らず、正しい都市を確認できます。" },

            { "label.SelectedSearchResult", "検索結果" },
            { "desc.SelectedSearchResult",
                "検索に一致した都市を、最も近いものから順に、地域・国・座標つきで表示します。選ぶとすぐに適用されます。" },

            { "label.SelectedFavourite", "最近使った都市" },
            { "desc.SelectedFavourite",
                "以前に使った都市です。選ぶと検索なしで即座に切り替わります。" },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "都市を適用" },
            { "desc.ApplyCity",
                "リストから選ばずに、上で入力した名前に最も近い候補をそのまま使います。" +
                "都市が見つからない場合は、直前に確定した場所がそのまま保持されます。" },

            { "label.RefreshWeatherNow", "今すぐ天気を更新" },
            { "desc.RefreshWeatherNow",
                "次の更新を待たずに、現在の気象状況をすぐ取得します。" },

            { "label.ApplyImmediately", "即座に適用" },
            { "desc.ApplyImmediately",
                "更新して、遷移を飛ばし新しい天気へ直接切り替えます。進行中の遷移も打ち切ります。" },

            { "label.ResetToGameWeather", "ゲームの天気に戻す" },
            { "desc.ResetToGameWeather",
                "気候のオーバーライドをすべて解放し、天気の制御をゲームに返します。" +
                "都市を適用するか手動で更新すると、実際の天気が再開します。" },
            { "warn.ResetToGameWeather",
                "天気のオーバーライドをすべて解放し、制御をゲームに戻しますか？" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "状態" },
            { "label.ResolvedLocationText", "確定した場所" },
            { "label.LastUpdateText", "最終更新" },
            { "label.CurrentWeatherText", "現在の天気" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "霧を同期する" },
            { "desc.SyncFog",
                "報告された霧のコードと視程から霧を求めます。雲量から霧が生じることはありません。" +
                "オフにすると、ゲーム自身の霧はそのまま残ります。" },

            { "label.SyncTemperature", "気温を同期する" },
            { "desc.SyncTemperature",
                "見た目の気温を実際の都市に合わせます。気温は最も多くのゲームシステムが読み返す値です" +
                "（暖房需要、維持費、火災リスク、観光、地面の積雪）。したがってこれをオフにすることが、" +
                "MODの影響を最小限に抑える最も効果的な方法です。代償として、ゲームは雨と雪を区別できなくなり、" +
                "表示される気温はゲーム自身のものになります。" },

            { "label.SimulationImpactNote", "ゲームが読み返すもの" },

            { "label.ForceSnowAppearance", "実際に雪のときは雪を表示する" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II は見た目の気温から雨か雪かを判断します。実際の天気が雪でも実際の気温が" +
                "氷点より高い場合、雪が描画されるように見た目の気温を氷点のすぐ下まで下げます。" +
                "そのため表示される気温は実際の気温と異なります。季節と日付は変更されません。" },

            { "label.IgnoreModConflicts", "MODの競合を無視する" },
            { "desc.IgnoreModConflicts",
                "既定では、既知の別の天気MODが読み込まれていると Real Weather Sync は自動的に停止します。" +
                "同じ気候の値を書き込むMODが2つあると奪い合いになるためです。" +
                "相手のMODが天気を上書きしないと確信できる場合にのみ有効にしてください。" },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "時間シフト（時間）" },
            { "desc.TimeShiftHours",
                "別の時刻の天気を読み込みます。負の値は過去、正の値は予報です。-24 ならあなたの都市は昨日の" +
                "天気で暮らし、+24 なら明日の天気を1日早く受け取ります。動くのは天気の読み取りだけで、" +
                "ゲームの時計・日付・季節には触れません。" },

            { "label.AntipodeMode", "対蹠地モード" },
            { "desc.AntipodeMode",
                "選んだ都市のちょうど地球の裏側にあたる地点の天気を使います。ヨーロッパの大半では南太平洋の" +
                "真ん中になるので、灰色の海上の霧雨がたっぷり続くと思ってください。都市はあなたが選んだ" +
                "ままで、裏側から来るのは天気だけです。" },

            { "label.ExtremeLocation", "どこかひどい場所へ連れて行って" },
            { "desc.ExtremeLocation",
                "悪天候で名高い場所へ一足飛びに移動します。地元の天気の機嫌を待たずに雪・霧・豪雨を見る" +
                "いちばん早い方法でもあります。再起動すると「なし」に戻りますが、適用された都市は" +
                "他と同じように保持されます。" },

            { "label.OppositeDay", "あべこべの日" },
            { "desc.OppositeDay",
                "天気を反転させます。暑さは寒さに、快晴は曇天に、乾燥はずぶ濡れに。霧はそのままです。" +
                "霧が延々と続くと都市が見えなくなり、すぐに面白くなくなるからです。" },

            { "enum.ExtremeLocation.None", "そのままの場所にいる" },
            { "enum.ExtremeLocation.Yakutsk", "ヤクーツク - 地球上でもっとも寒い都市" },
            { "enum.ExtremeLocation.Longyearbyen", "ロングイェールビーン - 極夜、白夜、ホッキョクグマ" },
            { "enum.ExtremeLocation.Ushuaia", "ウシュアイア - 世界の果て、見ればわかる" },
            { "enum.ExtremeLocation.Reykjavik", "レイキャビク - 風と雨、それも横殴り" },
            { "enum.ExtremeLocation.MountWashington", "ワシントン山 - 霧、そしてアメリカ最悪の天気" },
            { "enum.ExtremeLocation.DeathValley", "デスバレー - 観測史上もっとも暑い場所" },
            { "enum.ExtremeLocation.Cherrapunji", "チェラプンジ - 地球でもっとも雨の多い場所のひとつ" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "このMODについて" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "気象データ提供：Open-Meteo（open-meteo.com）、CC BY 4.0 ライセンス。アカウントもAPIキーも不要です。\n" +
                "Open-Meteo に送信されるのは、入力した都市名と、そこから求めた座標だけです。" },

            { "key.About.SimulationImpact",
                "Real Weather Sync が書き込むのは、ゲーム自身の開発用天気ツールが書き込むのと同じ4つの気候値で、" +
                "ゲームの一部はその値を読み返します。冷暖房需要、建物の維持費、火災リスク、レジャー、観光、" +
                "地面の積雪、路面の濡れ、気象イベントはいずれも気温と降水に反応します。" +
                "ゲーム自身の天気に反応するのとまったく同じです。\n" +
                "このMODはシステムを追加せず、ルールを変更せず、セーブデータには何も書き込みません。\n" +
                "「気温を同期する」をオフにすると影響の大部分がなくなりますが、雨と雪の判別精度は落ちます。" +
                "太陽光発電と地下水は一切影響を受けず、霧は見た目以外に何の影響も与えません。" },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "無効" },
            { "key.Status.CityNotConfigured", "都市が未設定" },
            { "key.Status.ResolvingLocation", "場所を特定中" },
            { "key.Status.Refreshing", "天気を更新中" },
            { "key.Status.CandidatesReady", "検索結果から都市を選んでください" },
            { "key.Status.Connected", "接続済み" },
            { "key.Status.Offline", "オフライン - 直近の有効な天気を使用中" },
            { "key.Status.ErrorResolvingCity", "都市の特定に失敗しました" },
            { "key.Status.IncompatibleMod", "非互換の天気MODが有効です" },
            { "key.Status.Released", "オーバーライドを解放 - ゲームの天気を使用中" },
            { "key.Status.WaitingForGame", "都市の読み込みを待機中" },

            { "key.Location.NotResolved", "場所はまだ特定されていません" },

            { "key.LastUpdate.Never", "なし" },
            { "key.LastUpdate.JustNow", "たった今" },
            { "key.LastUpdate.MinutesAgo", "{0}分前" },

            { "key.Weather.NoData", "気象データをまだ受信していません。" },
            { "key.Weather.Observed", "観測値" },
            { "key.Weather.Applied", "適用値" },
            { "key.Weather.Clouds", "雲量" },
            { "key.Weather.Precipitation", "降水" },
            { "key.Weather.Snow", "雪" },
            { "key.Weather.Code", "WMOコード" },
            { "key.Weather.Visibility", "視程" },
            { "key.Weather.Fog", "霧" },
            { "key.Weather.Conditions", "気象状況" },
            { "key.Weather.TimeShiftPast", "{0}時間前" },
            { "key.Weather.TimeShiftFuture", "{0}時間先 - 予報" },
            { "key.Weather.OppositeDay", "あべこべの日" },
            { "key.Weather.Antipode", "対蹠地" },

            { "key.Search.NoResults", "結果なし - 「検索」を押してください" },
            { "key.Search.PickOne", "都市を選択..." },
            { "key.Favourites.Empty", "最近使った都市はまだありません" },

            { "key.Overrides.Active", "気候のオーバーライドは有効です。" },
            { "key.Overrides.Inactive", "気候のオーバーライドは無効です。" },

            { "key.Error.CityNotFound", "一致する都市が見つかりません" },
            { "key.Error.EmptyCity", "先に都市名を入力してください" },
            { "key.Error.Network", "Open-Meteo に接続できません" },
            { "key.Error.RateLimited", "Open-Meteo がリクエストを制限しています" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "快晴" },
            { "wmo.MainlyClear", "おおむね晴れ" },
            { "wmo.PartlyCloudy", "所により曇り" },
            { "wmo.Overcast", "曇天" },
            { "wmo.Fog", "霧" },
            { "wmo.RimeFog", "着氷性の霧" },
            { "wmo.DrizzleLight", "弱い霧雨" },
            { "wmo.DrizzleModerate", "並の霧雨" },
            { "wmo.DrizzleDense", "強い霧雨" },
            { "wmo.FreezingDrizzleLight", "弱い着氷性の霧雨" },
            { "wmo.FreezingDrizzleDense", "強い着氷性の霧雨" },
            { "wmo.RainSlight", "弱い雨" },
            { "wmo.RainModerate", "並の雨" },
            { "wmo.RainHeavy", "強い雨" },
            { "wmo.FreezingRainLight", "弱い着氷性の雨" },
            { "wmo.FreezingRainHeavy", "強い着氷性の雨" },
            { "wmo.SnowSlight", "弱い雪" },
            { "wmo.SnowModerate", "並の雪" },
            { "wmo.SnowHeavy", "強い雪" },
            { "wmo.SnowGrains", "細氷（雪あられ）" },
            { "wmo.ShowersSlight", "弱いにわか雨" },
            { "wmo.ShowersModerate", "並のにわか雨" },
            { "wmo.ShowersViolent", "激しいにわか雨" },
            { "wmo.SnowShowersSlight", "弱いにわか雪" },
            { "wmo.SnowShowersHeavy", "強いにわか雪" },
            { "wmo.Thunderstorm", "雷雨" },
            { "wmo.ThunderstormHailSlight", "弱いひょうを伴う雷雨" },
            { "wmo.ThunderstormHailHeavy", "強いひょうを伴う雷雨" },
            { "wmo.Unknown", "不明な気象状況" }
        };
    }
}
