using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools
{
    sealed class AddImageManifestCommand : BaseCommand
    {
        private List<string> _selectedFiles = new List<string>();
        private string[] _allowed = { ".PNG", ".XAML" };

        private AddImageManifestCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public static AddImageManifestCommand Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new AddImageManifestCommand(provider);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdCreateImageManifest, Execute, BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand)sender;
            button.Visible = button.Enabled = false;

            var files = ProjectHelpers.GetSelectedFilePaths();

            if (!files.Any())
                return;

            _selectedFiles.Clear();
            _selectedFiles.AddRange(files.Where(f => _allowed.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)));

            button.Visible = button.Enabled = _selectedFiles.Any();
        }

        private void Execute(object sender, EventArgs e)
        {
            string manifestFileName;

            if (!TryGetFileName(Path.GetDirectoryName(_selectedFiles.First()), out manifestFileName))
                return;

            ProjectHelpers.CheckFileOutOfSourceControl(manifestFileName);

            var project = ProjectHelpers.DTE.Solution.FindProjectItem(_selectedFiles.First())?.ContainingProject;

            if (!TryGenerateManifest(project, manifestFileName))
                return;

            IncludeManifestInProjectAndVsix(project, manifestFileName);
            SetInputImagesAsResource(project);

            ProjectHelpers.DTE.ItemOperations.OpenFile(manifestFileName);
            ProjectHelpers.DTE.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
        }

        private void SetInputImagesAsResource(Project project)
        {
            foreach (var file in _selectedFiles.Where(f => Path.GetExtension(f).Equals(".png", StringComparison.OrdinalIgnoreCase)))
            {
                var item = ProjectHelpers.DTE.Solution.FindProjectItem(file);
                item.SetItemType("Resource");
            }
        }

        private static void IncludeManifestInProjectAndVsix(Project project, string fileName)
        {
            var item = project.AddFileToProject(fileName, "Content");

            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));

            IVsHierarchy hierarchy;
            solution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out hierarchy);

            IVsBuildPropertyStorage buildPropertyStorage = hierarchy as IVsBuildPropertyStorage;

            if (buildPropertyStorage != null)
            {
                uint itemId;
                hierarchy.ParseCanonicalName(fileName, out itemId);

                buildPropertyStorage.SetItemAttribute(itemId, "IncludeInVSIX", "true");
            }
        }

        private bool TryGenerateManifest(Project project, string fileName)
        {
            if (project == null)
                return false;

            try
            {
                string assembly = Assembly.GetExecutingAssembly().Location;
                string root = Path.GetDirectoryName(assembly);
                string toolsDir = Path.Combine(root, "ImageManifest\\Tools");

                string images = string.Join(";", _selectedFiles);
                string assemblyName = project.Properties.Item("AssemblyName").Value.ToString();
                string manifestName = Path.GetFileName(fileName);

                string args = $"/manifest:\"{manifestName}\" /resources:\"{images}\" /assembly:{assemblyName} /guidName:{Path.GetFileNameWithoutExtension(fileName)}Guid /rootPath:\"{project.GetRootFolder()}\\\"";

                var start = new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(fileName),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = Path.Combine(toolsDir, "ManifestFromResources.exe"),
                    Arguments = args
                };

                using (var p = new System.Diagnostics.Process())
                {
                    p.StartInfo = start;
                    p.Start();
                    p.WaitForExit();
                }

                return File.Exists(fileName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return false;
        }

        private bool TryGetFileName(string initialDirectory, out string fileName)
        {
            fileName = null;

            using (var dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = initialDirectory;
                dialog.FileName = "Monikers";
                dialog.DefaultExt = ".imagemanifest";
                dialog.Filter = "Image Manifest files | *.imagemanifest";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return false;

                fileName = dialog.FileName;
            }

            return true;
        }
    }
}
