using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GarupaSimulator.WpfUtil;
using System.Windows.Input;
using System.Net.Http;
using AngleSharp;

namespace GarupaSimulator.ViewModels
{
    /// <summary>
    /// メインウィンドウのビューモデル
    /// </summary>
    internal class MainViewModel : WpfUtil.ViewModelBase
    {
        private Views.MainWindow _mainWnd;

        /// <summary>
        /// HTTPクライアント
        /// </summary>
        /// <remarks>HttpClientがusing非推奨のためメンバ変数化している</remarks>
        HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 1000000 };

        /// <summary>
        /// 全メンバーの名前
        /// </summary>
        private List<List<string>> _memberNames = new List<List<string>>()
        {
            new List<string>() { "戸山香澄", "花園たえ", "牛込りみ", "山吹沙綾", "市ヶ谷有咲" }, // Poppin' Party
            new List<string>() { "美竹蘭", "青葉モカ", "上原ひまり", "宇田川巴", "羽沢つぐみ" }, // Afterglow
            new List<string>() { "丸山彩", "氷川日菜", "白鷺千聖", "大和麻弥", "若宮イヴ" }, // Pastel Palettes
            new List<string>() { "湊友希那", "氷川紗夜", "今井リサ", "宇田川あこ", "白金燐子" }, // Roselia
            new List<string>() { "弦巻こころ", "瀬田薫", "北沢はぐみ", "松原花音", "奥沢美咲" }, // Hello Happy World
        };

        /// <summary>
        /// カード情報を保存するパス
        /// </summary>
        private string _cardInfoPath = @"cardList.xml";

        #region スクレイピング先情報

        /// <summary>
        /// レア度4のカード一覧のスクレイピング先URL
        /// </summary>
        private static readonly string _rare4url = @"https://gameo.jp/bang-dream/1623";

        /// <summary>
        /// レア度4のカード一覧用CSSセレクタ
        /// </summary>
        private static readonly string _rare4selector = "#tablepress-9 > tbody tr td a";

        /// <summary>
        /// レア度3のカード一覧のスクレイピング先URL
        /// </summary>
        private static readonly string _rare3url = @"https://gameo.jp/bang-dream/1645";

        /// <summary>
        /// レア度3のカード一覧用CSSセレクタ
        /// </summary>
        private static readonly string _rare3selector = "#tablepress-10 > tbody tr td a";

        #endregion



        #region ctor

        internal MainViewModel(Views.MainWindow wnd)
        {
            _mainWnd = wnd;

            // ファイルから既存情報を読み取る
            var cardList = File.BinarySerializer.LoadFromBinaryFile(_cardInfoPath) as IEnumerable<Card>;

            if (cardList != null)
                this.Cards = new ObservableCollection<Card>(cardList);
            else
            {
                System.Windows.MessageBox.Show(
                    "カード情報が保存されていません.\nインターネットに接続し更新ボタンから最新のカード情報を取得する必要があります.",
                    "カード設定未保存");
                this.Cards = new ObservableCollection<Card>();
            }
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
                new Card {Name = "戸山香澄", Title="debug", BandName=Card.Band.PoppinParty, Rarity=4, CardType=Card.Type.Happy },
                new Card {Name = "市ヶ谷有咲", Title="debug", BandName=Card.Band.PoppinParty, Rarity=4, CardType=Card.Type.Cool },
                new Card {Name = "花園たえ", Title="debug", BandName=Card.Band.PoppinParty, Rarity=4, CardType=Card.Type.Pure },
            };
        }

        #endregion

        #region コマンド

        /// <summary>
        /// カード情報更新
        /// </summary>
        private ICommand updateCommand;
        public ICommand UpdateCardsCommand => updateCommand ?? (updateCommand = new DelegateCommand(UpdateCards, null));

        /// <summary>
        /// カード情報の再取得
        /// </summary>
        private ICommand reUpdateCommand;
        public ICommand ReUpdateCardsCommand => reUpdateCommand ?? (reUpdateCommand = new DelegateCommand(ReUpdateCards, null));

        /// <summary>
        /// ローカルに存在しないカード情報を更新する
        /// </summary>
        private async void UpdateCards(object o) => await this.UpdateCardsInternal();

        /// <summary>
        /// カード情報を1から更新し直す
        /// </summary>
        private async void ReUpdateCards(object o) => await this.UpdateCardsInternal(true);

        /// <summary>
        /// カード情報を更新する
        /// </summary>
        /// <param name="isCleanUpdate">現在のローカルのカード情報を破棄してすべてのカード情報を再取得するか</param>
        private async Task UpdateCardsInternal(bool isCleanUpdate = false)
        {
            var tokenSource = new System.Threading.CancellationTokenSource();
            var cancelToken = tokenSource.Token;

            // ローカルに存在しないカードのURLを取得
            var newCard4Urls = await GetNewCardUrlsAsync(_rare4url, _rare4selector, isCleanUpdate, cancelToken); // 新規星4
            var newCard3Urls = await GetNewCardUrlsAsync(_rare3url, _rare3selector, isCleanUpdate, cancelToken); // 新規星3
            var newCardUrls = newCard4Urls.Union(newCard3Urls).ToList();

            // ローカルとサーバのカード一覧が同じであれば更新しない
            if (newCardUrls.Count == 0)
            {
                this.ShowUpdatedCountMessage(newCardUrls.Count);
                return;
            }

            // 新規カード情報を取得
            var cardInfos = await GetCardsInfoAsync(newCardUrls, cancelToken);

            // ビュー更新
            if (isCleanUpdate)
                this.Cards = new ObservableCollection<Card>(cardInfos);
            else
                cardInfos.ForEach(card => this.Cards.Add(card));

            // ファイル更新
            File.BinarySerializer.SaveToBinaryFile(this.Cards, _cardInfoPath);
            this.ShowUpdatedCountMessage(newCardUrls.Count);
        }

        /// <summary>
        /// ローカルに存在しない新規カードのURLを取得する
        /// </summary>
        /// <param name="url">スクレイピング先のカード一覧のURL</param>
        private async Task<IEnumerable<string>> GetNewCardUrlsAsync(string url, string selector, bool isCleanUpdate, System.Threading.CancellationToken cancelToken)
        {
            // htmlソースをパース
            var doc = default(AngleSharp.Html.Dom.IHtmlDocument);
            using (var stream = await _client.GetStreamAsync(new Uri(url)))
            {
                var parser = new AngleSharp.Html.Parser.HtmlParser();
                doc = await parser.ParseDocumentAsync(stream, cancelToken);
            }

            // 表からカードの情報を取得
            var infos = doc.QuerySelectorAll(selector);

            if (isCleanUpdate)
            {
                return infos
                    .Select(info => (info as AngleSharp.Html.Dom.IHtmlAnchorElement).Href)
                    .ToList();
            }
            else
            {
                var newCardUrls = new List<string>();

                foreach (var info in infos)
                {
                    // カードタイトル(主キー)がローカルに存在しなかったら
                    if (_cards.Any(card => card.Title == info.TextContent.InnerBracket()) == false)
                    {
                        // カード情報ページのURLを追加
                        var cardUrl = (info as AngleSharp.Html.Dom.IHtmlAnchorElement).Href;
                        if (cardUrl != null)
                            newCardUrls.Add(cardUrl);
                    }
                    else
                    {
                        // カード情報が既にローカルに存在したらスルー
                        continue;
                    }
                }

                return newCardUrls;
            }
        }

        /// <summary>
        /// URL群からそれぞれのカード情報をスクレイピングする
        /// </summary>
        /// <param name="urls">スクレイピング先のURL群</param>
        private async Task<IEnumerable<Card>> GetCardsInfoAsync(IEnumerable<string> urls, System.Threading.CancellationToken cancelToken)
        {
            // URLからタスクを生成しカード情報を非同期で取得する
            var taskList = urls.Select(url => GetCardInfoAsync(url, cancelToken));
            var cards = await Task.WhenAll(taskList);
            return cards;
        }

        /// <summary>
        /// URLからカード情報をスクレイピングする
        /// </summary>
        /// <param name="url">スクレイピング先のURL</param>
        private async Task<Card> GetCardInfoAsync(string url, System.Threading.CancellationToken cancelToken)
        {
            var doc = default(AngleSharp.Html.Dom.IHtmlDocument);

            using (var stream = await _client.GetStreamAsync(new Uri(url)))
            {
                var parser = new AngleSharp.Html.Parser.HtmlParser();
                doc = await parser.ParseDocumentAsync(stream, cancelToken);
            }

            // サイトの表から情報を取得
            var items = doc.QuerySelectorAll("table > tbody > tr td, table > tbody > tr th");
            var title = doc.QuerySelector("h2 > span").TextContent.InnerBracket();

            return new Card
            {
                Name = items[5].TextContent.Replace(" ", ""),
                Title = title,
                BandName = this.ConvertBandName(items[7].TextContent),
                Rarity = items[1].TextContent.Count(),
                CardType = this.ConvertCardType(items[3].TextContent),

                MaxPerformance = int.Parse(items[11].TextContent),
                MaxTechnique = int.Parse(items[13].TextContent),
                MaxVisual = int.Parse(items[15].TextContent),

                SkillName = items[16].TextContent,
                SkillEffect = items[17].TextContent,
            };
        }

        #endregion

        #region Private Helper

        /// <summary>
        /// カード情報の更新数をモーダルでメッセージ表示する
        /// </summary>
        /// <param name="updateCount">更新件数</param>
        private void ShowUpdatedCountMessage(int updateCount)
        {
            var message = String.Format("更新件数: {0}", updateCount);
            if (updateCount == 0)
                message = message.Insert(0, "このカード情報は最新です.\n");

            System.Windows.MessageBox.Show(message, "更新結果");
        }

        /// <summary>
        /// バンド名を<see cref="string"/>から<see cref="Card.Band"/>へ変換
        /// </summary>
        private Card.Band ConvertBandName(string name)
        {
            if (name.StartsWith("Po", StringComparison.OrdinalIgnoreCase))
                return Card.Band.PoppinParty;
            else if (name.StartsWith("A", StringComparison.OrdinalIgnoreCase))
                return Card.Band.Afterglow;
            else if (name.StartsWith("Pa", StringComparison.OrdinalIgnoreCase))
                return Card.Band.PastelPalettes;
            else if (name.StartsWith("R", StringComparison.OrdinalIgnoreCase))
                return Card.Band.Roselia;
            else if (name.StartsWith("ハ") || name.StartsWith("H", StringComparison.OrdinalIgnoreCase))
                return Card.Band.HelloHappyWorld;
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// カード属性を<see cref="string"/>から<see cref="Card.Type"/>へ変換
        /// </summary>
        private Card.Type ConvertCardType(string type)
        {
            if (type.StartsWith("パ"))
                return Card.Type.Powerful;
            else if (type.StartsWith("ク"))
                return Card.Type.Cool;
            else if (type.StartsWith("ピ"))
                return Card.Type.Pure;
            else if (type.StartsWith("ハ"))
                return Card.Type.Happy;
            else
                throw new NotSupportedException();
        }

        #endregion


        #region バインディング用フィールド

        /// <summary>
        /// カード情報
        /// </summary>
        private ObservableCollection<Card> _cards;

        /// <summary>
        /// カード情報 変更通知用プロパティ
        /// </summary>
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

        #endregion
    }
}
