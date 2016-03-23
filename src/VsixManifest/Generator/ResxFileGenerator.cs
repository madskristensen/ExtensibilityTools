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

        void SetBuildProperties(string inputFileName, ProjectItem item)
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
            string csFilename = Path.ChangeExtension(InputFilePath, ".cs");

            string fileContent = GenerateClass();

            // Don't write if it didn't change.
            if (File.Exists(csFilename))
            {
                string current = File.ReadAllText(csFilename);

                if (current == fileContent)
                    return;
            }

            FileHelpers.WriteFile(csFilename, fileContent);

            item.ProjectItems.AddFromFile(csFilename);
        }

        private string GenerateClass()
        {
            int indentSize = Convert.ToInt32(Dte.Properties["TextEditor", "CSharp"].Item("TabSize").Value);
            bool useTabs = Convert.ToBoolean(Dte.Properties["TextEditor", "CSharp"].Item("InsertTabs").Value);

            Func<int, string> indent = amt => new string(useTabs ? '\t' : ' ', indentSize * amt);

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {FileNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"{indent(1)}static class Vsix");
            sb.AppendLine($"{indent(1)}{{");
            sb.AppendLine($"{indent(2)}public const string Id = \"{_manifest.ID}\";");
            sb.AppendLine($"{indent(2)}public const string Name = \"{_manifest.Name?.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
            sb.AppendLine($"{indent(2)}public const string Description = \"{_manifest.Description?.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
            sb.AppendLine($"{indent(2)}public const string Language = \"{_manifest.Language}\";");
            sb.AppendLine($"{indent(2)}public const string Version = \"{_manifest.Version}\";");
            sb.AppendLine($"{indent(2)}public const string Author = \"{_manifest.Author?.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
            sb.AppendLine($"{indent(2)}public const string Tags = \"{_manifest.Tags?.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
            sb.AppendLine($"{indent(1)}}}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private void GenerateIconFile(ProjectItem item)
        {
            if (string.IsNullOrEmpty(_manifest.Icon))
                return;

            string dir = Path.GetDirectoryName(InputFilePath);
            var src = new FileInfo(Path.Combine(dir, _manifest.Icon));

            if (!src.Exists)
                return;

            var dest = new FileInfo(Path.ChangeExtension(InputFilePath, ".ico"));

            // Don't proceed if previosly generated icon is newer than the source image
            if (dest.Exists && src.LastWriteTime < dest.LastWriteTime)
                return;

            FileHelpers.RemoveReadonlyFlagFromFile(dest);

            using (var stream = new FileStream(dest.FullName, FileMode.Create))
            {
                var image = Image.FromFile(src.FullName);
                Icon ico = PngIconFromImage(image, 32);
                ico.Save(stream);
            }

            item.ProjectItems.AddFromFile(dest.FullName);
        }

        public static Icon PngIconFromImage(Image img, int size = 16)
        {
            byte[] pngiconheader = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            using (Bitmap bmp = new Bitmap(img, new Size(size, size)))
            {
                byte[] png;
                using (var fs = new System.IO.MemoryStream())
                {
                    bmp.Save(fs, ImageFormat.Png);
                    fs.Position = 0;
                    png = fs.ToArray();
                }

                using (var fs = new MemoryStream())
                {
                    if (size >= 256) size = 0;
                    pngiconheader[6] = (byte)size;
                    pngiconheader[7] = (byte)size;
                    pngiconheader[14] = (byte)(png.Length & 255);
                    pngiconheader[15] = (byte)(png.Length / 256);
                    pngiconheader[18] = (byte)(pngiconheader.Length);

                    fs.Write(pngiconheader, 0, pngiconheader.Length);
                    fs.Write(png, 0, png.Length);
                    fs.Position = 0;
                    return new Icon(fs);
                }
            }
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
