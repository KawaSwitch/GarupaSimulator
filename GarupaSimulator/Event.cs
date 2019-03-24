using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarupaSimulator
{
    /// <summary>
    /// イベント情報クラス
    /// </summary>
    public class Event : WpfUtil.ModelBase
    {
        #region Enum definitions

        /// <summary>
        /// イベントタイプ
        /// </summary>
        public enum HeldType
        {
            /// <summary>
            /// バッヂイベント
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.BadgeEvent), typeof(Properties.BangDreamResources))]
            Badge,

            /// <summary>
            /// チャレンジライブイベント
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.ChallengeEvent), typeof(Properties.BangDreamResources))]
            Challenge,

            /// <summary>
            /// 対バンライブイベント
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.VsEvent), typeof(Properties.BangDreamResources))]
            VS,

            /// <summary>
            /// ライブトライ！イベント
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.TryEvent), typeof(Properties.BangDreamResources))]
            Try,

            /// <summary>
            /// ミッションライブイベント
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.MissionEvent), typeof(Properties.BangDreamResources))]
            Mission,
        }

        #endregion


        /// <summary>
        /// イベント名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// イベントタイプ
        /// </summary>
        public Event.HeldType EventType { get; set; }

        /// <summary>
        /// ボーナスカードタイプ
        /// </summary>
        public Card.Type BoostType { get; set; }

        /// <summary>
        /// カードタイプボーナスの補正値[%]
        /// </summary>
        public int BoostTypeBonus { get; set; }

        /// <summary>
        /// ボーナスメンバー
        ///     後から列挙型かクラスのListにするかも
        /// </summary>
        /// <remarks>メンバーごとに補正値が異なる可能性があるのでタプルのリスト</remarks>
        public List<(string member, int bonus)> BoostMemberBonus { get; set; }

        /// <summary>
        /// カードボーナス
        /// </summary>
        public (int performance, int technique, int visual) CardBonus { get; set; }

        /// <summary>
        /// 開催期間
        /// </summary>
        /// <remarks>文字列型の簡易実装</remarks>
        public string Period { get; set; }


        #region for Binding getter

        /// <summary>
        /// ボーナスメンバーの画像表示用コレクション
        /// </summary>
        public IEnumerable<dynamic> BoostMemberImageCollection
        {
            get
            {
                return this.BoostMemberBonus.Select(b =>
                    new
                    {
                        Name = b.member,
                        Bonus = b.bonus + "%",
                        ImageUri = new Uri(@"pack://application:,,,/Resources/MiniIcons/" + b.member + ".png")
                    });
            }
        }

        /// <summary>
        /// ボーナスメンバー名（バインディング用）
        /// </summary>
        public IEnumerable<string> BoostMemberNames { get { return this.BoostMemberBonus.Select(b => b.member); } }

        /// <summary>
        /// パフォーマンス補正値
        /// </summary>
        public int PerformanceBonus { get { return this.CardBonus.performance; } }
        /// <summary>
        /// テクニック補正値
        /// </summary>
        public int TechniqueBonus { get { return this.CardBonus.technique; } }
        /// <summary>
        /// ビジュアル補正値
        /// </summary>
        public int VisualBonus { get { return this.CardBonus. visual; } }

        #endregion
    }
}
