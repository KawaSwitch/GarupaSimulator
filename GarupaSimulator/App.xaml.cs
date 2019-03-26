using System;
using System.Windows;
using System.Globalization;
using System.Threading;
using GarupaSimulator.WpfUtil;

namespace GarupaSimulator
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ビューマネージャ
        /// </summary>
        private ViewManager _viewManager;

        /// <summary>
        /// ViewManagerを返す
        /// </summary>
        public static ViewManager ViewManager => (Current as App)._viewManager;


        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public App()
        {
            _viewManager = new ViewManager();

            ViewManager.Add(typeof(ViewModels.MainViewModel), typeof(Views.MainWindow));
            ViewManager.Add(typeof(ViewModels.OkimonoViewModel), typeof(Views.OkimonoWindow));
            ViewManager.Add(typeof(ViewModels.TeamUpViewModel), typeof(Views.TeamUpWindow));
            ViewManager.Add(typeof(ViewModels.SpecializedTeamViewModel), typeof(Views.SpecializedTeamWindow));
        }

        /// <summary>
        /// アプリケーション実行開始時の処理
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 言語情報設定(ja-JP:日本語 / en-US:米語)
            CultureInfo info = new CultureInfo("ja-JP");
            //CultureInfo info = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = info;
            Thread.CurrentThread.CurrentUICulture = info;
        }
    }
}
