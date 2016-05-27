using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools
{
    sealed class AddImageManifestCommand : BaseCommand
    {
        private List<string> _selectedFiles = new List<string>();

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

            var allowed = new[] { ".PNG", ".XAML" };
            _selectedFiles.Clear();

            for (int i = 1; i <= ProjectHelpers.DTE.SelectedItems.Count; i++)
            {
                string file = ProjectHelpers.DTE.SelectedItems.Item(i).ProjectItem?.FileNames[1];

                if (!string.IsNullOrEmpty(file))
                {
                    string ext = Path.GetExtension(file);

                    if (!allowed.Contains(ext, StringComparer.OrdinalIgnoreCase))
                        return;

                    if (!_selectedFiles.Contains(file))
                        _selectedFiles.Add(file);
                }
            }

            button.Visible = button.Enabled = true;
        }

        private void Execute(object sender, EventArgs e)
        {
            if (!_selectedFiles.Any())
                return;

            var project = ProjectHelpers.DTE.Solution.FindProjectItem(_selectedFiles.First())?.ContainingProject;
            string fileName;

            if (!GetFileName(Path.GetDirectoryName(_selectedFiles.First()), out fileName))
                return;

            ProjectHelpers.CheckFileOutOfSourceControl(fileName);
            string rootFolder = project.GetRootFolder();

            if (!GenerateManifest(project, fileName))
                return;

            project.AddFileToProject(fileName, "Content");

            foreach (var file in _selectedFiles)
            {
                var item = ProjectHelpers.DTE.Solution.FindProjectItem(_selectedFiles.First());
                item.SetItemType("Resource");
            }

            ProjectHelpers.DTE.ItemOperations.OpenFile(fileName);
            ProjectHelpers.DTE.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
        }

        private bool GenerateManifest(Project project, string fileName)
        {
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

        private bool GetFileName(string initialDirectory, out string fileName)
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
