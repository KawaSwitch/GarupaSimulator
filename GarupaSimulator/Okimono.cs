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
        /// 設置場所
        /// </summary>
        public string LocationName { get; set; }

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
                this.NotifyPropertyChanged(nameof(BonusMessage)); // メッセージ更新
            }
        }

        /// <summary>
        /// 画像パス
        /// </summary>
        public Uri ImageUri { get { return new Uri(@"pack://application:,,,/Resources/OkimonoIcons/" + this.Name + ".png"); } }

        #region for Binding getter

        /// <summary>
        /// 置物の可能レベル（バインディング用）
        /// </summary>
        public IEnumerable<int> Levels { get { return Enumerable.Range(0, this.Bonus.Count); } }

        /// <summary>
        /// 置物メッセージ（バインディング用）
        /// </summary>
        public string BonusMessage { get { return this.CreateBonusMessage(); } }

        #endregion

        #region Private Helper

        /// <summary>
        /// 置物の補正値メッセージを作成する
        /// </summary>
        private string CreateBonusMessage()
        {
            string msg = "";

            foreach (var targetType in this.TargetTypes)
                msg += Util.EnumUtil.GetDescription(targetType) + " ";
            foreach (var targetBand in this.TargetBands)
                msg += Util.EnumUtil.GetDescription(targetBand) + " ";

            msg += "の";

            // 簡易 あとでパラメータ補正がばらばらな置物が追加されたら変更する
            msg += "全パラメータ" + this.Bonus[this.Level].performance / 10 + "% UP";

            return msg;
        }

        #endregion
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
