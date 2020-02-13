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

            // 各パターンの最適編成
            var optimumTeams = okimonoPatterns.Select(pattern => this.TeamUpWithPattern(cardGroups, pattern));

            // 最適編成結果の通知
            this.Teams = new ObservableCollection<SpecializedTeam>(optimumTeams);
        }


        /// <summary>
        /// バンド/属性特化の置物パターンの最適編成パターンを取得
        /// </summary>
        /// <remarks>現状は総合力が一番高くなる編成を最適としている</remarks>
        /// <returns>最適編成パターン</returns>
        private SpecializedTeam TeamUpWithPattern(
            IEnumerable<IGrouping<string, Card>> cardGroups,
            (IEnumerable<Okimono> okimonos, Card.Band targetBand, Card.Type targetAttribute) okimonoPattern)
        {
            // 置物による3つのカードボーナスを先に計算する
            var bandBonusOkimonoList = okimonoPattern.okimonos.Where(okimono => okimono.TargetBands.Count > 0);
            var performanceBandBonus = bandBonusOkimonoList.Sum(o => o.Bonus[o.Level].performance) / 10.0;
            var techniqueBandBonus = bandBonusOkimonoList.Sum(o => o.Bonus[o.Level].technique) / 10.0;
            var visualBandBonus = bandBonusOkimonoList.Sum(o => o.Bonus[o.Level].visual) / 10.0;

            var typeBonusOkimonoList = okimonoPattern.okimonos.Where(okimono => okimono.TargetTypes.Count > 0);
            var performanceTypeBonus = typeBonusOkimonoList.Sum(okimono => okimono.Bonus[okimono.Level].performance) / 10.0;
            var techniqueTypeBonus = typeBonusOkimonoList.Sum(okimono => okimono.Bonus[okimono.Level].technique) / 10.0;
            var visualTypeBonus = typeBonusOkimonoList.Sum(okimono => okimono.Bonus[okimono.Level].visual) / 10.0;

            // 各キャラのカードの中で置物補正込みの総合力が最も高い1枚を取り出し, その総合力が高い順に5キャラを選ぶ
            var optimumCharacters = cardGroups.Select(group => group
                .Select(card =>
                {
                    var bonusPerformance = default(double);
                    var bonusTechnique = default(double);
                    var bonusVisual = default(double);

                    if (card.BandName == okimonoPattern.targetBand)
                    {
                        bonusPerformance += card.MaxPerformance * (performanceBandBonus / 100.0);
                        bonusTechnique += card.MaxTechnique * (techniqueBandBonus / 100.0);
                        bonusVisual += card.MaxVisual * (visualBandBonus / 100.0);
                    }
                    if (card.CardType == okimonoPattern.targetAttribute)
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

            return new SpecializedTeam {
                Members = optimumCharacters.Select(c => c.Card), Okimonos = okimonoPattern.okimonos,
                BandSpecialized = okimonoPattern.targetBand, TypeSpecialized = okimonoPattern.targetAttribute};
        }

        #endregion

        #region Private Helper



        #endregion

        #region バインディング用フィールド



        /// <summary>
        /// 編成
        /// </summary>
        private ObservableCollection<SpecializedTeam> _teams;

        /// <summary>
        /// 編成 変更通知用プロパティ
        /// </summary>
        public ObservableCollection<SpecializedTeam> Teams
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
