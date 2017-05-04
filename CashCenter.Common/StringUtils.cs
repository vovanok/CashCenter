using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CashCenter.Common
{
	public static class StringUtils
	{
		public const string DEFAULT_STRING_SUFFIX = "...";

		public static string FilledString(char symbol, int lineLength)
		{
			if (lineLength <= 0)
				return string.Empty;

			return new string(symbol, lineLength);
		}

		public static string FilledLeftFromContentString(string content, char symbol, int lineLength)
		{
			if (string.IsNullOrEmpty(content) || lineLength < 0)
				return string.Empty;

			if (content.Length >= lineLength)
				return CutString(content, lineLength);

			return new string(symbol, lineLength - content.Length) + content;
		}

		public static string CutString(string srcString, int cutLength, string suffix = DEFAULT_STRING_SUFFIX)
		{
			if (string.IsNullOrEmpty(srcString))
				return string.Empty;

			if (srcString.Length <= cutLength || string.IsNullOrEmpty(suffix))
				return srcString;

			if (suffix.Length > cutLength)
				return srcString;

			return srcString.Substring(0, cutLength - suffix.Length) + suffix;
		}

		public static string StringInCenter(string srcString, int lineLength)
		{
			if (srcString.Length >= lineLength)
				return CutString(srcString, lineLength);

			int spacesInLeft = (lineLength - srcString.Length) / 2;

			return new string(' ', spacesInLeft) + srcString + new string(' ', lineLength - spacesInLeft - srcString.Length);
		}

		public static string FilledBetweenContentsString(string content1, string content2, char symbol, int lineLength)
		{
			var resContent1 = content1 ?? string.Empty;
			var resContent2 = content2 ?? string.Empty;

			if (lineLength <= 0)
				return string.Empty;

			if (resContent1.Length + resContent2.Length > lineLength)
				return CutString(content1 + content2, lineLength);

			return resContent1 + new string(symbol, lineLength - resContent1.Length - resContent2.Length) + resContent2;
		}

        public static List<string> SplitStringByLines(string stringValue, int lineLength)
        {
            if (string.IsNullOrEmpty(stringValue))
                return new List<string>();

            if (lineLength <= 0)
                return new List<string>();

            var result = new List<string>();
            int curLineStart = 0;
            while (curLineStart < stringValue.Length)
            {
                result.Add(stringValue.Substring(curLineStart, Math.Min(lineLength, stringValue.Length - curLineStart)));
                curLineStart += lineLength;
            }

            return result;
        }

        public static IList<string> SplitStringWithSeparators(string stringValue)
        {
            var lines = stringValue.Split('|');
            var result = new List<string>();

            foreach(var line in lines)
            {
                result.AddRange(StringUtils.SplitStringByLines(line, Config.CheckPrinterMaxLineLength));
            }

            return result;
        }

        public static bool IsValidEmail(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return false;

            try
            {
                return Regex.IsMatch(inputString,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
