using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Linq;

namespace GarupaSimulator.Converters
{
    /// <summary>
    /// イベントタイプの文字列コンバータ
    /// </summary>
    [ValueConversion(typeof(Event.HeldType), typeof(string))]
    public class EventTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attribute = (Event.HeldType)value;
            return Util.EnumUtil.GetDescription(attribute);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 属性の文字列コンバータ
    /// </summary>
    [ValueConversion(typeof(Card.Type), typeof(string))]
    public class AttributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attribute = (Card.Type)value;
            return Util.EnumUtil.GetDescription(attribute);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 属性と属性画像のコンバータ
    /// </summary>
    [ValueConversion(typeof(Card.Type), typeof(Uri))]
    public class AttributeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attribute = (Card.Type)value;
            return new Uri(@"pack://application:,,,/Resources/AttributeIcons/" + Util.EnumUtil.GetDescription(attribute) + ".png");
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 数値とVisiblityのコンバータ
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntegerToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value > 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 文字列群と改行文字列のコンバータ
    /// </summary>
    [ValueConversion(typeof(IEnumerable<string>), typeof(string))]
    public class StringLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var src = value as IEnumerable<string>;
            if (src == null)
                return null;
            return string.Join(Environment.NewLine, src);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            var src = value as string;
            if (string.IsNullOrEmpty(src) == true)
                return new List<string>();
            return src.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }
    }

    /// <summary>
    /// 所属バンドのコンバータ
    /// </summary>
    [ValueConversion(typeof(Card.Band), typeof(string))]
    public class BandTypeConverter : IValueConverter
    {
        /// <summary>
        /// 所属バンドの型変換 Card.Band => string
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is Card.Band)
                return Util.EnumUtil.GetDescription((Card.Band)value);
            else
                return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// 所属バンドの型変換 string => Card.Band
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is string)
            {
                if ((string)value == Properties.BangDreamResources.PoppinParty)
                    return Card.Band.PoppinParty;
                else if ((string)value == Properties.BangDreamResources.Afterglow)
                    return Card.Band.Afterglow;
                else if ((string)value == Properties.BangDreamResources.PastelPalettes)
                    return Card.Band.PastelPalettes;
                else if ((string)value == Properties.BangDreamResources.Roselia)
                    return Card.Band.Roselia;
                else if ((string)value == Properties.BangDreamResources.HelloHappyWorld)
                    return Card.Band.HelloHappyWorld;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
