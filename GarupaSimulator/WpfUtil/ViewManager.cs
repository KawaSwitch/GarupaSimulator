using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GarupaSimulator.WpfUtil
{
    /// <summary>
    /// Viewマネージャ
    /// 　ViewとViewModelの対応付けを管理する
    /// </summary>
    public class ViewManager
    {
        /// <summary>
        /// ビューモデルをキーにしたビューディクショナリ
        /// </summary>
        private Dictionary<Type, Type> _viewTable;

        /// <summary>
        /// コンストラクタ1
        /// </summary>
        public ViewManager()
        {
            _viewTable = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// コンストラクタ2
        /// </summary>
        /// <param name="vm_v">ViewModelとViewのペアの列挙</param>
        public ViewManager(IEnumerable<Tuple<Type, Type>> vm_v)
            : this()
        {
            Add(vm_v);
        }

        /// <summary>
        /// ViewModelとViewの関連を追加する
        /// </summary>
        /// <param name="viewModel">Type : ViewModelの型</param>
        /// <param name="view">Type : Viewの型</param>
        public void Add(Type viewModel, Type view)
        {
            if (_viewTable.ContainsKey(viewModel))
                _viewTable[viewModel] = view;
            else
                _viewTable.Add(viewModel, view);
        }

        /// <summary>
        /// ViewModelとViewの関連を列挙で追加する
        /// </summary>
        /// <param name="vm_v">ViewModelとViewのペアの列挙</param>
        public void Add(IEnumerable<Tuple<Type, Type>> vm_v)
        {
            foreach (var pair in vm_v)
                Add(pair.Item1, pair.Item2);
        }

        /// <summary>
        /// 指定したViewModelに対応するViewを生成する
        /// </summary>
        public Window CreateView(ViewModelBase viewModel)
        {
            // ViewModelに対応するViewが存在するか
            if (_viewTable.ContainsKey(viewModel.GetType()))
            {
                // View生成
                var viewType = _viewTable[viewModel.GetType()];
                var wnd = Activator.CreateInstance(viewType) as Window;

                // DataContextに対応のViewModelを設定
                if (wnd != null)
                    wnd.DataContext = viewModel;

                return wnd;
            }
            else
                return null;
        }

        /// <summary>
        /// 既に存在するWindowのコレクションから, 指定したViewModelに対応するWindowを返す
        /// </summary>
        public Window GetView(ViewModelBase viewModel)
        {
            if (_viewTable.ContainsKey(viewModel.GetType()))
            {
                var viewType = _viewTable[viewModel.GetType()];

                foreach (Window wnd in Application.Current.Windows)
                {
                    if (wnd.GetType() == viewType)
                        return wnd;
                }
            }

            return null;
        }

        /// <summary>
        /// ViewModelから対応するViewをモーダルで表示する
        /// </summary>
        /// <param name="viewModel">ViewModel</param>
        /// <param name="ownerViewModel">親となるViewModel</param>
        /// <param name="locate">表示位置</param>
        /// <param name="left">表示位置X座標(相対位置)</param>
        /// <param name="top">表示位置Y座標(相対座標)</param>
        public bool ShowModalView(ViewModelBase viewModel, ViewModelBase ownerViewModel,
            WindowStartupLocation locate = WindowStartupLocation.CenterOwner, double left = 0.0, double top = 0.0)
        {
            var view = CreateView(viewModel);

            // 表示位置設定
            {
                view.WindowStartupLocation = locate;

                if (locate == WindowStartupLocation.Manual)
                {
                    view.Left = left;
                    view.Top = top;
                }
            }

            if (view != null)
            {
                view.Owner = GetView(ownerViewModel);

                return
                    (view.ShowDialog() == true);
            }
            else
                return false;
        }

        /// <summary>
        /// ViewModelから対応するViewをモードレスで表示する
        /// 　※ ここで生成されたViewはコレクションに保持されるのでクローズ時にリムーブする必要がある
        /// </summary>
        /// <param name="viewModel">ViewModel</param>
        /// <param name="ownerViewModel">親となるViewModel</param>
        /// <param name="locate">表示位置</param>
        /// <param name="left">表示位置X座標(相対位置)</param>
        /// <param name="top">表示位置Y座標(相対座標)</param>
        public void ShowModelessView<TWindow>(ViewModelBase viewModel, ViewModelBase ownerViewModel,
            WindowStartupLocation locate = WindowStartupLocation.CenterOwner, double left = 0.0, double top = 0.0)
                where TWindow : Window
        {
            this.ShowModelessView<TWindow>(viewModel, ownerViewModel, null, locate, left, top);
        }

        /// <summary>
        /// ViewModelから対応するViewをモードレスで表示する
        /// 　※ ここで生成されたViewはコレクションに保持されるのでクローズ時にリムーブする必要がある
        /// </summary>
        /// <param name="closingAct">終了する前の処理</param>
        public void ShowModelessView<TWindow>(ViewModelBase viewModel, ViewModelBase ownerViewModel, Action closingAct,
            WindowStartupLocation locate = WindowStartupLocation.CenterOwner, double left = 0.0, double top = 0.0)
                where TWindow : Window
        {
            // すでにビューを開いていたら開かない
            if (Application.Current.Windows.OfType<TWindow>().Any())
                return;

            var view = CreateView(viewModel);

            // 表示位置設定
            {
                view.WindowStartupLocation = locate;
                if (locate == WindowStartupLocation.Manual)
                {
                    view.Left = left;
                    view.Top = top;
                }
            }

            if (view != null)
            {
                view.Owner = GetView(ownerViewModel);
                view.Show();
                if (closingAct != null)
                    view.Closing += (o, e) => closingAct();
                view.Closing += (o, e) => view.Owner = null;
            }
        }

        /// <summary>
        /// Windowタイプに対応するウィンドウをクローズする
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        public void CloseViewIfExist<TWindow>()
            where TWindow : Window
        {
            var view = Application.Current.Windows .OfType<TWindow>().FirstOrDefault();
            view?.Close();
        }
    }
}
