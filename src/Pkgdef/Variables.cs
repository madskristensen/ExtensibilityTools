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
        public static Regex Guid = new Regex(@"{\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
