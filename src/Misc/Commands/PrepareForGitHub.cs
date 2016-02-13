using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using MadsKristensen.ExtensibilityTools.VsixManifest;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class PrepareForGitHub : BaseCommand
    {
        Package _package;
        static readonly string[] _visible = { "CHANGELOG.md", "README.md" };

        private PrepareForGitHub(Package package)
       : base(package)
        {
            _package = package;
        }

        public static PrepareForGitHub Instance { get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new PrepareForGitHub(package);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidSolutionCmdSet, PackageIds.cmdGitHubPrepare, Execute, BeforeQueryStatus);
        }

        void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;

            var solutionHasVsixProjects = ProjectHelpers.GetAllProjectsInSolution().Any(p => p.IsExtensibilityProject());

            button.Enabled = button.Visible = solutionHasVsixProjects;
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
            string dir = Path.Combine(root, "Misc\\Resources\\GitHub");

            foreach (var src in Directory.EnumerateFiles(dir))
            {
                string fileName = Path.GetFileName(src);
                string dest = Path.Combine(solutionRoot, fileName);

                if (!File.Exists(dest))
                {
                    var content = await ReplaceTokens(src, manifest);

                    File.WriteAllText(dest, content);

                    if (_visible.Contains(fileName))
                        AddFileToSolutionFolder(dest, (Solution2)DTE.Solution);
                }
            }
        }

        public bool UserWantToProceed()
        {
            string message = @"This will add some files to the solution folder and add some of them to Solution Items.

The files are:

  .gitattributes
  .gitignore
  CHANGELOG.md
  CONTRIBUTING.md
  LICENSE
  README.md

Files that already exist will not be overridden. Do you wish to continue?";

            return MessageBox.Show(message, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void AddFileToSolutionFolder(string file, Solution2 solution)
        {
            Project currentProject = null;

            foreach (Project project in solution.Projects)
            {
                if (project.Kind == EnvDTE.Constants.vsProjectKindSolutionItems && project.Name == "Solution Items")
                {
                    currentProject = project;
                    break;
                }
            }

            if (currentProject == null)
                currentProject = solution.AddSolutionFolder("Solution Items");

            currentProject.ProjectItems.AddFromFile(file);
        }

        public static async System.Threading.Tasks.Task<string> ReplaceTokens(string file, Manifest manifest)
        {
            using (var reader = new StreamReader(file))
            {
                var content = await reader.ReadToEndAsync();

                if (manifest != null)
                {
                    var properties = typeof(Manifest).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        var value = property.GetValue(manifest);

                        if (value != null)
                            content = content.Replace("{" + property.Name + "}", value.ToString());
                    }
                }

                return content;
            }
        }
    }
}
