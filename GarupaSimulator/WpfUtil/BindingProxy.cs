using System.Windows;

namespace GarupaSimulator.WpfUtil
{
    /// <summary>
    /// ViewModelのバインディングソースの代理クラス
    /// </summary>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// Freezableオブジェクトのインスタンスの生成
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        /// <summary>
        /// データコンテキスト用の代理プロパティ
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}