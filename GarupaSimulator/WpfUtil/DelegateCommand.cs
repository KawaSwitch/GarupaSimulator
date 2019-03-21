using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GarupaSimulator.WpfUtil
{
    /// <summary>
    /// ICommandの汎用実装クラス
    /// </summary>
    public class DelegateCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>ICommand.Executeのデリゲート</summary>
        private Action<object> ExecuteAction;

        /// <summary>ICommand.CanExecuteのデリゲート</summary>
        private Func<object, bool> CanExecuteAction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">実行処理</param>
        /// <param name="canExecute">実行可否</param>
        public DelegateCommand(Action<object> command, Func<object, bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentException("コマンドにはnullを指定できません");

            ExecuteAction = command;
            CanExecuteAction = canExecute;
            _isEnable = true;
        }

        /// <summary>
        /// ICommand.CanExecuteChangedの実装
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// ICommand.Executeの実装
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => ExecuteAction(parameter);

        /// <summary>
        /// ICommand.CanExecuteの実装
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (IsEnabled)
                return (CanExecuteAction != null) ? CanExecuteAction(parameter) : true;
            else
                return false;
        }

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

        private bool _isEnable;

        /// <summary>
        /// コマンドが有効であるか
        ///   他処理の実行中に一時的に無効化する場合などに使う
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnable; }
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }
    }
}
