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

            button.Enabled = IsButtonVisible();
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

        public static bool IsButtonVisible()
        {
            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));

            foreach (Project project in GetProjects(solution))
            {
                try
                {
                    string dir = project.Properties.Item("FullPath")?.Value?.ToString();

                    if (!string.IsNullOrEmpty(dir))
                    {
                        string manifest = Path.Combine(dir, "source.extension.vsixmanifest");

                        if (File.Exists(manifest))
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                    // Ignore and continue
                }
            }

            return false;
        }

        public static IEnumerable<EnvDTE.Project> GetProjects(IVsSolution solution)
        {
            foreach (IVsHierarchy hier in GetProjectsInSolution(solution))
            {
                EnvDTE.Project project = GetDTEProject(hier);
                if (project != null)
                    yield return project;
            }
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution)
        {
            return GetProjectsInSolution(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            if (solution == null)
                yield break;

            IEnumHierarchies enumHierarchies;
            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies);
            if (enumHierarchies == null)
                yield break;

            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
            }
        }

        public static Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException(nameof(hierarchy));

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as Project;
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
