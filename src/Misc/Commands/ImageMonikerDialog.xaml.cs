using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.Misc.Commands
{
    public partial class ImageMonikerDialog : Window
    {
        private static IVsImageService2 _imageService;

        public ImageMonikerDialog(IVsImageService2 imageService)
        {
            _imageService = imageService;
            Loaded += OnLoad;
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            Icon = GetImage(KnownMonikers.ExportPerformanceReport, 16);

            PropertyInfo[] monikers = typeof(KnownMonikers).GetProperties(BindingFlags.Static | BindingFlags.Public);
            List<ViewModel> list = new List<ViewModel>();

            foreach (var monikerName in monikers)
            {
                ImageMoniker moniker = (ImageMoniker)monikerName.GetValue(null, null);
                ViewModel container = new ViewModel(monikerName.Name, moniker);
                list.Add(container);
            }

            cbMonikers.Focus();
            cbMonikers.ItemsSource = list;
            var edit = (System.Windows.Controls.TextBox)cbMonikers.Template.FindName("PART_EditableTextBox", cbMonikers);
            edit.SelectAll();
        }

        public static BitmapSource GetImage(ImageMoniker moniker, int size)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags;
            imageAttributes.ImageType = (uint)_UIImageType.IT_Bitmap;
            imageAttributes.Format = (uint)_UIDataFormat.DF_WPF;
            imageAttributes.LogicalHeight = size;
            imageAttributes.LogicalWidth = size;
            imageAttributes.StructSize = Marshal.SizeOf(typeof(ImageAttributes));

            IVsUIObject result = _imageService.GetImage(moniker, imageAttributes);

            object data;
            result.get_Data(out data);

            if (data == null)
                return null;

            return data as BitmapSource;
        }

        private void cbMonikers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMonikers.SelectedItem == null)
                return;

            ViewModel container = (ViewModel)cbMonikers.SelectedItem;
            imgMoniker.Source = GetImage(container.Moniker, 150);
            cbMonikers.Text = container.Label;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            ViewModel container = (ViewModel)cbMonikers.SelectedItem;
            int size = 175;
            int.TryParse(txtSize.Text, out size);

            var image = GetImage(container.Moniker, size);
            bool saved = SaveImage(image, container.Label);
        }

        private bool SaveImage(BitmapSource image, string name)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = name + ".png";
            sfd.DefaultExt = ".png";
            var saved = sfd.ShowDialog();

            if (saved == System.Windows.Forms.DialogResult.OK)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (var filestream = new FileStream(sfd.FileName, FileMode.Create))
                    encoder.Save(filestream);
            }

            return saved == System.Windows.Forms.DialogResult.OK;
        }

        public class ViewModel
        {
            public ViewModel(string label, ImageMoniker moniker)
            {
                Label = label;
                Moniker = moniker;
            }

            public string Label { get; set; }

            public ImageMoniker Moniker { get; set; }

            public ImageSource Image
            {
                get
                {
                    return GetImage(Moniker, 16);
                }
            }

            public override string ToString()
            {
                return Label;
            }
        }
    }
}
