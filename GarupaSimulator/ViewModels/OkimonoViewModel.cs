using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GarupaSimulator.WpfUtil;

namespace GarupaSimulator.ViewModels
{
    /// <summary>
    /// 置物設定ビューのビューモデル
    /// </summary>
    public class OkimonoViewModel : ViewModelBase
    {
        /// <summary>
        /// ビューで選択された置物
        /// </summary>
        /// <remarks>コードビハインドで設定</remarks>
        internal Okimono SelectedItem { get; set; } = null;


        #region ctor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal OkimonoViewModel(List<OkimonoArea> areas)
        {
            this.Areas = new ObservableCollection<OkimonoArea>(areas);
        }

        /// <summary>
        /// デザイナー用 使用禁止
        /// </summary>
        [Obsolete("Designer Only", true)]
        public OkimonoViewModel()
        {
            this.Areas = new ObservableCollection<OkimonoArea>()
            {
                new OkimonoArea { Name = "江戸川楽器店", AreaItems = new List<Okimono> { new Okimono { Name = "ポピパのポスター", LocationName = "居酒屋",
                   Bonus = new List<(int performance, int technique, int visual)>() { (20, 10, 30), (30, 20, 40), (100, 200, 300) } } } },
                new OkimonoArea { Name = "CiRCLE" },
                new OkimonoArea { Name = "流星堂" },
                new OkimonoArea { Name = "カフェテリア" },
            };
        }

        #endregion

        #region バインディング用フィールド

        

        /// <summary>
        /// 置物エリア情報
        /// </summary>
        private ObservableCollection<OkimonoArea> _areas;

        /// <summary>
        /// 置物エリア情報 変更通知用プロパティ
        /// </summary>
        public ObservableCollection<OkimonoArea> Areas
        {
            get
            {
                return _areas;
            }
            set
            {
                _areas = value;
                this.NotifyPropertyChanged(nameof(Areas));
            }
        }

        #endregion
    }
}
