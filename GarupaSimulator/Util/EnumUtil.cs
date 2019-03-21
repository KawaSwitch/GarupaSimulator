using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace GarupaSimulator.Util
{
    public static class EnumUtil
    {
        /// <summary>
        /// 設定したDescription属性を取得する
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            var desciptionString = attributes.Select(n => n.Description).FirstOrDefault();

            if (desciptionString != null)
                return desciptionString;

            return value.ToString();
        }
    }
}
