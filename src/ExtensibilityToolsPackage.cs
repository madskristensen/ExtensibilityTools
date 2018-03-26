using System;
using System.Runtime.InteropServices;
using System.Threading;
using MadsKristensen.ExtensibilityTools.ThemeColorsToolWindow;
using MadsKristensen.ExtensibilityTools.VSCT.Commands;
using MadsKristensen.ExtensibilityTools.VSCT.Generator;
using MadsKristensen.ExtensibilityTools.VsixManifest;
using MadsKristensen.ExtensibilityTools.VsixManifest.Commands;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Task = System.Threading.Tasks.Task;

namespace MadsKristensen.ExtensibilityTools
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), Vsix.Name, "General", 101, 102, true, new[] { "pkgdef", "vsct" })]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true, ProjectSystem = ProvideCodeGeneratorAttribute.VisualBasicProjectGuid)]
    [ProvideCodeGenerator(typeof(ResxFileGenerator), ResxFileGenerator.Name, ResxFileGenerator.Desription, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(PackageGuids.guidExtensibilityToolsPkgString)]
    [ProvideToolWindow(typeof(SwatchesWindow))]
    public sealed class ExtensibilityToolsPackage : AsyncPackage
    {
        private static Options _options;
        private static object _syncRoot = new object();

        public static Options Options
        {
            get
            {
                if (_options == null)
                {
                    lock (_syncRoot)
                    {
                        if (_options == null)
                        {
                            EnsurePackageLoaded();
                        }
                    }
                }

                return _options;
            }
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ProjectHelpers.InitializeAsync(this);
            await Logger.InitializeAsync(this, Vsix.Name);

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            _options = (Options)GetDialogPage(typeof(Options));

            // VSCT
            AddCustomToolCommand.Initialize(this);

            // Misc
            SignBinaryCommand.Initialize(this);
            ShowProjectInformation.Initialize(this);
            ExportImageMoniker.Initialize(this);
            SwatchesWindowCommand.Initialize(this);
            ShowActivityLog.Initialize(this);
            ToggleVsipLogging.Initialize(this);

            // Solution node
            PrepareForGitHub.Initialize(this);
            PrepareForAppVeyor.Initialize(this);

            // Vsix Manifest
            AddResxGeneratorCommand.Initialize(this);

            // Image Manifest
            AddImageManifestCommand.Initialize(this);
        }

        private static void EnsurePackageLoaded()
        {
            var shell = (IVsShell)GetGlobalService(typeof(SVsShell));

            if (shell.IsPackageLoaded(ref PackageGuids.guidExtensibilityToolsPkg, out IVsPackage package) != VSConstants.S_OK)
            {
                ErrorHandler.Succeeded(shell.LoadPackage(ref PackageGuids.guidExtensibilityToolsPkg, out package));
            }
        }
    }
}
