using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GarupaSimulator.WpfUtil
{
    public static class XamlDebug
    {
        // DebugとReleaseでのxaml表示内容切替
        // http://sourcechord.hatenablog.com/entry/2016/12/02/193950

        /// <summary>
        /// DebugモードのみVisibleとなるプロパティ
        /// </summary>
        /// <remarks>
        ///     コントロールのVisibilityプロパティに設定すると、
        ///     Debugビルド時のみ表示、という表示切替ができます。
        /// </remarks>
        public static Visibility IsDebugVisible
        {
#if DEBUG
            get { return Visibility.Visible; }
#else
            get { return Visibility.Collapsed; }
#endif
        }
    }
}
