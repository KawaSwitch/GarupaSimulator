using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace GarupaSimulator.ViewModels
{
    /// <summary>
    /// メインウィンドウのビューモデル
    /// </summary>
    internal class MainViewModel : WpfUtil.ViewModelBase
    {
        private Views.MainWindow _mainWnd;

        internal MainViewModel(Views.MainWindow wnd)
        {
            _mainWnd = wnd;
        }

        /// <summary>
        /// デザイナー用 使用禁止
        /// </summary>
        [Obsolete("Designer Only", true)]
        public MainViewModel()
        {
            // 適当な設定を入れておく
            _cards = new ObservableCollection<Card>()
            {
                new Card {Name = "戸山香澄", Title="debug", Rarity=4, CardType=Card.Type.Happy },
                new Card {Name = "市ヶ谷有咲", Title="debug", Rarity=4, CardType=Card.Type.Cool },
                new Card {Name = "花園たえ", Title="debug", Rarity=4, CardType=Card.Type.Pure },
            };
        }

        /// <summary>
        /// カード情報
        /// </summary>
        private ObservableCollection<Card> _cards = new ObservableCollection<Card>()
            {
                new Card {Name = "戸山香澄", Title="debug", Rarity=4, CardType=Card.Type.Happy },
                new Card {Name = "市ヶ谷有咲", Title="debug", Rarity=4, CardType=Card.Type.Cool },
                new Card {Name = "花園たえ", Title="debug", Rarity=4, CardType=Card.Type.Pure },
            };

        public ObservableCollection<Card> Cards
        {
            get
            {
                return _cards;
            }
            set
            {
                _cards = value;
                this.NotifyPropertyChanged(nameof(Cards));
            }
        }
    }
}
