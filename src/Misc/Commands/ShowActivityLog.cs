using System;
using System.Linq;
using System.IO;
using System.Text;
using EnvDTE;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using EnvDTE80;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class ShowActivityLog : BaseCommand
    {
        private ShowActivityLog(IServiceProvider serviceProvider)
       : base(serviceProvider)
        {
        }

        public static ShowActivityLog Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new ShowActivityLog(provider);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdActivityLog, OpenBrowser, BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            /* Nothing to do here */
        }

        private void OpenBrowser(object sender, EventArgs e)
        {
            string url = GetFilePath();
            var dte = GetService<DTE2, DTE>();

            dte.ItemOperations.Navigate(url, vsNavigateOptions.vsNavigateOptionsNewWindow);
        }

        private string GetFilePath()
        {
            var shell = GetService<IVsShell, SVsShell>();
            object root;

            // Gets the version number with the /rootsuffix. Example: "14.0Exp"
            if (shell.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out root) == VSConstants.S_OK)
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string version = Path.GetFileName(root.ToString());

                return Path.Combine(appData, "Microsoft\\VisualStudio", version, "ActivityLog.xml");
            }

            return null;
        }
    }
}
