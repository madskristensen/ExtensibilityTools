using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using MadsKristensen.ExtensibilityTools.Settings;
using MadsKristensen.ExtensibilityTools.VSCT;
using MadsKristensen.ExtensibilityTools.VSCT.Generator;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace MadsKristensen.ExtensibilityTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Extensibility Tools", "General", 101, 101, true, new[] { "pkgdef", "vsct" })]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(GuidList.guidExtensibilityToolsPkgString)]
    public sealed class ExtensibilityToolsPackage : Package
    {
        public const string Version = "0.1";
        public static Options Options;

        protected override void Initialize()
        {
            base.Initialize();

            Options = (Options)GetDialogPage(typeof(Options));
            AddCustomToolCommand.Initialize(this);
        }
    }
}
