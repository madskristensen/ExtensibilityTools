using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class ToggleVsipLogging : BaseCommand
    {
        const string _dword = "EnableVSIPLogging";
        Package _package;
        bool _isEnabled;

        private ToggleVsipLogging(Package package)
       : base(package)
        {
            _package = package;
        }

        public static ToggleVsipLogging Instance
        {
            get;
            private set;
        }

        public static void Initialize(Package package)
        {
            Instance = new ToggleVsipLogging(package);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdVsipLogging, OpenBrowser, BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;

            var rawValue = _package.UserRegistryRoot.OpenSubKey("General").GetValue(_dword, 0);
            int value;

            int.TryParse(rawValue.ToString(), out value);

            _isEnabled = value == 1;
            button.Text = (_isEnabled ? "Disable" : "Enable") + " VSIP Logging";
        }

        private void OpenBrowser(object sender, EventArgs e)
        {
            int value = _isEnabled ? 0 : 1;

            using (var key = _package.UserRegistryRoot.OpenSubKey("General", true))
            {
                key.SetValue(_dword, value);
            }

            if (UserWantsToRestart(!_isEnabled))
            {
                RestartVS();
            }
        }

        void RestartVS()
        {
            IVsShell4 shell = GetService<IVsShell4, SVsShell>();
            shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);
        }

        static bool UserWantsToRestart(bool willEnable)
        {
            string mode = willEnable ? "enabled" : "disabled";
            string text = $"VSIP Logging has now been {mode}, but will not take effect before Visual Studio has been restarted.\r\rDo you wish to restart now?";
            return MessageBox.Show(text, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
