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
    /// 最適編成ビューのビューモデル
    /// </summary>
    public class TeamUpViewModel : ViewModelBase
    {
        /// <summary>
        /// カード情報
        /// </summary>
        private List<Card> _cards;

        /// <summary>
        /// 置物情報
        /// </summary>
        private List<OkimonoArea> _areas;

        /// <summary>
        /// イベント情報を保存するパス
        /// </summary>
        private string _eventInfoPath = @"eventInfo.xml";

        #region ctor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal TeamUpViewModel(List<Card> cards, List<OkimonoArea> areas)
        {
            _cards = cards;
            _areas = areas;

            if (!System.IO.File.Exists(_eventInfoPath))
            {
                // デフォルトのイベント (空のイベント情報)
                var eventDefault = new List<Event> {
                    new Event { Name = "イベント外期間" },
                    new Event { Name = "君に伝うメッセージ", BoostType = Card.Type.Happy, BoostTypeBonus = 20, CardBonus = (0, 0, 0), EventType = Event.HeldType.Try,
                        BoostMemberBonus = new List<(string member, int bonus)> { ("戸山香澄", 10), ("花園たえ", 10), ("牛込りみ", 10), ("山吹沙綾", 10), ("市ヶ谷有咲", 10) },
                     Period = "3/8(金)~3/14(木)"},
                    new Event { Name = "Backstage Pass 2", BoostType = Card.Type.Powerful, BoostTypeBonus = 20, CardBonus = (0, 0, 0), EventType = Event.HeldType.Mission,
                        BoostMemberBonus = new List<(string member, int bonus)> { ("花園たえ", 10), ("青葉モカ", 10), ("瀬田薫", 10), ("氷川日奈", 10), ("氷川紗夜", 10) },
                     Period="3/16(土)~3/29(金)"}
                };
                File.XmlSerializer.SaveToBinaryFile(eventDefault, eventDefault.GetType(), _eventInfoPath);
            }

            var eventList = File.XmlSerializer.LoadFromBinaryFile(typeof(List<Event>), _eventInfoPath) as IEnumerable<Event>;
            this.Events = new ObservableCollection<Event>(eventList);
        }

        /// <summary>
        /// デザイナー用 使用禁止
        /// </summary>
        [Obsolete("Designer Only", true)]
        public TeamUpViewModel()
        {
            this.Events = new ObservableCollection<Event>
            {
                new Event { Name = "イベント外期間" },
                new Event { Name = "BackStagePass2" },
            };
        }

        #endregion

        #region ViewModel override

        /// <summary>
        /// メインビューを閉じる時に実行する
        /// </summary>
        public override void ClosedView(ClosedViewArgs arg)
        {
            this.CloseViewCommandImplement(arg);
            base.ClosedView(arg);
        }

        /// <summary>
        /// メインビューを閉じる時に実行する
        /// </summary>
        /// <remarks>CloseViewCommandをバインディングした際に呼ばれる</remarks>
        protected override void CloseViewCommandImplement(object o)
        {
            // 置物情報を保存する
            File.XmlSerializer.SaveToBinaryFile(_events, _events.GetType(), _eventInfoPath);

            base.CloseViewCommandImplement(o);
        }

        #endregion

        #region コマンド

        // 最適編成
        private ICommand teamupCommand;
        public ICommand TeamUpCommand => teamupCommand ?? (teamupCommand = new DelegateCommand(TeamUpOptimally, null));

        /// <summary>
        /// イベント情報から最適編成を構成する
        /// </summary>
        /// <param name="eventInfo">イベント情報</param>
        private void TeamUpOptimally(object eventInfo)
        {
            // TODO: 各イベントごとの最適編成

            // イベント外通常編成



            this.TeamCards = new ObservableCollection<Card>(_cards.Take(5));
            this.Life = this.Life;
            this.BandPower = this.TeamCards.Sum(card => card.MaxTotal);
        }

        // 詳細パネル表示
        private ICommand showDetail;
        public ICommand ShowDetailPanelCommand =>
            showDetail ?? (showDetail = new DelegateCommand((object o) => { this.IsDetailOpen = true; ; }, null));

        #endregion

        #region バインディング用フィールド

        /// <summary>
        /// イベント情報
        /// </summary>
        private ObservableCollection<Event> _events;

        /// <summary>
        /// イベント情報 変更通知用プロパティ
        /// </summary>
        public ObservableCollection<Event> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                this.NotifyPropertyChanged(nameof(Events));
            }
        }

        /// <summary>
        /// イベント名一覧
        /// </summary>
        public IEnumerable<string> EventNames { get { return _events.Select(e => e.Name); } }

        /// <summary>
        /// 編成情報
        /// </summary>
        private ObservableCollection<Card> _teamCards = new ObservableCollection<Card>
        {
            // 5枚の空カードを初期値とする
            new Card(), new Card(), new Card(), new Card(), new Card(),
        };

        /// <summary>
        /// 編成情報 変更通知用プロパティ
        /// </summary>
        public ObservableCollection<Card> TeamCards
        {
            get
            {
                return _teamCards;
            }
            set
            {
                _teamCards = value;
                this.NotifyPropertyChanged(nameof(TeamCards));
            }
        }

        /// <summary>
        /// ライフ
        /// </summary>
        private int _life = 1000;

        /// <summary>
        /// ライフ 変更通知用プロパティ
        /// </summary>
        public int Life
        {
            get { return _life; }
            set
            {
                _life = value;
                this.NotifyPropertyChanged(nameof(Life));
            }
        }

        /// <summary>
        /// バンド総合力 名前変えるかも...
        /// </summary>
        private int _bandPower = 0;

        /// <summary>
        /// バンド総合力 変更通知用プロパティ
        /// </summary>
        public int BandPower
        {
            get { return _bandPower; }
            set
            {
                _bandPower = value;
                this.NotifyPropertyChanged(nameof(BandPower));
            }
        }

        /// <summary>
        /// 詳細パネル表示
        /// </summary>
        private bool _isDetailOpen = false;

        /// <summary>
        /// 詳細パネル表示 変更通知用プロパティ
        /// </summary>
        public bool IsDetailOpen
        {
            get { return _isDetailOpen; }
            set
            {
                _isDetailOpen = value;
                NotifyPropertyChanged(nameof(IsDetailOpen));
            }
        }

        #endregion
    }
}
