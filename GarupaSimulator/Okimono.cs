using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarupaSimulator
{
    /// <summary>
    /// 置物情報クラス
    /// </summary>
    [Serializable()]
    public class Okimono : WpfUtil.ModelBase
    {
        /// <summary>
        /// 置物名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 置物効果の対象カードタイプ
        /// </summary>
        public List<Card.Type> TargetTypes { get; set; }

        /// <summary>
        /// 置物効果の対象バンドタイプ
        /// </summary>
        public List<Card.Band> TargetBands { get; set; }

        /// <summary>
        /// 置物の補正値 * 10[%]（各レベルごと）
        ///     小数点の誤差を回避するため10倍して整数値で格納する
        /// </summary>
        public List<(int performance, int technique, int visual)> Bonus { get; set; }

        /// <summary>
        /// 置物のレベル
        /// </summary>
        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                this.NotifyPropertyChanged(nameof(Level));
            }
        }

        /// <summary>
        /// 画像パス
        /// </summary>
        public Uri ImageUri { get { return new Uri(@"pack://application:,,,/Resources/OkimonoIcons/" + this.Name + ".png"); } }
    }

    /// <summary>
    /// エリア情報クラス
    /// </summary>
    [Serializable()]
    public class OkimonoArea
    {
        /// <summary>
        /// エリア名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// エリアアイテム
        /// </summary>
        public List<Okimono> AreaItems { get; set; }

        /// <summary>
        /// 画像パス
        /// </summary>
        public Uri ImageUri { get { return new Uri(@"pack://application:,,,/Resources/OkimonoAreaIcons/" + this.Name + ".png"); } }
    }
}
