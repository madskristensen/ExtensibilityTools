using System.Text.RegularExpressions;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class Variables
    {
        public static Regex Comment = new Regex(@"(^([\s]+)?(?<comment>;.+))|(?<comment>//.+)", RegexOptions.Compiled);
        public static Regex RegKey = new Regex(@"(\[)(?<path>[^\]]+)?(\])?", RegexOptions.Compiled);
        public static Regex String = new Regex(@"""(?<content>[^""]+)?""?", RegexOptions.Compiled);
        public static Regex Dword = new Regex(@"^([\s]+)?(?<dword>(@)|("")([^""]+)(""))(?<operator>([\s]+)?=)", RegexOptions.Compiled);
        public static Regex Keyword = new Regex(@"\$([^\$]+)\$|(?(?<==)([\s]+)?(dword|hex)(?=:))", RegexOptions.Compiled);
    }
}
