using System.Linq;

namespace Unity.Multiplayer.Tools.Common
{
    internal static class StringUtil
    {
        internal static string AddSpacesToCamelCase(string s)
        {
            return string.Concat(s.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }

        internal static string RemoveSpaces(string s)
        {
            return s.Replace(" ", "");
        }
    }
}