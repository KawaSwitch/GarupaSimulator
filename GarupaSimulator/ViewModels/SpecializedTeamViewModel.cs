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
    /// 特化編成ビューのビューモデル
    /// </summary>
    public class SpecializedTeamViewModel : ViewModelBase
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
        /// 編成メンバーの人数
        /// </summary>
        private static readonly int _teamMemberCount = 5;

        #region ctor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal SpecializedTeamViewModel(List<Card> cards, List<OkimonoArea> areas)
        {
            _cards = cards;
            _areas = areas;
        }

        /// <summary>
        /// デザイナー用 使用禁止
        /// </summary>
        [Obsolete("Designer Only", true)]
        public SpecializedTeamViewModel()
        {
        }

        #endregion

        #region コマンド

        // 特化編成
        private ICommand teamupCommand;
        public ICommand TeamUpCommand => teamupCommand ?? (teamupCommand = new DelegateCommand(TeamUpAllSpecialized, null));

        /// <summary>
        /// すべてのバンド/属性特化の置物パターンでチーム編成する
        /// </summary>
        private void TeamUpAllSpecialized(object o)
        {
            // キャラクターごとにグループ分け（編成は同キャラ不可）
            var cardGroups = _cards
                .Where(card => (this.IsTeamUpWithOnlyOwned) ? card.IsOwned : true) // 所持情報使用の有無
                .GroupBy(card => card.Name);

            // 全ての置物データ
            var okimonos = _areas.Select(area => area.AreaItems).SelectMany(p => p);

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

            // TODO: 未完成

            // イベント外 編成パターン
            var optimumTeam = ViewModels.TeamUpViewModel.TeamUpWhenNoEvents(cardGroups, okimonoPatterns);
            var optimumCharacters = optimumTeam.Members;

            // 最適編成結果の通知
            //this.TeamCards = new ObservableCollection<Card>(optimumCharacters);
        }

        #endregion

        #region Private Helper



        #endregion

        #region バインディング用フィールド



        /// <summary>
        /// 編成
        /// </summary>
        private ObservableCollection<Team> _teams;

        /// <summary>
        /// 編成 変更通知用プロパティ
        /// </summary>
        public ObservableCollection<Team> Teams
        {
            get
            {
                return _teams;
            }
            set
            {
                _teams = value;
                this.NotifyPropertyChanged(nameof(Teams));
            }
        }

        /// <summary>
        /// 所持情報使用
        /// </summary>
        private bool _isTeamUpWithOnlyOwned = true;

        /// <summary>
        /// 所持情報使用 変更通知用プロパティ
        /// </summary>
        public bool IsTeamUpWithOnlyOwned
        {
            get { return _isTeamUpWithOnlyOwned; }
            set
            {
                _isTeamUpWithOnlyOwned = value;
                NotifyPropertyChanged(nameof(IsTeamUpWithOnlyOwned));
            }
        }

        #endregion
    }
}
