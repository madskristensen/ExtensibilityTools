using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MadsKristensen.ExtensibilityTools.VSCT.Signing
{
    /// <summary>
    /// Helper class to allow accessing files.
    /// </summary>
    static class DialogHelper
    {
        private static OpenFileDialog _openPackageDialog;
        private static OpenFileDialog _openCertificateDialog;

        /// <summary>
        /// Creates dialog to point to binary file.
        /// </summary>
        public static OpenFileDialog OpenPackageFile(string title, string initPath)
        {
            if (_openPackageDialog != null)
            {
                _openPackageDialog.InitialDirectory = GetSecureInitPath(initPath);
                return _openPackageDialog;
            }

            var openFile = new OpenFileDialog();
            openFile.InitialDirectory = GetSecureInitPath(initPath);
            openFile.Title = title;
            openFile.DefaultExt = ".exe";
            openFile.Filter = "VSIX Package|*.vsix;*.cab;*.msi|All files|*.*";

            openFile.FilterIndex = 0;
            openFile.CheckFileExists = true;
            openFile.CheckPathExists = true;

            return _openPackageDialog = openFile;
        }
        
        /// <summary>
        /// Creates dialog to point to certificate file.
        /// </summary>
        public static OpenFileDialog OpenCertificateFile(string title, string initPath)
        {
            if (_openCertificateDialog != null)
            {
                _openCertificateDialog.InitialDirectory = GetSecureInitPath(initPath);
                return _openCertificateDialog;
            }

            var openFile = new OpenFileDialog();
            openFile.InitialDirectory = GetSecureInitPath(initPath);
            openFile.Title = title;
            openFile.DefaultExt = ".pfx";
            openFile.Filter = "Certificate File|*.pfx|All files|*.*";

            openFile.FilterIndex = 0;
            openFile.CheckFileExists = true;
            openFile.CheckPathExists = true;

            return _openCertificateDialog = openFile;
        }

        private static string GetSecureInitPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (File.Exists(path))
                return Path.GetDirectoryName(path);

            if (Directory.Exists(path))
                return path;

            var parentPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parentPath) && Directory.Exists(parentPath))
                return parentPath;

            return null;
        }

        /// <summary>
        /// Opens Windows Explorer window with specified file selected.
        /// </summary>
        public static void StartExplorerForFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            // open Explorer window with specified file selected:
            Process.Start("Explorer.exe", "/select,\"" + path + "\"");
        }
    }
}
