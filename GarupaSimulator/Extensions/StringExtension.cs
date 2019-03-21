using System.Text.RegularExpressions;

namespace GarupaSimulator
{
    /// <summary>
    /// System.Stringの拡張メソッド
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 丸括弧内の文字列を取得
        /// </summary>
        /// <returns>丸括弧内の文字列 ただし入れ子や複数の括弧への動作は未保証</returns>
        public static string InnerParenthesis(this string str)
        {
            string pattern = @"(\()(?<something>.*?)(\))";
            return Regex.Match(str, pattern).Groups["something"].Value;
        }

        /// <summary>
        /// 中括弧内の文字列を取得
        /// </summary>
        /// <returns>中括弧内の文字列 ただし入れ子や複数の括弧への動作は未保証</returns>
        public static string InnerCurlyBracket(this string str)
        {
            string pattern = @"(\{)(?<something>.*?)(\})";
            return Regex.Match(str, pattern).Groups["something"].Value;
        }

        /// <summary>
        /// 角括弧内(大括弧)の文字列を取得
        /// </summary>
        /// <returns>角括弧内の文字列 ただし入れ子や複数の括弧への動作は未保証</returns>
        public static string InnerBracket(this string str)
        {
            string pattern = @"(\[)(?<something>.*?)(\])";
            return Regex.Match(str, pattern).Groups["something"].Value;
        }
    }
}
