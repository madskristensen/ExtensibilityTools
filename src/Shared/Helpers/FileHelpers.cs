using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadsKristensen.ExtensibilityTools
{
    public static class FileHelpers
    {
        public static void WriteFile(string fileName, string content)
        {
            FileInfo file = new FileInfo(fileName);
            RemoveReadonlyFlagFromFile(fileName);

            File.WriteAllText(file.FullName, content);
        }

        public static void RemoveReadonlyFlagFromFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            RemoveReadonlyFlagFromFile(file);
        }

        public static void RemoveReadonlyFlagFromFile(FileInfo file)
        {
            if (file.Exists && file.IsReadOnly)
                file.IsReadOnly = false;
        }
    }
}
