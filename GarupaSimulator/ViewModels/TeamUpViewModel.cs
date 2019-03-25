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

            // フリー/イベント外 編成
            // ただしフリーはハイスコアレーティングが高い方が最適なので後から別に考え直すかも

            // 設置場所ごとに置物をグループ分け（設置場所毎に1つの置物）
            var okimonos = _areas.Select(area => area.AreaItems).SelectMany(p => p);
            var okimonoGroup = okimonos.GroupBy(item => item.LocationName);

            // キャラクターごとにグループ分け（編成は同キャラ不可）
            var cardGroup = _cards.GroupBy(card => card.Name);

            // バンド/属性特化の総置物パターン(5×4=20パターン)の置物リスト
            var okimonoPatterns = new List<(IEnumerable<Okimono> okimonos, Card.Band targetBand, Card.Type targetAttribute)>();
            var specialized = default(IEnumerable<Okimono>);
            foreach (Card.Band band in Enum.GetValues(typeof(Card.Band)))
            {
                foreach (Card.Type attribute in Enum.GetValues(typeof(Card.Type)))
                {
                    specialized = okimonos.Where(okimono =>
                        (okimono.TargetBands.Count == 1 && okimono.TargetBands.FirstOrDefault() == band) ||
                        (okimono.TargetTypes.Count == 1 && okimono.TargetTypes.FirstOrDefault() == attribute));

                    okimonoPatterns.Add((specialized, band, attribute));
                }
            }

            var optimumPattern = okimonoPatterns.Select(pattern =>
            {
                // 置物による3つのカードボーナスを先に計算する
                var bandBonusOkimonoList = pattern.okimonos.Where(okimono => okimono.TargetBands.Count > 0);
                var performanceBandBonus = bandBonusOkimonoList.Sum(o => o.Bonus[o.Level].performance) / 10.0;
                var techniqueBandBonus = bandBonusOkimonoList.Sum(o => o.Bonus[o.Level].technique) / 10.0;
                var visualBandBonus = bandBonusOkimonoList.Sum(o => o.Bonus[o.Level].visual) / 10.0;

                var typeBonusOkimonoList = pattern.okimonos.Where(okimono => okimono.TargetTypes.Count > 0);
                var performanceTypeBonus = typeBonusOkimonoList.Sum(okimono => okimono.Bonus[okimono.Level].performance) / 10.0;
                var techniqueTypeBonus = typeBonusOkimonoList.Sum(okimono => okimono.Bonus[okimono.Level].technique) / 10.0;
                var visualTypeBonus = typeBonusOkimonoList.Sum(okimono => okimono.Bonus[okimono.Level].visual) / 10.0;

                // 各キャラのカードの中で置物補正込みの総合力が最も高い1枚を取り出し, その総合力が高い順に5キャラを選ぶ
                var optimumCharacters = cardGroup.Select(group => group
                    .Select(card =>
                    {
                        var bonusPerformance = default(double);
                        var bonusTechnique = default(double);
                        var bonusVisual = default(double);

                        if (card.BandName == pattern.targetBand)
                        {
                            bonusPerformance += card.MaxPerformance * (performanceBandBonus / 100.0);
                            bonusTechnique += card.MaxTechnique * (techniqueBandBonus / 100.0);
                            bonusVisual += card.MaxVisual * (visualBandBonus / 100.0);
                        }
                        if (card.CardType == pattern.targetAttribute)
                        {
                            bonusPerformance += card.MaxPerformance * (performanceTypeBonus / 100.0);
                            bonusTechnique += card.MaxTechnique * (techniqueTypeBonus / 100.0);
                            bonusVisual += card.MaxVisual * (visualTypeBonus / 100.0);
                        }

                        // 補正値込みのカード単体の総合力
                        var cardPower = (
                            card.MaxPerformance + bonusPerformance +
                            card.MaxTechnique + bonusTechnique +
                            card.MaxVisual + bonusVisual);

                        return new
                        {
                            CardPower = cardPower,
                            Card = card,
                        };
                    })
                    .OrderByDescending(item => item.CardPower)
                    .FirstOrDefault())
                .OrderByDescending(item => item.CardPower)
                .Take(5);

                return new { PatternPower = optimumCharacters.Sum(c => c.CardPower), OptimumCharacters = optimumCharacters.Select(c => c.Card), Pattern = pattern };
            })
            .OrderByDescending(item => item.PatternPower)
            .FirstOrDefault();

            // 最適編成結果
            {
                this.TeamCards = new ObservableCollection<Card>(optimumPattern.OptimumCharacters);
                // TODO: 後から最適の置物追加

                this.Life = this.Life;
                this.BandPower = (int)optimumPattern.PatternPower;
            }
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
