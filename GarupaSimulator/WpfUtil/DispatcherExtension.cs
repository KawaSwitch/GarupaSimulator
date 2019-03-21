using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GarupaSimulator.WpfUtil
{
    public static class DispatcherExtension
    {
        /// <summary>
        /// スレッドを確認して実行
        /// </summary>
        /// <param name="act">実行関数</param>
        /// <remarks>
        ///     メソッド呼び出し元がUIスレッド上ならそのまま
        ///     ワーカースレッド上ならディスパッチを実行します
        /// </remarks>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action act)
        {
            if (dispatcher == null)
                return;

            if (!dispatcher.CheckAccess())
                dispatcher.BeginInvoke(act, DispatcherPriority.Send);
            else
                act();
        }
    }
}
