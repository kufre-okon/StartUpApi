using System;
using System.Collections.Generic;
using System.Linq;


namespace StartUpApi.Utility
{
    public static class Helper
    {
        #region strings
        /// <summary>
        /// Convert to base64 representation
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(byteValue);
        }

        /// <summary>
        /// Convert base64 encoded string to string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FromBase64(string str)
        {
            byte[] byteValue = Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(byteValue);
        }

        public static string GetHash(string input)
        {
            var hashAlgorithm = System.Security.Cryptography.SHA256.Create();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static byte[] GetBytes(string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
        }


        #endregion

        public static T? GetValueOrNull<T>(string value) where T : struct
        {
            try
            {
                object Value = value;
                if ((Value is DBNull | string.IsNullOrWhiteSpace(value)))
                    return null;
                else
                    return (T)Convert.ChangeType(Value, typeof(T));
            }
            catch
            {
                return null;
            }
        }

        public static T GetValueOrDefault<T>(string value)
        {
            try
            {
                object Value = value;
                if ((Value is DBNull | string.IsNullOrWhiteSpace(value)))
                    return default(T);
                else
                    return (T)Convert.ChangeType(Value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        #region Enumerable

        public static IEnumerable<TSource> RecursiveSelect<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector)
        {
            return RecursiveSelect(source, childSelector, element => element);
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
           Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, TResult> selector)
        {
            return RecursiveSelect(source, childSelector, (element, index, depth) => selector(element));
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
           Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, int, TResult> selector)
        {
            return RecursiveSelect(source, childSelector, (element, index, depth) => selector(element, index));
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
           Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, int, int, TResult> selector)
        {
            return RecursiveSelect(source, childSelector, selector, 0);
        }

        private static IEnumerable<TResult> RecursiveSelect<TSource, TResult>(this IEnumerable<TSource> source,
           Func<TSource, IEnumerable<TSource>> childSelector, Func<TSource, int, int, TResult> selector, int depth)
        {
            return source.SelectMany((element, index) => Enumerable.Repeat(selector(element, index, depth), 1)
               .Concat(RecursiveSelect(childSelector(element) ?? Enumerable.Empty<TSource>(),
                  childSelector, selector, depth + 1)));
        }

        #endregion

        #region decimals
        public static decimal GetDecimal(string input)
        {
            return Decimal.Parse(input.Replace("\"", ""), System.Globalization.NumberStyles.Currency);
        }
        #endregion
    }
}
