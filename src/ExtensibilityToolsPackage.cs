using System.Runtime.InteropServices;
using MadsKristensen.ExtensibilityTools.Settings;
using MadsKristensen.ExtensibilityTools.VSCT.Commands;
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
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.GeneratorName, VsctCodeGenerator.GeneratorDescription, true, ProjectSystem = ProvideCodeGeneratorAttribute.VisualBasicProjectGuid)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(GuidList.guidExtensibilityToolsPkgString)]
    public sealed class ExtensibilityToolsPackage : Package
    {
        public const string Version = "0.1";
        public static ExtensibilityToolsPackage Instance;
        private Options _options;

        public Options Options
        {
            get
            {
                if (_options == null)
                    _options = (Options)GetDialogPage(typeof(Options));

                return _options;
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            AddCustomToolCommand.Initialize(this);
            SignBinaryCommand.Initialize(this);

            base.Initialize();
        }
    }
}
