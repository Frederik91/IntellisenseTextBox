using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartTextBox
{
    public static class PathVariableUtils
    {
        public static Regex VariableRegex { get; } = new Regex(@"\${{.+?}}");
        public static Regex VariableNameRegex { get; } = new Regex(@"(?<=\${{).*(?=}})");

        public static string ExpandVariables(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = Environment.ExpandEnvironmentVariables(input);
            input = Regex.Replace(input, "%date%", DateTime.Now.ToString(@"yyyy-MM-dd"), RegexOptions.IgnoreCase);
            return Regex.Replace(input, "%time%", DateTime.Now.ToString(@"HH-mm-ss"), RegexOptions.IgnoreCase);
        }

        public static string FormatAsVariable(string variable)
        {
            if (string.IsNullOrWhiteSpace(variable))
                return variable;

            return "${{ " + variable.Trim() + " }}";
        }

        public static bool ContainsVariable(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && VariableRegex.IsMatch(text);
        }

        public static List<string> GetVariables(string documentPath)
        {
            return string.IsNullOrWhiteSpace(documentPath) ? new List<string>() : VariableRegex.Matches(documentPath).OfType<Match>().Select(x => VariableNameRegex.Match(x.Value).Value.Trim()).ToList();
        }
    }
}