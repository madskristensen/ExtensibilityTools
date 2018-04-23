using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using Task = System.Threading.Tasks.Task;

namespace MadsKristensen.ExtensibilityTools
{
    public class PromptForAnalyzers
    {
        private const string _nugetId = "Microsoft.VisualStudio.SDK.Analyzers";
        private static SolutionEvents _solutionEvents;
        private static AsyncPackage _package;
        private static IComponentModel _componentModel;
        private static DTE2 _dte;
        private static HashSet<string> _history = new HashSet<string>();

        public static async Task InitializeAsync(AsyncPackage package)
        {
            _package = package;
            _componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            var events = _dte.Events as Events2;
            _solutionEvents = events.SolutionEvents;
            _solutionEvents.Opened += OnSolutionOpened;

            if (ShouldCheckInstallation(_dte.Solution))
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                StartCheckingForAnalyzersAsync().ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        private static bool ShouldCheckInstallation(Solution solution)
        {
            if (!ExtensibilityToolsPackage.Options.PromptForAnalyzers || string.IsNullOrEmpty(solution.FileName) || !solution.IsOpen)
            {
                return false;
            }

            if (_history.Contains(_dte.Solution?.FileName))
            {
                return false;
            }

            string configFile = GetConfigFilePath(solution);

            if (File.Exists(configFile))
            {
                return false;
            }

            return true;
        }

        private static void OnSolutionOpened()
        {
            if (ShouldCheckInstallation(_dte.Solution))
            {
                StartCheckingForAnalyzersAsync().ConfigureAwait(false);
            }
        }

        private static async Task StartCheckingForAnalyzersAsync()
        {
            var solution = await _package.GetServiceAsync(typeof(IVsSolution)) as IVsSolution;
            await CheckForInstalledAnalyzersAsync(solution).ConfigureAwait(false);
        }

        private static async Task CheckForInstalledAnalyzersAsync(IVsSolution solution)
        {
            IEnumerable<Project> projects = ProjectHelpers.GetAllProjectsInSolution(solution);
            var candidateProjects = new List<Project>();

            foreach (Project project in projects.Where(p => p.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")) // .NET project kind
            {
                if (project.IsExtensibilityProject(solution) && !IsPackageInstalled(project))
                {
                    candidateProjects.Add(project);
                }
            }

            _history.Add(_dte.Solution?.FileName);

            if (candidateProjects.Count == 0)
            {
                return;
            }

            await PromptToInstallAsync(candidateProjects).ConfigureAwait(false);

            string configFile = GetConfigFilePath(_dte.Solution);
            PackageUtilities.EnsureOutputPath(Path.GetDirectoryName(configFile));
            try
            {
                File.WriteAllText(configFile, "1");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static string GetConfigFilePath(Solution solution)
        {
            string dir = Path.GetDirectoryName(solution.FileName);
            return Path.Combine(dir, ".vs", "VSSDK.Analyzers.txt");
        }

        private static bool IsPackageInstalled(Project project)
        {
            try
            {
                IVsPackageInstallerServices installerServices = _componentModel.GetService<IVsPackageInstallerServices>();
                return installerServices.IsPackageInstalled(project, _nugetId);
            }
            catch
            {
                return false;
            }
        }

        private static async Task PromptToInstallAsync(IEnumerable<Project> projects)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            string msg = $"One or more VSIX projects in this solution aren't using the Microsoft.VisualStudio.SDK.Analyzers NuGet package.\r\rDo you wish to install it now?";

            MessageBoxResult answer = MessageBox.Show(msg, Vsix.Name, MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (answer == MessageBoxResult.OK)
            {
                _dte.StatusBar.Text = $"Installing {_nugetId}...";
                IVsPackageInstaller2 installer = _componentModel.GetService<IVsPackageInstaller2>();

                try
                {
                    foreach (Project project in projects)
                    {
                        installer.InstallLatestPackage(null, project, _nugetId, true, false);
                    }
                }
                catch (Exception ex)
                {
                    _dte.StatusBar.Text = ex.Message;
                }

                _dte.StatusBar.Text = $"VSSDK Analyzers installed successfully";
            }
        }
    }
}
