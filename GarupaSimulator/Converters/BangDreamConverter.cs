using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Markup;

namespace GarupaSimulator.Converters
{
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
