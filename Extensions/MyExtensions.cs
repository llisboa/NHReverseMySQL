using System.Globalization;
using System.Text.RegularExpressions;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static string ToSentenseCase(this String str)
        {
            var pos = 0;
            while (pos != -1)
            {
                pos = str.IndexOf("_");
                if (pos != -1)
                {
                    str = str.Remove(pos, 2).Insert(pos, str.Substring(pos + 1, 1).ToUpper());
                }
            }

            return str.Remove(0, 1).Insert(0, str.Substring(0, 1).ToUpper());
        }
    }
}