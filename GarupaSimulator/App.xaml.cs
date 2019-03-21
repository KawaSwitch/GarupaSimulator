using System;
using System.Windows;
using System.Globalization;
using System.Threading;

namespace GarupaSimulator
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
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
