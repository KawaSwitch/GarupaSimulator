﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.IO;

namespace GarupaSimulator.Converters
{
    /// <summary>
    /// 画像のコンバータ
    /// </summary>
    [ValueConversion(typeof(string), typeof(System.Windows.Media.ImageSource))]
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // 絶対パスを取得（WPFのImageSourceらへんは相対パスがややこしい）
                var path = Path.GetFullPath((string)value);

                using (var fs = new FileStream(path, FileMode.Open))
                {
                    // 画像表示中にファイルをロックしないように画像をメモリにキャッシュする
                    var decoder = BitmapDecoder.Create(
                        fs,
                        BitmapCreateOptions.None,
                        BitmapCacheOption.OnLoad);

                    var bmp = new WriteableBitmap(decoder.Frames[0]);
                    bmp.Freeze();
                    return bmp;
                }
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// カード属性のコンバータ
    /// </summary>
    [ValueConversion(typeof(Card.Type), typeof(string))]
    public class CardTypeConverter : IValueConverter
    {
        /// <summary>
        /// カード属性の型変換 Card.Type => string
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is Card.Type)
                return Util.EnumUtil.GetDescription((Card.Type)value);
            else
                return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// カード属性の型変換 string => Card.Type
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is string)
            {
                if ((string)value == Properties.CardInfoResources.PowerfulType)
                    return Card.Type.Powerful;
                else if ((string)value == Properties.CardInfoResources.CoolType)
                    return Card.Type.Cool;
                else if ((string)value == Properties.CardInfoResources.PureType)
                    return Card.Type.Pure;
                else if ((string)value == Properties.CardInfoResources.HappyType)
                    return Card.Type.Happy;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
