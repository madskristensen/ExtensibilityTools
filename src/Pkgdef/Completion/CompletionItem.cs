using System.Collections.Generic;
using System.Linq;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class CompletionItem
    {
        private static List<CompletionItem> _dic = new List<CompletionItem>
        {
            {new CompletionItem("AppDataLocalFolder", "The subfolder under %LOCALAPPDATA% for this application.")},
            {new CompletionItem("AppName", "The qualified name of the application that is passed to the AppEnv.dll entry points. The qualified name consists of the application name, an underscore, and the class identifier (CLSID) of the application automation object, which is also recorded as the value of the ThisVersionDTECLSID setting in the project .pkgdef file.")},
            {new CompletionItem("BaseInstallDir", "The full path of the location where Visual Studio was installed.")},
            {new CompletionItem("CommonFiles", "The value of the %CommonProgramFiles% environment variable.")},
            {new CompletionItem("MyDocuments", "The full path of the My Documents folder of the current user.")},
            {new CompletionItem("PackageFolder", "The full path of the directory that contains the package assembly files for the application.")},
            {new CompletionItem("ProgramFiles", "The value of the %ProgramFiles% environment variable.")},
            {new CompletionItem("RootFolder", "The full path of the root directory of the application.")},
            {new CompletionItem("RootKey", "The root registry key for the application. By default the root is in HKEY_CURRENT_USER\\Software\\CompanyName\\ProjectName\\VersionNumber (when the application is running, _Config is appended to this key). It is set by the RegistryRoot value in the SolutionName.pkgdef file.")},
            {new CompletionItem("ShellFolder", "The full path of the location where Visual Studio was installed.")},
            {new CompletionItem("System", "The Windows\\system32 folder.")},
            {new CompletionItem("WinDir", "The Windows folder.")},
        };

        private CompletionItem(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public static IEnumerable<CompletionItem> Items
        {
            get { return _dic; }
        }

        public static CompletionItem GetCompletionItem(string name)
        {
            return _dic.SingleOrDefault(c => c.Name == name);
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
