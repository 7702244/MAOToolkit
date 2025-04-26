using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MAOToolkit.Utilities
{
    public static class TextHelpers
    {
        private static readonly Regex PairedTags = new(@"<([A-Z][A-Z0-9]*)\b(?:(?!>).)*>(?:.*?)<\/\1>|(?:<!DOCTYPE(?:.*?)>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex PhpCodeBlock = new(@"<\?php.+?\?>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex BBCodeUrl = new(@"\[url\](.+?)\[\/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex BBCodeCode = new(@"\[code\](.+?)\[\/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex Words = new(@"[\wА-Яа-я]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Class Methods

        /// <summary>
        /// Removes from the string all characters except digits.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <returns></returns>
        public static string StripAllButDigits(string? str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }

            return RegularExpressions.NotDigits.Replace(str, String.Empty);
        }

        /// <summary>
        /// Returns the string between first occurrences of <paramref name="start"/> and <paramref name="end"/> strings.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <param name="start">Start string.</param>
        /// <param name="end">End string.</param>
        /// <returns></returns>
        public static string SubstringBetween(string? str, string start, string end)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }

            if (String.IsNullOrEmpty(start))
            {
                throw new ArgumentException("String can't be null or empty.", nameof(start));
            }

            if (String.IsNullOrEmpty(end))
            {
                throw new ArgumentException("String can't be null or empty.", nameof(end));
            }

            int startIndex = str.IndexOf(start, StringComparison.OrdinalIgnoreCase);

            // Check if the start string was found.
            if (startIndex >= 0)
            {
                startIndex += start.Length;

                int endIndex = str.IndexOf(end, startIndex, StringComparison.OrdinalIgnoreCase);
                if (endIndex > startIndex)
                {
                    return str.Substring(startIndex, endIndex - startIndex);
                }
            }

            return String.Empty;
        }
        
        /// <summary>
        /// Replaces visually similar characters with their Latin counterparts.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceVisualCyrToLat(string input)
        {
            // Use StringBuilder to work efficiently with strings.
            var output = new StringBuilder(input.Length);

            // Go through each character of the input string.
            foreach (char c in input)
            {
                // Use switch to save memory.
                output.Append(c switch
                {
                    'а' => 'a', 'А' => 'A',
                    /*'в' => 'b',*/ 'В' => 'B',
                    'е' => 'e', 'Е' => 'E',
                    'к' => 'k', 'К' => 'K',
                    /*'м' => 'm',*/ 'М' => 'M',
                    /*'н' => 'h',*/ 'Н' => 'H',
                    'о' => 'o', 'О' => 'O',
                    'р' => 'p', 'Р' => 'P',
                    'с' => 'c', 'С' => 'C',
                    /*'т' => 't',*/ 'Т' => 'T',
                    'у' => 'y', 'У' => 'Y',
                    'х' => 'x', 'Х' => 'X',
                    _ => c // If the character is not found, we return it unchanged.
                });
            }

            return output.ToString();
        }
        
        /// <summary>
        /// Replaces visually similar characters with their Cyrillic counterparts.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceVisualLatToCyr(string input)
        {
            // Use StringBuilder to work efficiently with strings.
            var output = new StringBuilder(input.Length);

            // Go through each character of the input string.
            foreach (char c in input)
            {
                // Use switch to save memory.
                output.Append(c switch
                {
                    'a' => 'а', 'A' => 'А',
                    /*'b' => 'в',*/ 'B' => 'В',
                    'e' => 'е', 'E' => 'Е',
                    'k' => 'к', 'K' => 'К',
                    /*'m' => 'м',*/ 'M' => 'М',
                    /*'h' => 'н',*/ 'H' => 'Н',
                    'o' => 'о', 'O' => 'О',
                    'p' => 'р', 'P' => 'Р',
                    'c' => 'с', 'C' => 'С',
                    /*'t' => 'т',*/ 'T' => 'Т',
                    'y' => 'у', 'Y' => 'У',
                    'x' => 'х', 'X' => 'Х',
                    _ => c // If the character is not found, we return it unchanged.
                });
            }

            return output.ToString();
        }

        /// <summary>
        /// Returns words in the case dependent on the given number.
        /// </summary>
        /// <param name="number">The number on which the selected word depends.</param>
        /// <param name="nominativ">The nominative case of a word. Example: "день".</param>
        /// <param name="genetiv">The genitive case of the word. Example: "дня".</param>
        /// <param name="plural">The plural of a word. Example: "дней".</param>
        /// <returns></returns>
        public static string GetDeclension(int number, string nominativ, string genetiv, string plural)
        {
            number %= 100;

            if (number is >= 11 and <= 19)
            {
                return plural;
            }

            switch (number % 10)
            {
                case 1:
                    return nominativ;
                case 2:
                case 3:
                case 4:
                    return genetiv;
                default:
                    return plural;
            }
        }

        /// <summary>
        /// Displays bytes in readable format.
        /// </summary>
        /// <param name="byteCount">Source bytes count.</param>
        /// <returns></returns>
        public static string BytesToString(long byteCount)
        {
            string[] suffix = ["B", "KB", "MB", "GB", "TB", "PB", "EB"]; //Longs run out around EB

            if (byteCount == 0)
                return "0 " + suffix[0];

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + " " + suffix[place];
        }

        /// <summary>
        /// Returns a substring from the start of <paramref name="str"/> no
        /// longer than <paramref name="length"/>.
        /// Returning only whole words is favored over returning a string that
        /// is exactly <paramref name="length"/> long.
        /// </summary>
        /// <param name="str">The original string from which the substring
        /// will be returned.</param>
        /// <param name="length">The maximum length of the substring.</param>
        /// <param name="nonWordCharacters">Characters that, while not whitespace,
        /// are not considered part of words and therefor can be removed from a
        /// word in the end of the returned value.
        /// Defaults to ",", ".", ":" and ";" if null.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="length"/> is negative
        /// </exception>
        public static string CropWholeWords(string? str, int length, HashSet<char>? nonWordCharacters = null)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }

            if (length < 0)
            {
                throw new ArgumentException("Negative values not allowed.", nameof(length));
            }

            if (length >= str.Length)
            {
                return str;
            }
            
            nonWordCharacters ??= [',', '.', ':', ';'];

            int end = length;

            for (int i = end; i > 0; i--)
            {
                if (Char.IsWhiteSpace(str[i]))
                {
                    break;
                }

                if (nonWordCharacters.Contains(str[i])
                    && (str.Length == i + 1 || str[i + 1] == ' '))
                {
                    // Removing a character that isn't whitespace but not part 
                    // of the word either (ie ".") given that the character is 
                    // followed by whitespace or the end of the string makes it
                    // possible to include the word, so we do that.
                    break;
                }

                end--;
            }

            if (end == 0)
            {
                // If the first word is longer than the length we favor 
                // returning it as cropped over returning nothing at all.
                end = length;
            }

            return String.Concat(str.AsSpan(0, end), "…");
        }
        
        /// <summary>
        /// Gets the maximum number of first words from the text, truncates long words and adds "…" to the end if there was a truncation.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <param name="maxWords">Maximum number of words in result string. 0 - without limit.</param>
        /// <param name="maxWordLength">Maximum length of each word in result string. 0 - without limit.</param>
        /// <param name="truncateWith">The string that will be added when text or a word is truncated. Defaults: "…".</param>
        /// <param name="additionalDelimiters">Additional word delimiters.</param>
        /// <returns>In the output line: line translations and all additional word delimiters remain as in the original text.</returns>
        public static string TruncateText(
            string? str, 
            int maxWords = 0, 
            int maxWordLength = 0,
            string truncateWith = "…", 
            string additionalDelimiters = ",.;:!?()[]{}")
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }
            
            return new string(IterateChars().ToArray());

            bool IsSeparator(char c) => Char.IsSeparator(c) || additionalDelimiters.Contains(c);

            IEnumerable<char> IterateChars()
            {
                yield return str[0];
                
                int words = 1;
                int wordLength = 0;

                for (int i = 1; i < str.Length; i++)
                {
                    if (IsSeparator(str[i]) && !IsSeparator(str[i - 1]))
                    {
                        if (words == maxWords)
                        {
                            // Avoid three dots when abbreviating words and shortening the line.
                            if (maxWordLength > 0 && wordLength >= maxWordLength)
                            {
                                break;
                            }
                            
                            if (!String.IsNullOrEmpty(truncateWith))
                            {
                                foreach (char c in truncateWith)
                                    yield return c;
                            }

                            break;
                        }

                        words++;
                        wordLength = 0;
                    }
                    else
                    {
                        wordLength++;
                    }

                    if (maxWordLength > 0 && wordLength >= maxWordLength)
                    {
                        if (wordLength == maxWordLength)
                        {
                            if (!String.IsNullOrEmpty(truncateWith))
                            {
                                foreach (char c in truncateWith)
                                    yield return c;
                            }
                        }
                    }
                    else
                    {
                        yield return str[i];
                    }
                }
            }
        }

        /// <summary>
        /// Checks the string for spam.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <param name="spamWords">Source list of spam words.</param>
        /// <param name="spamWordsScore">The maximum number of occurrences of spam words. 0 - do not check.</param>
        /// <param name="urlWordsScore">The maximum number of occurrences URL links. 0 - do not check.</param>
        public static bool IsSpam(string? str, IEnumerable<string> spamWords, int spamWordsScore = 3, int urlWordsScore = 1)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            
            // If the links are more than urlWordsScore, it is spam.
            if (urlWordsScore > 0 && RegularExpressions.URL.Matches(str).Count >= urlWordsScore)
            {
                return true;
            }
            
            if (spamWordsScore <= 0 || !spamWords.Any())
            {
                return false;
            }
            
            // If spamWords is greater than spamWordsScore, it is spam.
            int score = Words.Matches(str)
                .Count(match => spamWords.Any(word => match.Value.Contains(word, StringComparison.OrdinalIgnoreCase)));

            return score >= spamWordsScore;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string CanonizeEmailString(string? str, bool removeInvalid = false)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }

            try
            {
                var emails = new HashSet<string>(str.Split(new[] { ';', ',', '/', '\\', '|', '&' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

                if (removeInvalid)
                {
                    emails.RemoveWhere(x => !IsValidEmail(x));
                }

                return String.Join(',', emails);
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// Returns a copy of string converted to HTML markup.
        /// </summary>
        public static string ToHtml(string? s) => ToHtml(s, false);

        /// <summary>
        /// Returns a copy of string converted to HTML markup.
        /// </summary>
        /// <param name="s">Source string.</param>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        public static string ToHtml(string? s, bool nofollow)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            // Protect from HtmlEncode.
            s = RegularExpressions.URL.Replace(s, "[url]$&[/url]");
            s = PairedTags.Replace(s, "[code]$&[/code]");
            s = PhpCodeBlock.Replace(s, "[code]$&[/code]");

            s = WebUtility.HtmlEncode(s);

            s = BBCodeCode.Replace(s, "<code>$1</code>");

            if (nofollow)
            {
                s = BBCodeUrl.Replace(s, "<a href=\"$1\" rel=\"nofollow\" target=\"_blank\">$1</a>");
            }
            else
            {
                s = BBCodeUrl.Replace(s, "<a href=\"$1\" target=\"_blank\">$1</a>");
            }

            // Replace new lines for different OS.
            s = s.Replace("\r\n", "\n").Replace('\r', '\n');

            // Replace tabulation.
            s = s.Replace("\t", "&emsp;");

            string[] paragraphs = s.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();

            foreach (string par in paragraphs)
            {
                builder.Append("<p>");

                string p = par.Replace("\n", "<br />");

                builder.Append(p);
                builder.Append("</p>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Decodes unicode characters in string.
        /// </summary>
        public static string UnicodeDecode(string? s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            return RegularExpressions.UnicodeString.Replace(s,
                m => Char.ToString((char)UInt16.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));
        }

        /// <summary>
        /// Encodes string to unicode characters.
        /// </summary>
        public static string UnicodeEncode(string? s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            byte[] stringBytes = Encoding.Unicode.GetBytes(s);
            char[] stringChars = Encoding.Unicode.GetChars(stringBytes);
            var builder = new StringBuilder();
            Array.ForEach(stringChars, c => builder.AppendFormat("\\u{0:X4}", (int)c));
            return builder.ToString();
        }

        /// <summary>
        /// Creates SHA1 hash with salt from a string
        /// </summary>
        /// <returns>Hash as string.</returns>
        public static string GetHMACSHA1Hash(string input, string secret)
        {
            return HMACSHA1.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(input)).Aggregate(String.Empty, (s, e) => s + e.ToString("x2"));
        }

        /// <summary>
        /// Creates SHA256 hash with salt from a string
        /// </summary>
        /// <returns>Hash as string.</returns>
        public static string GetHMACSHA256Hash(string input, string secret)
        {
            return HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(input)).Aggregate(String.Empty, (s, e) => s + e.ToString("x2"));
        }

        /// <summary>
        /// Creates MD5 hash from a string
        /// </summary>
        /// <returns>Hash as string.</returns>
        public static string GetMd5Hash(string input)
        {
            return MD5.HashData(Encoding.UTF8.GetBytes(input)).Aggregate(String.Empty, (s, e) => s + e.ToString("x2"));
        }

        /// <summary>
        /// Creates SHA1 hash from a string
        /// </summary>
        /// <returns>Hash as string.</returns>
        public static string GetSHA1Hash(string input)
        {
            return SHA1.HashData(Encoding.UTF8.GetBytes(input)).Aggregate(String.Empty, (s, e) => s + e.ToString("x2"));
        }

        /// <summary>
        /// Creates SHA256 hash from a string
        /// </summary>
        /// <returns>Hash as string.</returns>
        public static string GetSha256Hash(string input)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(input)).Aggregate(String.Empty, (s, e) => s + e.ToString("x2"));
        }

        /// <summary>
        /// Performs the ROT13 character rotation.
        /// </summary>
        public static string Rot13Transform(string? value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }

            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = array[i];

                if (number is >= 'a' and <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number is >= 'A' and <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        /// <summary>
        /// Gets the name from the DisplayAttribute of the selected property or its name.
        /// </summary>
        public static string GetDisplayName<T>(Expression<Func<T, object?>> expression)
        {
            MemberInfo? memberInfo = null;

            var propertyExpression = expression.Body;

            var memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                if (propertyExpression is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                memberInfo = memberExpr.Member;
            }

            if (memberInfo == null)
            {
                throw new ArgumentException("No property reference expression was found.", nameof(expression));
            }

            return memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? memberInfo.Name;
        }
        
        /// <summary>
        /// Gets fullpath of the selected property.
        /// </summary>
        public static string GetPropertyPath<T>(Expression<Func<T, object?>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var path = new StringBuilder();
            var current = expression.Body;

            // Handle type conversion (e.g., when property is a value type)
            if (current is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                current = unary.Operand;
            }

            // Traverse the chain of MemberExpressions
            while (current is MemberExpression member)
            {
                if (path.Length > 0)
                {
                    path.Insert(0, ".");
                }

                path.Insert(0, member.Member.Name);
                current = member.Expression;
            }

            // Verify we've reached the lambda parameter (x => x.Prop...)
            if (current is not ParameterExpression)
            {
                throw new ArgumentException("The expression is not a valid member access expression.", nameof(expression));
            }

            return path.ToString();
        }

        #endregion
    }
}