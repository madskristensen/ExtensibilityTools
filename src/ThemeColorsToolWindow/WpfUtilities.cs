using System.Runtime.InteropServices;
using System.Windows.Media;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.ThemeColorsToolWindow
{
    public static class ImageMonikerHelpers
    {
        public static ImageSource GetImage(this ImageMoniker imageMoniker)
        {
            var vsIconService = ServiceProvider.GlobalProvider.GetService(typeof(SVsImageService)) as IVsImageService2;

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags;
            imageAttributes.ImageType = (uint)_UIImageType.IT_Bitmap;
            imageAttributes.Format = (uint)_UIDataFormat.DF_WPF;
            imageAttributes.LogicalHeight = 16;//IconHeight,
            imageAttributes.LogicalWidth = 16;//IconWidth,
            imageAttributes.StructSize = Marshal.SizeOf(typeof(ImageAttributes));

            IVsUIObject result = vsIconService.GetImage(imageMoniker, imageAttributes);

            object data;
            result.get_Data(out data);
            ImageSource glyph = data as ImageSource;
            glyph.Freeze();

            return glyph;
        }
    }
}
