using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarupaSimulator
{
    /// <summary>
    /// 編成 クラス
    /// </summary>
    public class Team : WpfUtil.ModelBase
    {
        /// <summary>
        /// 編成メンバーの人数
        /// </summary>
        private static readonly int _teamMemberCount = 5;

        #region ctor



        #endregion

        #region Property

        private IEnumerable<Card> _members = default(IEnumerable<Card>);

        /// <summary>
        /// 編成メンバー
        /// </summary>
        public IEnumerable<Card> Members
        {
            get { return _members; }
            set
            {
                // 編成メンバーの人数が不足していた場合, 空のカードで穴埋めする
                var shortageCards = this.GetTeamMemberShortageCards(value);
                _members = value.Concat(shortageCards);
            }
        }

        /// <summary>
        /// 適用する置物
        /// </summary>
        public IEnumerable<Okimono> Okimonos { get; set; }

        /// <summary>
        /// 編成ライフ
        /// </summary>
        public int Life { get; set; } = 1000;

        #endregion

        #region for Binding getter

        /// <summary>
        /// 編成のパフォーマンス値
        /// </summary>
        public int PerformancePower { get { return (int)this.CalcTeamPerformancePower(); } }

        /// <summary>
        /// 編成のテクニック値
        /// </summary>
        public int TechniquePower { get { return (int)this.CalcTeamTechniquePower(); } }

        /// <summary>
        /// 編成のビジュアル値
        /// </summary>
        public int VisualPower { get { return (int)this.CalcTeamVisualPower(); } }

        /// <summary>
        /// 編成の総合力
        /// </summary>
        public int OverallPower { get { return this.PerformancePower + this.TechniquePower + this.VisualPower; } }

        #endregion

        /// <summary>
        /// 不足分のカードを取得する（デフォルト値）
        /// </summary>
        private IEnumerable<Card> GetTeamMemberShortageCards(IEnumerable<Card> optimumCharacters)
        {
            // 既定メンバー数(5人)に足りない分のカードを作成
            var emptyCards = new List<Card>();
            for (int i = 0; i < _teamMemberCount - optimumCharacters.Count(); ++i)
                emptyCards.Add(new Card());

            return emptyCards;
        }

        #region Private Helper

        /// <summary>
        /// 編成のパフォーマンス値を取得
        /// </summary>
        /// <param name="isMax">最大値(理想値)を取得するか</param>
        private double CalcTeamPerformancePower(bool isMax = false)
        {
            return this.Members
                .Select(card =>
                {
                    var bandBonus = this.Okimonos.Where(o => o.TargetBands.Contains(card.BandName))
                                                    .Sum(o => o.Bonus[isMax ? o.Levels.Count() - 1 : o.Level].performance) / 10.0;
                    var typeBonus = this.Okimonos.Where(o => o.TargetTypes.Contains(card.CardType))
                                                    .Sum(o => o.Bonus[isMax ? o.Levels.Count() - 1 : o.Level].performance) / 10.0;

                    return  card.MaxPerformance + card.MaxPerformance * (bandBonus / 100) + card.MaxPerformance * (typeBonus / 100);
                })
                .Sum();
        }

        /// <summary>
        /// 編成のテクニック値を取得
        /// </summary>
        /// <param name="isMax">最大値(理想値)を取得するか</param>
        private double CalcTeamTechniquePower(bool isMax = false)
        {
            return this.Members
                .Select(card =>
                {
                    var bandBonus = this.Okimonos.Where(o => o.TargetBands.Contains(card.BandName))
                                                    .Sum(o => o.Bonus[isMax ? o.Levels.Count() - 1 : o.Level].technique) / 10.0;
                    var typeBonus = this.Okimonos.Where(o => o.TargetTypes.Contains(card.CardType))
                                                    .Sum(o => o.Bonus[isMax ? o.Levels.Count() - 1 : o.Level].technique) / 10.0;

                    return card.MaxTechnique + card.MaxTechnique * (bandBonus / 100) + card.MaxTechnique * (typeBonus / 100);
                })
                .Sum();
        }

        /// <summary>
        /// 編成のビジュアル値を取得
        /// </summary>
        /// <param name="isMax">最大値(理想値)を取得するか</param>
        private double CalcTeamVisualPower(bool isMax = false)
        {
            return this.Members
                .Select(card =>
                {
                    var bandBonus = this.Okimonos.Where(o => o.TargetBands.Contains(card.BandName))
                                                    .Sum(o => o.Bonus[isMax ? o.Levels.Count() - 1 : o.Level].visual) / 10.0;
                    var typeBonus = this.Okimonos.Where(o => o.TargetTypes.Contains(card.CardType))
                                                    .Sum(o => o.Bonus[isMax ? o.Levels.Count() - 1 : o.Level].visual) / 10.0;

                    return card.MaxVisual + card.MaxVisual * (bandBonus / 100) + card.MaxVisual * (typeBonus / 100);
                })
                .Sum();
        }

        #endregion
    }
}
