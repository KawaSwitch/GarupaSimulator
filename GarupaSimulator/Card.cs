using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarupaSimulator
{
    /// <summary>
    /// 各種カード情報
    /// </summary>
    [Serializable()]
    internal class Card
    {
        #region Enum definitions

        /// <summary>
        /// カード属性
        /// </summary>
        internal enum Type
        {
            /// <summary>
            /// パワフル
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.PowerfulType), typeof(Properties.CardInfoResources))]
            Powerful,

            /// <summary>
            /// クール
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.CoolType), typeof(Properties.CardInfoResources))]
            Cool,

            /// <summary>
            /// ピュア
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.PureType), typeof(Properties.CardInfoResources))]
            Pure,

            /// <summary>
            /// ハッピー
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.HappyType), typeof(Properties.CardInfoResources))]
            Happy
        }

        /// <summary>
        /// 所属バンド
        /// </summary>
        internal enum Band
        {
            /// <summary>
            /// Poppin'Party
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.PoppinParty), typeof(Properties.BangDreamResources))]
            PoppinParty,

            /// <summary>
            /// Afterglow
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.Afterglow), typeof(Properties.BangDreamResources))]
            Afterglow,

            /// <summary>
            /// Pastel * Palettes
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.PastelPalettes), typeof(Properties.BangDreamResources))]
            PastelPalettes,

            /// <summary>
            /// Roselia
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.Roselia), typeof(Properties.BangDreamResources))]
            Roselia,

            /// <summary>
            /// ハロー、ハッピーワールド！
            /// </summary>
            [Util.LocalizedDescription(nameof(Properties.BangDreamResources.HelloHappyWorld), typeof(Properties.BangDreamResources))]
            HelloHappyWorld,
        }

        #endregion

        #region Field definitions

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// カードタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 所属バンド
        /// </summary>
        public Band BandName { get; set; }

        /// <summary>
        /// レアリティ
        /// </summary>
        public int Rarity { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Type CardType { get; set; }

        /// <summary>
        /// 最大総合力
        /// </summary>
        public int MaxTotal { get { return MaxPerformance + MaxTechnique + MaxVisual; } }

        /// <summary>
        /// 最大パフォーマンス値
        /// </summary>
        public int MaxPerformance { get; set; }
        /// <summary>
        /// 最大テクニック値
        /// </summary>
        public int MaxTechnique { get; set; }
        /// <summary>
        /// 最大ビジュアル値
        /// </summary>
        public int MaxVisual { get; set; }

        /// <summary>
        /// スキル名
        /// </summary>
        public string SkillName { get; set; }
        /// <summary>
        /// スキルの効果
        /// </summary>
        public string SkillEffect { get; set; } // とりあえず文章

        /// <summary>
        /// 特訓前 アイコン画像のパス
        /// </summary>
        public string IconBeforePath { get; set; }
        /// <summary>
        /// 特訓後 アイコン画像のパス
        /// </summary>
        public string IconAfterPath { get; set; }

        /// <summary>
        /// 特訓前 全体画像のパス
        /// </summary>
        public string ImageBeforePath { get; set; }
        /// <summary>
        /// 特訓後 全体画像のパス
        /// </summary>
        public string ImageAfterPath { get; set; }

        #endregion
    }
}
