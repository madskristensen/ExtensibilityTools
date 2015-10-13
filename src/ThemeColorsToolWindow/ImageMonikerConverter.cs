using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualStudio.Imaging.Interop;

namespace MadsKristensen.ExtensibilityTools.ThemeColorsToolWindow
{
    public class ImageMonikerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageMoniker moniker = (ImageMoniker)parameter;
            return moniker.GetImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
