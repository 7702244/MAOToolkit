using System.ComponentModel;
using System.Text;

namespace MAOToolkit.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Repeats string <paramref name="n"/> times.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string Repeat(this string str, int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), "Negative values not allowed.");
            }

            return new StringBuilder(str.Length * n)
                            .AppendJoin(str, new string[n + 1])
                            .ToString();
        }

        /// <summary>
        /// Returns the right part of a character string with the specified number of characters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string str, int length)
        {
            if (length > 0 && str.Length > length)
            {
                return str.Substring(str.Length - length);
            }

            return str;
        }

        /// <summary>
        /// Returns the left part of a character string with the specified number of characters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string str, int length)
        {
            if (length > 0 && str.Length > length)
            {
                return str.Substring(0, length);
            }

            return str;
        }

        /// <summary>
        /// Removes all leading occurrences of a specified <paramref name="trimString"/> from the current String object.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimStart(this string str, string? trimString)
        {
            if (!String.IsNullOrEmpty(str) && !String.IsNullOrEmpty(trimString))
            {
                while (str.StartsWith(trimString))
                {
                    str = str.Substring(trimString.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// Removes all trailing occurrences of a specified <paramref name="trimString"/> from the current String object.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimEnd(this string str, string? trimString)
        {
            if (!String.IsNullOrEmpty(str) && !String.IsNullOrEmpty(trimString))
            {
                while (str.EndsWith(trimString))
                {
                    str = str.Substring(0, str.Length - trimString.Length);
                }
            }

            return str;
        }

        public static T? ToNullable<T>(this string s) where T : struct
        {
            var result = new T?();
            try
            {
                if (!String.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    var conv = TypeDescriptor.GetConverter(typeof(T));
                    result = (T?)conv.ConvertFrom(s);
                }
            }
            catch { }
            return result;
        }
    }
}