using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace MadsKristensen.ExtensibilityTools.VsixManifest
{
    [Guid("f86b5aa5-733c-4e8f-8d3b-ea6e9b97b343")]
    public sealed class ResxFileGenerator : BaseCodeGeneratorWithSite
    {
        public const string Name = "VsixManifestGenerator";
        public const string Desription = "Automatically generates the .resx file based on the .vsixmanifest file values";

        Manifest _manifest;

        public override string GetDefaultExtension()
        {
            return ".resx";
        }

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            _manifest = VsixManifestParser.FromManifest(inputFileContent);

            var item = Dte.Solution.FindProjectItem(inputFileName);

            if (item != null)
            {
                GenerateIconFile(item);
                GenerateClassFile(item);
                SetBuildProperties(inputFileName, item);
            }

            return GenerateResource();
        }

        private void SetBuildProperties(string inputFileName, ProjectItem item)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                string uniqueName = item.ContainingProject.UniqueName;

                IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));

                IVsHierarchy hierarchy;
                solution.GetProjectOfUniqueName(uniqueName, out hierarchy);

                IVsBuildPropertyStorage buildPropertyStorage = hierarchy as IVsBuildPropertyStorage;

                if (buildPropertyStorage != null)
                {
                    string fullPath = Path.ChangeExtension(inputFileName, GetDefaultExtension());

                    uint itemId;
                    hierarchy.ParseCanonicalName(fullPath, out itemId);

                    buildPropertyStorage.SetItemAttribute(itemId, "MergeWithCTO", "true");
                    buildPropertyStorage.SetItemAttribute(itemId, "ManifestResourceName", "VSPackage");
                }
            }), DispatcherPriority.ApplicationIdle, null);
        }

        // TODO: This should use CodeModel
        private void GenerateClassFile(ProjectItem item)
        {
            string dir = Path.GetDirectoryName(InputFilePath);

            string csFilename = Path.ChangeExtension(InputFilePath, ".cs");

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {FileNamespace}");
            sb.AppendLine("{");
            sb.AppendLine("\tstatic class Vsix");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic const string Id = \"{_manifest.ID}\";");
            sb.AppendLine($"\t\tpublic const string Name = \"{_manifest.Name.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
            sb.AppendLine($"\t\tpublic const string Description = \"{_manifest.Description.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
            sb.AppendLine($"\t\tpublic const string Version = \"{_manifest.Version}\";");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            FileHelpers.WriteFile(csFilename, sb.ToString());

            item.ProjectItems.AddFromFile(csFilename);
        }

        private void GenerateIconFile(ProjectItem item)
        {
            if (string.IsNullOrEmpty(_manifest.Icon))
                return;

            string dir = Path.GetDirectoryName(InputFilePath);
            string icon = Path.Combine(dir, _manifest.Icon);

            string icoFilename = Path.ChangeExtension(InputFilePath, ".ico");

            FileHelpers.RemoveReadonlyFlagFromFile(icoFilename);

            using (var stream = new FileStream(icoFilename, FileMode.Create))
            {
                var image = Image.FromFile(icon);
                var bitmap = (Bitmap)ImageResize(image, 32, 32);
                Icon.FromHandle(bitmap.GetHicon()).Save(stream);
            }

            item.ProjectItems.AddFromFile(icoFilename);
        }

        public static Image ImageResize(Image SourceImage, Int32 NewHeight, Int32 NewWidth)
        {
            var bitmap = new Bitmap(NewWidth, NewHeight, SourceImage.PixelFormat);

            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed | bitmap.PixelFormat == PixelFormat.Format4bppIndexed | bitmap.PixelFormat == PixelFormat.Format8bppIndexed | bitmap.PixelFormat == PixelFormat.Undefined | bitmap.PixelFormat == PixelFormat.DontCare | bitmap.PixelFormat == PixelFormat.Format16bppArgb1555 | bitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                throw new NotSupportedException("Pixel format of the image is not supported.");
            }

            var graphicsImage = Graphics.FromImage(bitmap);

            graphicsImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphicsImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphicsImage.DrawImage(SourceImage, 0, 0, bitmap.Width, bitmap.Height);
            graphicsImage.Dispose();
            return bitmap;
        }

        private byte[] GenerateResource()
        {
            using (var writer = new StringWriter())
            using (var resx = new ResXResourceWriter(writer))
            {
                resx.AddAlias("System.Windows.Forms", new AssemblyName("System.Windows.Forms"));
                resx.AddResource(new ResXDataNode("110", _manifest.Name));
                resx.AddResource(new ResXDataNode("112", _manifest.Description));

                if (!string.IsNullOrEmpty(_manifest.Icon))
                {
                    string iconFileName = Path.GetFileNameWithoutExtension(InputFilePath) + ".ico";

                    var fileRef = new ResXFileRef(iconFileName, "System.Drawing.Icon, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                    resx.AddResource(new ResXDataNode("400", fileRef));
                }

                resx.Generate();
                writer.Flush();

                var sb = writer.GetStringBuilder();
                return Encoding.UTF8.GetBytes(sb.ToString());
            }
        }
    }
}
