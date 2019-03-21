using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarupaSimulator
{
    /// <summary>IEnumerableに関する拡張メソッド群</summary>
    public static class IEnumerableExtensions
    {
        /// <summary>Tを<see cref="System.Collections.Generic.IEnumerable{T}"/>に変換します。</summary>
        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            return Enumerable.Repeat(value, 1);
        }

        /// <summary>配列を<see cref="System.Collections.Generic.IEnumerable{T}"/>に変換します。</summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T[]> source)
        {
            var list = new List<T>();

            foreach (var item in source)
            {
                list.AddRange(item);
            }

            return list;
        }

        /// <summary>指定されたシーケンスにインデックスを付加</summary>
        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((t, index) => (t, index));
        }

        /// <summary>指定されたシーケンスの各要素に対して指定された処理を実行します。</summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>指定されたシーケンスがnullである。</summary>
        public static bool IsNull<T>(this IEnumerable<T> source)
        {
            return source == null;
        }

        /// <summary>指定されたシーケンスが空（要素数０）である。</summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        /// <summary>指定されたシーケンスがnullもしくは空（要素数０）である。</summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source.IsNull() || source.IsEmpty();
        }

        /// <summary>指定されたシーケンスを要素数毎に区切ってまとめたシーケンスとして取得する</summary>
        public static IEnumerable<IList<T>> Chunk<T>(this IEnumerable<T> source, int count)
        {
            var list = new List<T>(count);
            foreach (var item in source)
            {
                list.Add(item);

                if (list.Count == count)
                {
                    yield return list;

                    list = new List<T>(count);
                }
            }
        }
    }
}
