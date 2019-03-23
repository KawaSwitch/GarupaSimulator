using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
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

        #region コマンド

        private ICommand setAllLevelMax;
        public ICommand SetAllLevelMax => setAllLevelMax ?? (setAllLevelMax = new DelegateCommand(SetItemsLevelMax, null));

        private ICommand setAllLevelMin;
        public ICommand SetAllLevelMin => setAllLevelMin ?? (setAllLevelMin = new DelegateCommand(SetItemsLevelMin, null));

        /// <summary>
        /// 指定されたすべてのエリアアイテムのレベルを最大にする
        /// </summary>
        /// <param name="areaItems">エリアアイテム</param>
        private void SetItemsLevelMax(object areaItems)
        {
            // NOTE: キャスト方法が分からなかったので動的に取得
            dynamic _areaItems = areaItems;

            foreach (var areaItem in _areaItems)
            {
                try
                {
                    areaItem.Level = (areaItem as Okimono).Bonus.Count - 1;
                }
                catch
                {
                    throw; // とりあえず放置
                }
            }
        }

        /// <summary>
        /// 指定されたすべてのエリアアイテムのレベルを最小(0)にする
        /// </summary>
        /// <param name="areaItems">エリアアイテム</param>
        private void SetItemsLevelMin(object areaItems)
        {
            dynamic _areaItems = areaItems;

            foreach (var areaItem in _areaItems)
            {
                try
                {
                    areaItem.Level = 0;
                }
                catch
                {
                    throw; // とりあえず放置
                }
            }
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
