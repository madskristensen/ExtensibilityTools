using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE80;
using MadsKristensen.ExtensibilityTools.VsixManifest;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class PrepareForAppVeyor : BaseCommand
    {
        Package _package;

        private PrepareForAppVeyor(Package package)
       : base(package)
        {
            _package = package;
        }

        public static PrepareForAppVeyor Instance { get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new PrepareForAppVeyor(package);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidSolutionCmdSet, PackageIds.cmdAppVeyorPrepare, Execute, BeforeQueryStatus);
        }

        void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;

            string solutionRoot = Path.GetDirectoryName(DTE.Solution.FullName);
            bool appVeyorExist = File.Exists(Path.Combine(solutionRoot, "appveyor.yml"));

            button.Enabled = !appVeyorExist && PrepareForGitHub.IsButtonVisible();
        }

        async void Execute(object sender, EventArgs e)
        {
            if (!UserWantToProceed())
                return;

            string solutionRoot = Path.GetDirectoryName(DTE.Solution.FullName);
            var manifestFile = Directory.EnumerateFiles(solutionRoot, "*.vsixmanifest", SearchOption.AllDirectories).FirstOrDefault();
            var manifest = await VsixManifestParser.FromFileAsync(manifestFile);

            string assembly = Assembly.GetExecutingAssembly().Location;
            string root = Path.GetDirectoryName(assembly);
            string dir = Path.Combine(root, "Misc\\Resources\\Appveyor");

            foreach (var src in Directory.EnumerateFiles(dir))
            {
                string fileName = Path.GetFileName(src);
                string dest = Path.Combine(solutionRoot, fileName);

                if (!File.Exists(dest))
                {
                    var content = await PrepareForGitHub.ReplaceTokens(src, manifest);

                    string manifestCs = Path.ChangeExtension(manifestFile, ".cs");
                    string versionFile = "{source.extension.cs}";
                    string relative = manifestCs.Replace(solutionRoot, string.Empty).Trim('\\');
                    content = content.Replace(versionFile, relative);

                    if (File.Exists(manifestCs))
                        content = content.Replace("#- ps: Vsix-TokenReplacement", "- ps: Vsix-TokenReplacement");

                    File.WriteAllText(dest, content);

                    PrepareForGitHub.AddFileToSolutionFolder(dest, (Solution2)DTE.Solution);
                }
            }
        }

        public bool UserWantToProceed()
        {
            string message = @"This will add AppVeyor.yml to Solution Items.

Do you wish to continue?";

            return MessageBox.Show(message, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
