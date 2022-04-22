using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex numericRegex = new Regex("^[0-9]+$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex specialChars = new Regex("[^a-zA-Z0-9_]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex multipleUnderscores = new Regex("_+", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex multipleSpaces = new Regex(@"[ ]+", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex guidRegex = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex allowedFileSystemCharacters = new Regex(@"[^!#$%&'()+,\-\.;=@\[\]\^_`{}~ a-zA-Z0-9]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex allowedAlphabetCharacters = new Regex(@"[^A-Za-z0-9\/\-\?\:\(\)\.\,\'\+\s]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);


        public static string ToUnsecureString(this SecureString secureString)
        {
            if (secureString == null) throw new ArgumentNullException("secureString");

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ToSecureString(this string unsecureString)
        {
            if (unsecureString == null) return null;

            return ToSecureString(unsecureString.AsEnumerable());
        }

        public static SecureString ToSecureString(this IEnumerable<char> unsecureString)
        {
            if (unsecureString == null || !unsecureString.Any()) return null;

            var secStr = new SecureString();
            foreach (char c in unsecureString)
            {
                secStr.AppendChar(c);
            }

            secStr.MakeReadOnly();

            return secStr;
        }

        /// <summary>
        /// Checks if the inputString is null to apply the defaultValue.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>if null the defaultValue otherwise inputString</returns>
        public static string Default(this string inputString, string defaultValue)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return defaultValue;
            }

            return inputString;
        }

        /// <summary>
        /// Truncates the text.
        /// </summary>
        /// <param name="str">String to truncate.</param>
        /// <param name="len">Lenght of truncation.</param>
        /// <returns>The truncated text.</returns>
        public static string TruncateText(this string str, int len, bool truncateFromLastSpace = false)
        {
            if (len < str.Length)
            {
                str = str.Substring(0, len);

                if (truncateFromLastSpace)
                {
                    int len1 = str.LastIndexOf(" ");

                    if (len1 <= 0)
                    {
                        len1 = len;
                    }

                    str = str.Substring(0, len1) + " ...";
                }
            }

            return str;
        }

        /// <summary>
        /// Replaces the newline to a br html tag.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string NewLineToBr(this string source)
        {
            return Regex.Replace(source, @"\n", "<br />", RegexOptions.Multiline);
        }

        /// <summary>
        /// Makes the string safe to use as ID in HTML (by removing special characters and replacing them with underscores)
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string MakeIdSafe(this string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return multipleUnderscores.Replace(specialChars.Replace(input.Trim(), "_"), "_").Trim();
        }

        /// <summary>
        /// Makes the filename or directory name, filesystem safe. Do not use a full path as input, but just the name of the file or directory.
        /// </summary>
        /// <param name="fileName">Name of the file/directory.</param>
        /// <returns></returns>
        public static string MakeFileSystemSafe(this string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                return fileName;
            }

            if (fileName.Contains(System.IO.Path.DirectorySeparatorChar))
            {
                throw new ArgumentException("Don't include a full path, just use the filename or directory name as input");
            }

            return multipleUnderscores.Replace(allowedFileSystemCharacters.Replace(RemoveDiacritics(fileName.Trim()), "_"), "_").Trim();
        }

        /// <summary>
        /// Removes the diacritics.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The input value without diacritics.</returns>
        public static string RemoveDiacritics(this string input)
        {
            string inputFormD = input.Normalize(NormalizationForm.FormD);
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < inputFormD.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(inputFormD[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    output.Append(inputFormD[i]);
                }
            }

            return (output.ToString().Normalize(NormalizationForm.FormC));
        }

        public static string LimitCharacterSetToAlphabet(this string input, string replaceString = ".")
        {
            // Check if the string only contains the characters allowed for SEPA and otherwise replace them with a dot.
            // The following characters are allowed:
            // a b c d e f g h i j k l m n o p q r s t u v w x y z
            // A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
            // 0 1 2 3 4 5 6 7 8 9
            // / - ? : ( ) . , ‘ +
            // Space
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            return allowedAlphabetCharacters.Replace(input, replaceString);
        }

        /// <summary>
        /// Entropies the specified string value. Good to calculate the unqiueness of a password (the higher, the better).
        /// </summary>
        /// <param name="value">The password or other string value</param>
        /// <returns></returns>
        public static int Entropy(this string value)
        {
            HashSet<char> chars = new HashSet<char>(value);
            return chars.Count;
        }
    }
}
