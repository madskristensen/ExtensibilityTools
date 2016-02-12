using System;
using System.Runtime.InteropServices;
using MadsKristensen.ExtensibilityTools.VSCT.Commands;
using MadsKristensen.ExtensibilityTools.VSCT.Generator;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using MadsKristensen.ExtensibilityTools.ThemeColorsToolWindow;
using MadsKristensen.ExtensibilityTools.VsixManifest;
using MadsKristensen.ExtensibilityTools.VsixManifest.Commands;

namespace MadsKristensen.ExtensibilityTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(ExtensibilityOptions), Name, "General", 101, 102, true, new[] { "pkgdef", "vsct" })]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true, ProjectSystem = ProvideCodeGeneratorAttribute.VisualBasicProjectGuid)]
    [ProvideCodeGenerator(typeof(ResxFileGenerator), ResxFileGenerator.Name, ResxFileGenerator.Desription, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [Guid(PackageGuids.guidExtensibilityToolsPkgString)]
    [ProvideToolWindow(typeof(SwatchesWindow))]
    public sealed class ExtensibilityToolsPackage : Package
    {
        public const string Version = "0.1";
        public const string Name = "Extensibility Tools";
        public static ExtensibilityOptions Options { get; private set; }

        protected override void Initialize()
        {
            Options = (ExtensibilityOptions)GetDialogPage(typeof(ExtensibilityOptions));

            // VSCT
            AddCustomToolCommand.Initialize(this);

            // Misc
            SignBinaryCommand.Initialize(this);
            ShowProjectInformation.Initialize(this);
            ExportImageMoniker.Initialize(this);
            SwatchesWindowCommand.Initialize(this);
            ShowActivityLog.Initialize(this);
            ToggleVsipLogging.Initialize(this);

            // Vsix Manifest
            AddResxGeneratorCommand.Initialize(this);

            base.Initialize();
        }
    }
}
