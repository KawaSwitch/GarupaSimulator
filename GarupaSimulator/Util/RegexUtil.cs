using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GarupaSimulator.Util
{
    public static class RegexUtil
    {
        /// <summary>
        /// 括弧内の文字列を取得
        ///     入れ子は非対応
        /// </summary>
        public static string GetParenthesisEnclosedString(string str)
        {
            string pattern = @"(\()(?<something>.*?)(\))";
            return Regex.Match(str, pattern).Groups["something"].Value;
        }

        /// <summary>
        /// 角括弧内の文字列を取得
        ///     入れ子は非対応
        /// </summary>
        public static string GetBracketEnclosedString(string str)
        {
            string pattern = @"(\[)(?<something>.*?)(\])";
            return Regex.Match(str, pattern).Groups["something"].Value;
        }
    }
}
