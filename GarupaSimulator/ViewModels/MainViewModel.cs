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

        /// <summary>
        /// 置物情報を保存するパス
        /// </summary>
        private string _okimonoInfoPath = @"okimonoList.xml";

        /// <summary>
        /// カードの特訓前アイコンを保存するディレクトリ名
        /// </summary>
        private string _cardIconBeforeDir = "downloadIcons";

        /// <summary>
        /// エピソード/メモリアルエピソード解放時の各ステータス上昇値
        /// </summary>
        /// <remarks>パフォーマンス・テクニック・ビジュアルそれぞれに等しく上昇値が上乗せされる</remarks>
        private List<(int episode, int memorial)> _episodeRelaseStatusUp = new List<(int episode, int memorial)>
        {
            (100, 200), // 星1
            (150, 300), // 星2
            (200, 500), // 星3
            (250, 600), // 星4
        };

        #region スクレイピング情報

        /// <summary>
        /// レア度4のカード一覧のスクレイピング先URL
        /// </summary>
        private static readonly string _rare4url = @"https://gameo.jp/bang-dream/1623";

        /// <summary>
        /// レア度4のカード一覧用CSSセレクタ
        /// </summary>
        private static readonly string _rare4selector = "#tablepress-9 > tbody tr td a";
        /// <summary>
        /// レア度4のカードアイコン用CSSセレクタ
        /// </summary>
        private static readonly string _rare4IconSelector = "#tablepress-9 > tbody tr td img";

        /// <summary>
        /// レア度3のカード一覧のスクレイピング先URL
        /// </summary>
        private static readonly string _rare3url = @"https://gameo.jp/bang-dream/1645";

        /// <summary>
        /// レア度3のカード一覧用CSSセレクタ
        /// </summary>
        private static readonly string _rare3selector = "#tablepress-10 > tbody tr td a";
        /// <summary>
        /// レア度3のカードアイコン用CSSセレクタ
        /// </summary>
        private static readonly string _rare3IconSelector = "#tablepress-10 > tbody tr td img";

        #endregion

        #region ctor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="wnd">メインビュー</param>
        internal MainViewModel(Views.MainWindow wnd) : base()
        {
            _mainWnd = wnd;

            // 各種保存データ読み込み
            this.LoadDatas();
        }

        /// <summary>
        /// 各種保存データをファイルから読み込み格納する
        /// </summary>
        private void LoadDatas()
        {
            this.LoadCardDatas(); // カード
            this.LoadOkimonoDatas(); // 置物
        }
        /// <summary>
        /// カードデータをファイルから読み込む
        /// </summary>
        private void LoadCardDatas()
        {
            // ファイルから保存情報を読み取る
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
        /// 置物データをファイルから読み込む
        /// </summary>
        private void LoadOkimonoDatas()
        {
            // 置物保存のデータファイルが存在しなければ作成（ハードコーディングのデータ）
            // TODO: いつかスクレイピング？
            if (!System.IO.File.Exists(_okimonoInfoPath))
            {
                // 全エリアと置物情報
                var _areaDataLocal = new List<OkimonoArea>()
                {
                    #region 江戸川楽器店

                    new OkimonoArea {
                        Name = "江戸川楽器店",
                        AreaItems = new List<Okimono> {
                            // スタジオ(マイク)
                            new Okimono {
                                Name = "スタジオマイク",
                                LocationName = "スタジオ(マイク)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "ロックマイク",
                                LocationName = "スタジオ(マイク)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "アイドルマイク",
                                LocationName = "スタジオ(マイク)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "青薔薇のマイク",
                                LocationName = "スタジオ(マイク)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "マーチングマイク",
                                LocationName = "スタジオ(マイク)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },

                            // スタジオ(ギター)
                            new Okimono {
                                Name = "たえのギター",
                                LocationName = "スタジオ(ギター)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "モカのギター",
                                LocationName = "スタジオ(ギター)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "日菜のギター",
                                LocationName = "スタジオ(ギター)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "紗夜のギター",
                                LocationName = "スタジオ(ギター)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "薫のギター",
                                LocationName = "スタジオ(ギター)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },

                            // スタジオ(ベース)
                            new Okimono {
                                Name = "りみのベース",
                                LocationName = "スタジオ(ベース)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "ひまりのベース",
                                LocationName = "スタジオ(ベース)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "千聖のベース",
                                LocationName = "スタジオ(ベース)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "リサのベース",
                                LocationName = "スタジオ(ベース)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "はぐみのベース",
                                LocationName = "スタジオ(ベース)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },

                            // スタジオ(ドラム)
                            new Okimono {
                                Name = "沙綾のドラム",
                                LocationName = "スタジオ(ドラム)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "巴のドラム",
                                LocationName = "スタジオ(ドラム)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "麻弥のドラム",
                                LocationName = "スタジオ(ドラム)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "あこのドラム",
                                LocationName = "スタジオ(ドラム)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "花音のドラム",
                                LocationName = "スタジオ(ドラム)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },

                            // スタジオ(その他)
                            new Okimono {
                                Name = "有咲のキーボード",
                                LocationName = "スタジオ(その他)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "つぐみのキーボード",
                                LocationName = "スタジオ(その他)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "イヴのキーボード",
                                LocationName = "スタジオ(その他)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "燐子のキーボード",
                                LocationName = "スタジオ(その他)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "美咲のDJセット",
                                LocationName = "スタジオ(その他)",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (20, 20, 20), (25, 25, 25), (30, 30, 30), (35, 35, 35), (40, 40, 40), (45, 45, 45) },
                                Level = 0,
                            },
                        }
                    },

                    #endregion

                    #region CiRCLE

                    new OkimonoArea { Name = "CiRCLE",
                        AreaItems = new List<Okimono> {
                            // ポスター
                            new Okimono {
                                Name = "ポピパのポスター",
                                 LocationName = "ポスター",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "Afterglowのポスター",
                                 LocationName = "ポスター",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "パスパレのポスター",
                                 LocationName = "ポスター",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "Roseliaのポスター",
                                 LocationName = "ポスター",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "ハロハピのポスター",
                                 LocationName = "ポスター",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },

                            // フライヤー
                            new Okimono {
                                Name = "ポピパのフライヤー",
                                 LocationName = "フライヤー",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PoppinParty },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "Afterglowのフライヤー",
                                 LocationName = "フライヤー",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Afterglow },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "パスパレのフライヤー",
                                 LocationName = "フライヤー",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.PastelPalettes },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "Roseliaのフライヤー",
                                 LocationName = "フライヤー",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.Roselia },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "ハロハピのフライヤー",
                                 LocationName = "フライヤー",
                                TargetTypes = new List<Card.Type>(),
                                TargetBands = new List<Card.Band>{ Card.Band.HelloHappyWorld },
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (60, 60, 60), (70, 70, 70), (80, 80, 80), (90, 90, 90), (100, 100, 100), (110, 110, 110) },
                                Level = 0,
                            },
                        },
                    },

                    #endregion

                    #region 流星堂

                    new OkimonoArea { Name = "流星堂",
                        AreaItems = new List<Okimono> {
                            // センタースペース
                            new Okimono {
                                Name = "噴水",
                                 LocationName = "センタースペース",
                                TargetTypes = new List<Card.Type> { Card.Type.Pure },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "足湯",
                                 LocationName = "センタースペース",
                                TargetTypes = new List<Card.Type> { Card.Type.Cool },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "ミッシェルの銅像",
                                 LocationName = "センタースペース",
                                TargetTypes = new List<Card.Type> { Card.Type.Happy },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "ヤシの木",
                                 LocationName = "センタースペース",
                                TargetTypes = new List<Card.Type> { Card.Type.Powerful },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "盆栽セット",
                                 LocationName = "センタースペース",
                                TargetTypes = new List<Card.Type> { Card.Type.Cool, Card.Type.Happy, Card.Type.Powerful, Card.Type.Pure },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (5, 5, 5), (10, 10, 10), (15, 15, 15), (20, 20, 20), (25, 25, 25) },
                                Level = 0,
                            },
                        },
                    },

                    #endregion

                    #region カフェテリア

                    new OkimonoArea { Name = "カフェテリア",
                        AreaItems = new List<Okimono> {
                            // おすすめメニュー
                            new Okimono {
                                Name = "ミートソースパスタ",
                                 LocationName = "おすすめメニュー",
                                TargetTypes = new List<Card.Type> { Card.Type.Pure },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "アサイーボウル",
                                 LocationName = "おすすめメニュー",
                                TargetTypes = new List<Card.Type> { Card.Type.Cool },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "マカロンタワー",
                                 LocationName = "おすすめメニュー",
                                TargetTypes = new List<Card.Type> { Card.Type.Happy },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "フルーツタルト",
                                 LocationName = "おすすめメニュー",
                                TargetTypes = new List<Card.Type> { Card.Type.Powerful },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (10, 10, 10), (30, 30, 30), (50, 50, 50), (70, 70, 70), (100, 100, 100), (120, 120, 120) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "チョココロネ",
                                 LocationName = "おすすめメニュー",
                                TargetTypes = new List<Card.Type> { Card.Type.Cool, Card.Type.Happy, Card.Type.Powerful, Card.Type.Pure },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (5, 5, 5), (10, 10, 10), (15, 15, 15), (20, 20, 20), (25, 25, 25) },
                                Level = 0,
                            },
                            new Okimono {
                                Name = "極上コーヒー",
                                 LocationName = "おすすめメニュー",
                                TargetTypes = new List<Card.Type> { Card.Type.Cool, Card.Type.Happy, Card.Type.Powerful, Card.Type.Pure },
                                TargetBands = new List<Card.Band>(),
                                Bonus = new List<(int performance, int technique, int visual)>()
                                    { (0, 0, 0), (5, 5, 5), (10, 10, 10), (15, 15, 15), (20, 20, 20), (25, 25, 25) },
                                Level = 0,
                            },
                        },},

                    #endregion
                };

                // 置物情報を保存する
                File.XmlSerializer.SaveToBinaryFile(_areaDataLocal, _areaDataLocal.GetType(), _okimonoInfoPath);
            }

            // ファイルから保存情報を読み取る
            var areaList = File.XmlSerializer.LoadFromBinaryFile(typeof(List<OkimonoArea>), _okimonoInfoPath) as IEnumerable<OkimonoArea>;

            if (areaList != null)
                _areas = new List<OkimonoArea>(areaList);
            else
            {
                System.Windows.MessageBox.Show(
                    "置物情報が保存されていません.",
                    "置物設定未保存");
                _areas = new List<OkimonoArea>();
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
            // カード情報を保存する
            File.BinarySerializer.SaveToBinaryFile(this.Cards, _cardInfoPath);
            // 置物情報を保存する
            File.XmlSerializer.SaveToBinaryFile(_areas, _areas.GetType(), _okimonoInfoPath);

            base.CloseViewCommandImplement(o);
        }

        #endregion

        #region コマンド

        private ICommand showSpecTeamCommand;
        public ICommand ShowSpecializedTeamViewCommand => showSpecTeamCommand ?? (showSpecTeamCommand = new DelegateCommand(ShowSpecializedTeamView, null));

        /// <summary>
        /// 特化編成ビューをモーダレスで開く
        /// </summary>
        private void ShowSpecializedTeamView(object o)
        {
            // モーダレスで特化編成ウィンドウ表示
            var vm = new ViewModels.SpecializedTeamViewModel(_cards.ToList(), _areas);
            App.ViewManager.ShowModelessView<Views.SpecializedTeamWindow>(vm, this);
        }

        private ICommand saveOwnedCommand;
        public ICommand SaveOwnedCommand => saveOwnedCommand ?? (saveOwnedCommand = new DelegateCommand(SaveOwnedInfo, null));

        /// <summary>
        /// 所持情報をファイルに保存する
        /// </summary>
        private void SaveOwnedInfo(object o)
        {
            // XMLファイルに上書きするだけ
            File.BinarySerializer.SaveToBinaryFile(this.Cards, _cardInfoPath);
        }

        private ICommand showTeamupCommand;
        public ICommand ShowTeamUpViewCommand => showTeamupCommand ?? (showTeamupCommand = new DelegateCommand(ShowTeamUpView, null));

        /// <summary>
        /// 最適編成ビューをモーダレスで開く
        /// </summary>
        private void ShowTeamUpView(object o)
        {
            // モーダレスで最適編成ウィンドウ表示
            var vm = new ViewModels.TeamUpViewModel(_cards.ToList(), _areas);
            App.ViewManager.ShowModelessView<Views.TeamUpWindow>(vm, this);
        }

        private ICommand showOkimonoCommand;
        public ICommand ShowOkimonoViewCommand => showOkimonoCommand ?? (showOkimonoCommand = new DelegateCommand(ShowOkimonoView, null));

        /// <summary>
        /// 置物設定ビューをモーダレスで開く
        /// </summary>
        private void ShowOkimonoView(object o)
        {
            // モーダレスで設定ウィンドウ表示
            var vm = new ViewModels.OkimonoViewModel(_areas);
            App.ViewManager.ShowModelessView<Views.OkimonoWindow>(vm, this);
        }

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
            var newCard4Urls = await GetNewCardUrlsAsync(_rare4url, _rare4selector, _rare4IconSelector, isCleanUpdate, cancelToken); // 新規星4
            var newCard3Urls = await GetNewCardUrlsAsync(_rare3url, _rare3selector, _rare3IconSelector, isCleanUpdate, cancelToken); // 新規星3
            var newCardUrls = newCard4Urls.Union(newCard3Urls).ToList();

            // ローカルとサーバのカード一覧が同じであれば更新しない
            if (newCardUrls.Count == 0)
            {
                this.ShowUpdatedCountMessage(newCardUrls.Count);
                return;
            }

            // 新規カード情報を取得
            Util.DirectoryUtil.CreateDirectoryIfNeeded(_cardIconBeforeDir);
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
        private async Task<IEnumerable<(string, string)>> GetNewCardUrlsAsync(string url, string selector, string iconSelector, bool isCleanUpdate, System.Threading.CancellationToken cancelToken)
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
            var iconInfos = doc.QuerySelectorAll(iconSelector).ToList();

            if (isCleanUpdate)
            {
                return infos
                    .Select((info, index) => 
                        ((info as AngleSharp.Html.Dom.IHtmlAnchorElement).Href, 
                        (iconInfos[2 * index] as AngleSharp.Html.Dom.IHtmlImageElement).Source))
                    .ToList();
            }
            else
            {
                var newCardUrls = new List<(string, string)>();

                foreach (var (info, index) in infos.WithIndex())
                {
                    // カードタイトル(主キー)がローカルに存在しなかったら
                    if (_cards.Any(card => card.Title == info.TextContent.InnerBracket()) == false)
                    {
                        // カード情報ページと画像アイコンのURLを追加
                        var cardUrl = (info as AngleSharp.Html.Dom.IHtmlAnchorElement).Href;
                        var iconUrl = (iconInfos[2 * index] as AngleSharp.Html.Dom.IHtmlImageElement).Source;
                        if (cardUrl != null && iconUrl != null)
                            newCardUrls.Add((cardUrl, iconUrl));
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
        /// 画像を指定URLからダウンロードしてパスへ出力する
        /// </summary>
        /// <param name="imgUrl">ダウンロードする画像のURL</param>
        /// <param name="outputPath">ローカルの出力パス</param>
        private async void DownloadImageAsync(string imgUrl, string outputPath)
        {
            var res = await _client.GetAsync(
                imgUrl,
                HttpCompletionOption.ResponseContentRead);

            // リソースを出力パスへ出力
            using (var fileStream = System.IO.File.Create(outputPath))
            using (var httpStream = await res.Content.ReadAsStreamAsync())
                httpStream.CopyTo(fileStream);
        }

        /// <summary>
        /// URL群からそれぞれのカード情報をスクレイピングする
        /// </summary>
        /// <param name="urls">スクレイピング先のURL群</param>
        private async Task<IEnumerable<Card>> GetCardsInfoAsync(IEnumerable<(string cardUrl, string iconUrl)> urls, System.Threading.CancellationToken cancelToken)
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
        private async Task<Card> GetCardInfoAsync((string cardUrl, string iconUrl) url, System.Threading.CancellationToken cancelToken)
        {
            var doc = default(AngleSharp.Html.Dom.IHtmlDocument);

            using (var stream = await _client.GetStreamAsync(new Uri(url.cardUrl)))
            {
                var parser = new AngleSharp.Html.Parser.HtmlParser();
                doc = await parser.ParseDocumentAsync(stream, cancelToken);
            }

            // サイトの表からカード情報を取得
            var items = doc.QuerySelectorAll("table > tbody > tr td, table > tbody > tr th");
            var name = items[5].TextContent.Replace(" ", "");
            var title = doc.QuerySelector("h2 > span").TextContent.InnerBracket();

            // アイコン情報を取得しローカルにダウンロード
            var iconBeforePath =_cardIconBeforeDir + @"/" + name + "[" + title + "]" + ".png";
            this.DownloadImageAsync(url.iconUrl, iconBeforePath);

            // エピソード/メモリアル解放のステータス上昇値
            var rarity = items[1].TextContent.Count();
            var releaseUp = _episodeRelaseStatusUp[rarity - 1].episode + _episodeRelaseStatusUp[rarity - 1].memorial;

            return new Card
            {
                Name = name,
                Title = title,
                BandName = this.ConvertBandName(items[7].TextContent),
                Rarity = rarity,
                CardType = this.ConvertCardType(items[3].TextContent),

                MaxPerformance = int.Parse(items[11].TextContent) + releaseUp,
                MaxTechnique = int.Parse(items[13].TextContent) + releaseUp,
                MaxVisual = int.Parse(items[15].TextContent) + releaseUp,

                SkillName = items[16].TextContent,
                SkillEffect = items[17].TextContent,

                IconBeforePath = iconBeforePath,
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

        /// <summary>
        /// エリア情報 置物情報
        /// </summary>
        /// <remarks>
        ///     エリア情報が置物情報を含んでいる
        ///     置物ビューのビューモデルへ渡す
        /// </remarks>
        private List<OkimonoArea> _areas;

        #endregion
    }
}
