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
    internal class Card
    {
        /// <summary>
        /// カード属性
        /// </summary>
        internal enum Type
        {
            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.PowerfulType), typeof(Properties.CardInfoResources))]
            Powerful,

            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.CoolType), typeof(Properties.CardInfoResources))]
            Cool,

            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.PureType), typeof(Properties.CardInfoResources))]
            Pure,

            [Util.LocalizedDescription(nameof(Properties.CardInfoResources.HappyType), typeof(Properties.CardInfoResources))]
            Happy
        }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// カードタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// レアリティ
        /// </summary>
        public int Rarity { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Type CardType { get; set; }
    }
}
