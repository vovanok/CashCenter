using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CashCenter.Common
{
	public static class StringUtils
	{
		public static string FilledString(string str, int lineLength)
		{
			if (lineLength <= 0)
				return string.Empty;

            var sb = new StringBuilder(str);
            while (sb.Length < lineLength)
                sb.Append(str);

            return sb.ToString().Substring(0, lineLength);
		}

		public static IEnumerable<string> StringInCenter(string srcString, int lineLength)
		{
            if (string.IsNullOrEmpty(srcString) || lineLength <= 0)
                return new List<string>();
            
            var lines = SplitStringByLines(srcString, lineLength);
            var lastLine = lines[lines.Count - 1];

            if (lastLine.Length < lineLength)
            {
                int spacesInLeft = (lineLength - lastLine.Length) / 2;
                lines[lines.Count - 1] = new string(' ', spacesInLeft) + lastLine + new string(' ', lineLength - spacesInLeft - srcString.Length);
            }

            return lines;
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

        public static string SeparatedLeftAndRight(string stringValue, char separator, int lineLength)
        {
            if (string.IsNullOrEmpty(stringValue) || lineLength <= 0)
                return string.Empty;

            var stringParts = stringValue.Split(separator);
            if (stringParts.Length != 2)
                return stringValue;

            int countSpaces = lineLength - stringParts[0].Length - stringParts[1].Length;
            if (countSpaces < 0)
                countSpaces = 0;

            return stringParts[0] + new string(' ', countSpaces) + stringParts[1];
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

        public static decimal ForseDecimalParse(string strValue)
        {
            decimal result;
            if (decimal.TryParse(strValue.Replace(',', '.'), out result))
                return result;

            if (decimal.TryParse(strValue.Replace('.', ','), out result))
                return result;

            return 0;
        }

        public static double ForseDoubleParse(string strValue)
        {
            double result;
            if (double.TryParse(strValue.Replace(',', '.'), out result))
                return result;

            if (double.TryParse(strValue.Replace('.', ','), out result))
                return result;

            return 0;
        }

        public static float ForseFloatParse(string strValue)
        {
            float result;
            if (float.TryParse(strValue.Replace(',', '.'), out result))
                return result;

            if (float.TryParse(strValue.Replace('.', ','), out result))
                return result;

            return 0;
        }
    }
}
