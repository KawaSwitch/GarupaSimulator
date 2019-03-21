using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GarupaSimulator.WpfUtil
{
    /// <summary>
    /// ViewModelの基底クラス
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INotifyDataErrorInfo 
        /// <summary>エラーの一覧</summary>
        readonly Dictionary<string, HashSet<string>> _currentErrors = new Dictionary<string, HashSet<string>>();

        public string this[string propertyName]
        {
            get
            {
                return propertyName + " :" + _currentErrors[propertyName];
            }
        }

        /// <summary>エラーの追加</summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        protected bool AddError(string propertyName, string error)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException(nameof(propertyName));
            if (string.IsNullOrEmpty(error))
                throw new ArgumentException(nameof(error));
            
            if (!_currentErrors.ContainsKey(propertyName))
                _currentErrors[propertyName] = new HashSet<string>();

            bool ret = _currentErrors[propertyName].Add(error);
            if (ret)
                RaiseErrorsChanged(propertyName);

            return ret;
        }

        /// <summary>エラーの破棄</summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        protected bool ResetError(string propertyName, string error)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException(nameof(propertyName));

            bool ret = false;
            if (_currentErrors.ContainsKey(propertyName))
                ret = _currentErrors.Remove(propertyName);

            if (ret)
                RaiseErrorsChanged(propertyName);

            return ret;
        }

        /// <summary>エラーの変更を発行する</summary>
        /// <param name="propertyName">プロパティの名前</param>
        /// <remarks>INotifyDataErrorInfo の補助</remarks>
        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null) 
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>エラー通知イベント</summary>
        /// <remarks>INotifyDataErrorInfo.ErrorsChanged の実装</remarks>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>INotifyDataErrorInfo.GetErrorsの実装</summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _currentErrors.Values.SelectMany(p => p).ToList().AsReadOnly();    //エンティティレベルでのエラー
            else if (_currentErrors.ContainsKey(propertyName))
                return _currentErrors[propertyName].ToList().AsReadOnly();
            else
                return Enumerable.Empty<string>();
        }

        /// <summary>エラーがあるか判定</summary>
        /// <remarks>INotifyDataErrorInfo.HasErrorsの実装</remarks>
        public bool HasErrors
        {
            get { return _currentErrors.Any(); }
        }

        #endregion


        /// <summary>
        /// INotifyPropertyChanged.PropertyChanged の実装 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// INotifyPropertyChanged.PropertyChangedイベントを発生させる。 
        /// </summary>
        /// <param name="propertyName">string : プロパティ名</param>
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// マウスカーソルを待機状態にする
        /// </summary>
        protected void SetMouseCursorWait()
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        }

        /// <summary>
        /// マウスカーソルを矢印状態(デフォルト)にする
        /// </summary>
        protected void SetMouseCursorDefault()
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        /// <summary>
        /// マウスカーソルを待機状態にして時間のかかる処理を実行する
        /// <para>１．マウスカーソルを待機状態にする</para>
        /// <para>２．処理（act）を実行する</para>
        /// <para>３．マウスカーソルを通常状態にする</para>
        /// </summary>
        protected void RunHeavyAction(Action act)
        {
            System.Windows.Application.Current.Dispatcher.InvokeIfRequired(this.SetMouseCursorWait);
            act();
            System.Windows.Application.Current.Dispatcher.InvokeIfRequired(this.SetMouseCursorDefault);
        }

        /// <summary>
        /// Viewに対するクローズ通知
        /// </summary>
        public event EventHandler<CloseViewEventArgs> CloseView;

        /// <summary>
        /// Viewに対するメッセージボックス表示通知
        /// </summary>
        public event EventHandler<ShowMessageBoxEventArgs> ShowMessageBox;

        /// <summary>
        /// クローズコマンド
        /// </summary>
        public DelegateCommand CloseViewCommand { get; private set; }



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewModelBase()
            : base()
        {
            // クローズビューコマンドの生成
            CloseViewCommand = new DelegateCommand(CloseViewCommandImplement, CanExecuteCloseViewCommand);
        }

        /// <summary>
        /// CanCloseView用のパラメーター
        /// </summary>
        public class CanCloseViewArgs
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="isModel">bool : モーダルウィンドウか？</param>
            /// <param name="dialogResult">bool? : DialogResult</param>
            public CanCloseViewArgs(bool isModel, bool? dialogResult)
            {
                IsModel = isModel;
                DialogResult = dialogResult;
            }

            /// <summary>
            /// Viewから渡されるパラメーター
            ///     Viewがモーダルであった場合にtrueとなる : bool
            /// </summary>
            public bool IsModel { get; private set; }

            /// <summary>
            /// Viewから渡されるパラメーター
            ///     Viewがモーダルであった場合に、戻り値として設定されるDialogResult値 : bool?
            /// </summary>
            public bool? DialogResult { get; private set; }
        }

        /// <summary>
        /// ClosedViewArgs用のパラメーター
        /// </summary>
        public class ClosedViewArgs
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="isModel">bool : モーダルウィンドウか？</param>
            /// <param name="dialogResult">bool? : DialogResult</param>
            public ClosedViewArgs(bool isModel, bool? dialogResult)
            {
                IsModel = isModel;
                DialogResult = dialogResult;
            }

            /// <summary>
            /// Viewから渡されるパラメーター
            ///     Viewがモーダルであった場合にtrueとなる : bool
            /// </summary>
            public bool IsModel { get; private set; }

            /// <summary>
            /// Viewから渡されるパラメーター
            ///     Viewがモーダルであった場合に、戻り値として設定されるDialogResult値 : bool?
            /// </summary>
            public bool? DialogResult { get; private set; }
        }

        /// <summary>
        /// Viewに対してクローズを通知する
        ///     正常にViewを閉じられた場合はTrueを返す。
        /// </summary>
        /// <param name="dialogResult">モーダルウィンドウ時のdialogResult値 : bool</param>
        /// <returns>結果値 : bool</returns>
        public bool InvokeCloseView(bool dialogResult = true)
        {
            if (CloseView != null)
            {
                var e = new CloseViewEventArgs(dialogResult);
                CloseView(this, e);
                return e.Success;
            }
            else
                return false;
        }

        /// <summary>
        /// ShowMessageBoxイベントを呼び出す。 
        /// </summary>
        /// <param name="caption">string : メッセージボックスのタイトル</param>
        /// <param name="message">string : メッセージボックスに表示されるテキスト</param>
        /// <param name="buttons">MessageBoxButton : メッセージボックスに表示されるボタン</param>
        /// <param name="icon">MessageBoxImage : メッセージボックスに表示されるアイコン</param>
        /// <returns>MessageBoxResult : メッセージボックスの戻り値</returns>
        public MessageBoxResult InvokeShowMessageBox(string caption, string message,
                                                     MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            if (ShowMessageBox != null)
            {
                var e = new ShowMessageBoxEventArgs(caption, message, buttons, icon);
                ShowMessageBox(this, e);
                return e.Result;
            }
            else
                return MessageBoxResult.None;
        }

        /// <summary>
        /// ユーザーからの確認入力
        /// メッセージに対して肯定(OK or Yes)ならTrue、それ以外ならFalseを返す。
        /// </summary>
        public virtual bool UserConfirm(string caption, string message, MessageBoxButton buttons = MessageBoxButton.YesNo,
                                        MessageBoxImage icon = MessageBoxImage.Information)
        {
            var result = InvokeShowMessageBox(caption, message, buttons, icon);

            return 
                (result == MessageBoxResult.OK || result == MessageBoxResult.Yes);
        }

        /// <summary>
        /// アラートを表示させる
        /// </summary>
        public void Alert(string caption, string message, MessageBoxImage icon = MessageBoxImage.Information)
        {
            InvokeShowMessageBox(caption, message, MessageBoxButton.OK, icon);
        }

        /// <summary>
        /// CloseViewCommandの実体
        /// </summary>
        /// <param name="o">object : コマンドパラメーター</param>
        protected virtual void CloseViewCommandImplement(object o)
        {
            InvokeCloseView((o is bool) ? (bool)o : true);
        }

        /// <summary>
        /// CloseViewCommandの実行可否
        /// </summary>
        /// <param name="o">object : コマンドパラメーター</param>
        /// <returns>bool</returns>
        protected virtual bool CanExecuteCloseViewCommand(object o)
        {
            return true;
        }

        /// <summary>
        /// Viewを閉じても良いか？
        ///     ViewでWindowExtension.Setup()が呼ばれた場合、Window.Closing()イベント内からコールされる。
        ///     閉じて良ければtureを返す。
        /// </summary>
        /// <param name="arg">Viewから状態通知パラメーター</param>
        /// <returns>戻り値 : bool</returns>
        public virtual bool CanCloseView(CanCloseViewArgs arg)
        {
            return true;
        }

        /// <summary>
        /// Viewが閉じられた
        ///     ViewでWindowExtension.Setup()が呼ばれた場合、Window.Closed()イベント内からコールされる。
        /// </summary>
        public virtual void ClosedView(ClosedViewArgs arg)
        {
            // イベントハンドラをクリアする
            CloseView = null;
            ShowMessageBox = null;
        }
    }
}
