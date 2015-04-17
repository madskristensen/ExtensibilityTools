using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    static class ExtensionMethods
    {
        public static bool HasString(this string text, params string[] items)
        {
            foreach (string item in items)
            {
                if (text.IndexOf(item, StringComparison.OrdinalIgnoreCase) > -1)
                    return true;
            }

            return false;
        }
    }
}
