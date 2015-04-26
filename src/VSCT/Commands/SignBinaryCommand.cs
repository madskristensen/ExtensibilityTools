using System;
using System.IO;
using EnvDTE;
using MadsKristensen.ExtensibilityTools.VSCT.Signing;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class SignBinaryCommand : BaseCommand
    {
        private const string ExtensibilityProjectGuid = "{82b43b9b-a64c-4715-b499-d71e9ca2bd60}";
        private Project _project;

        public SignBinaryCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public static SignBinaryCommand Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new SignBinaryCommand(provider);
        }

        /// <summary>
        /// Overriden by child class to setup own menu commands and bind with invocation handlers.
        /// </summary>
        protected override void SetupCommands()
        {
            AddCommand(GuidList.guidExtensibilityToolsCmdSet, PackageCommands.cmdSignBinary, ShowSignBinaryUI, CheckForExtensibilityPackageFlavorBeforeQueryStatus);
        }

        private void ShowSignBinaryUI(object sender, EventArgs e)
        {
            var form = new SignForm(GetPackagePath(), BuildProjectToSign);
            form.ShowDialog();
        }

        private void BuildProjectToSign(object sender, EventArgs e)
        {
            Solution solution = DTE.Solution;
            var selectedItem = GetSelectedItem();
            Project project = selectedItem != null ? selectedItem.Object as Project : null;

            // build the whole solution or only the selected project the command was invoked against:
            if (project == null)
            {
                solution.SolutionBuild.Build(true);
            }
            else
            {
                // use the currently selected configuration and architecture:
                solution.SolutionBuild.BuildProject(solution.SolutionBuild.ActiveConfiguration.Name, project.UniqueName, true);
            }
        }

        private void CheckForExtensibilityPackageFlavorBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand) sender;
            button.Visible = false;

            UIHierarchyItem uiItem = GetSelectedItem();

            if (uiItem == null)
                return;

            _project = uiItem.Object as Project;
            if (_project == null)
                return;

            var solution = GetService<IVsSolution>();
            if (solution == null)
                return;

            IVsHierarchy projectHierarchy;
            ErrorHandler.ThrowOnFailure(solution.GetProjectOfUniqueName(_project.UniqueName, out projectHierarchy));

            var aggregatableProject = projectHierarchy as IVsAggregatableProject;
            if (aggregatableProject == null)
                return;

            string projectTypeGuids;
            ErrorHandler.ThrowOnFailure(aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids));

            button.Visible = projectTypeGuids != null && projectTypeGuids.IndexOf(ExtensibilityProjectGuid, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string GetPackagePath()
        {
            var activeConfiguration = _project != null && _project.ConfigurationManager != null ? _project.ConfigurationManager.ActiveConfiguration : null;
            if (activeConfiguration == null)
                return null;

            var properties = activeConfiguration.Properties;
            var outputPath = properties.Item("OutputPath").Value.ToString(); // the path set in project properties

            properties = _project.Properties;
            var projectFolder = properties.Item("FullPath").Value.ToString();
            var assemblyName = properties.Item("AssemblyName").Value.ToString();

            return GetTargetFullName(projectFolder, outputPath, assemblyName + ".vsix");
        }

        /// <summary>
        /// Gets the full path to the target outcome of specified Visual C++ project.
        /// </summary>
        private static string GetTargetFullName(string projectFolder, string outputPath, string targetName)
        {
            if (string.IsNullOrEmpty(targetName))
                return null;

            if (!string.IsNullOrEmpty(projectFolder))
            {
                projectFolder = projectFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            // The output folder can be anything, let's assume it's any of these patterns:
            // 1) "\\server\folder"
            // 2) "drive:\folder"
            // 3) "..\..\folder"
            // 4) "folder"
            // 5) ""
            if (string.IsNullOrEmpty(outputPath))
            {
                // 5) ""
                if (string.IsNullOrEmpty(projectFolder))
                    return targetName;
                return Path.Combine(projectFolder, targetName);
            }

            if (outputPath.Length >= 2 && outputPath[0] == Path.DirectorySeparatorChar && outputPath[1] == Path.DirectorySeparatorChar)
            {
                // 1) "\\server\folder"
                return Path.Combine(outputPath, targetName);
            }

            if (outputPath.Length >= 3 && outputPath[1] == Path.VolumeSeparatorChar && outputPath[2] == Path.DirectorySeparatorChar)
            {
                // 2) "drive:\folder"
                return Path.Combine(outputPath, targetName);
            }

            if (outputPath.StartsWith("..\\") || outputPath.StartsWith("../"))
            {
                // 3) "..\..\folder"
                while (outputPath.StartsWith("..\\") || outputPath.StartsWith("../"))
                {
                    outputPath = outputPath.Substring(3);
                    if (!string.IsNullOrEmpty(projectFolder))
                    {
                        projectFolder = Path.GetDirectoryName(projectFolder);
                    }
                }

                if (string.IsNullOrEmpty(projectFolder))
                    return Path.Combine(outputPath, targetName);
                return Path.Combine(projectFolder, outputPath, targetName);
            }

            // 4) "folder"
            if (string.IsNullOrEmpty(projectFolder))
                return Path.Combine(outputPath, targetName);
            return Path.Combine(projectFolder, outputPath, targetName);
        }
    }
}
