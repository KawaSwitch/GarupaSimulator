using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GarupaSimulator.WpfUtil;

namespace GarupaSimulator.Views
{
    /// <summary>
    /// OkimonoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OkimonoWindow : Window
    {
        /// <summary>
        /// 置物ビューのデータコンテキスト（ビューモデル）
        /// </summary>
        public ViewModels.OkimonoViewModel ViewModel => this.DataContext as ViewModels.OkimonoViewModel;

        #region ctor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OkimonoWindow()
        {
            this.Setup();
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// 置物リストビューの選択が変更されたときのハンドラ
        /// </summary>
        private void ListViewOkimonos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ビューの選択アイテムをビューモデルへ渡す
            this.ViewModel.SelectedItem = listViewOkimonos.SelectedItem as Okimono;
        }
    }
}
