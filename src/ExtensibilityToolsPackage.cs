using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using MadsKristensen.ExtensibilityTools.Settings;

namespace MadsKristensen.ExtensibilityTools
{  
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Extensibility Tools", "General", 101, 101, true, new[] { "pkgdef" })]
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

            // Add our command handlers for menu (commands must exist in the .vsct file)
            //OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            //if ( null != mcs )
            //{
            //    CommandID menuCommandID = new CommandID(GuidList.guidExtensibilityToolsCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
            //    MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
            //    mcs.AddCommand( menuItem );
            //}
        }
    }
}
