using System;
using System.Windows.Interop;
using EnvDTE;
using MadsKristensen.ExtensibilityTools.Misc.Commands;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class ExportImageMoniker : BaseCommand
    {
        private ExportImageMoniker(IServiceProvider serviceProvider)
       : base(serviceProvider)
        {
        }

        public static ExportImageMoniker Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new ExportImageMoniker(provider);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdExportMoniker, ShowInformation, BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            /* Nothing to do here */
        }

        private void ShowInformation(object sender, EventArgs e)
        {
            var imageService = GetService<IVsImageService2, SVsImageService>();

            var dialog = new ImageMonikerDialog(imageService);

            var dte = GetService<DTE>();
            var hwnd = new IntPtr(dte.MainWindow.HWnd);
            System.Windows.Window window = (System.Windows.Window)HwndSource.FromHwnd(hwnd).RootVisual;
            dialog.Owner = window;

            dialog.ShowDialog();
        }
    }
}
