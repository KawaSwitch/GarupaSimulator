using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GarupaSimulator.WpfUtil
{
    /// <summary>
    /// Viewに対するメッセージボックス表示通知のイベントパラメーター
    /// </summary>
    public class ShowMessageBoxEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="caption">メッセージボックスのタイトル : string</param>
        /// <param name="message">メッセージボックスに表示されるテキスト : string</param>
        public ShowMessageBoxEventArgs(string caption, string message, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            Caption = caption;
            Message = message;
            Buttons = buttons;
            Icon = icon;
            Result = MessageBoxResult.OK;
        }

        /// <summary>
        /// メッセージボックスのタイトル
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// メッセージボックスに表示するテキスト
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///  メッセージボックスに表示されるボタン
        /// </summary>
        public MessageBoxButton Buttons { get; private set; }

        /// <summary>
        /// メッセージボックスに表示されるアイコン
        /// </summary>
        public MessageBoxImage Icon { get; private set; }

        /// <summary>
        /// メッセージボックスからの戻り値
        ///     View側のイベントハンドラで値を設定する
        /// </summary>
        public MessageBoxResult Result { get; set; }

    }
}
