using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
            var imageAttributes = new ImageAttributes
            {
                Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WPF,
                Dpi = 96,
                LogicalHeight = size,
                LogicalWidth = size,
                StructSize = Marshal.SizeOf(typeof(ImageAttributes)),
            };

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
                SaveBitmapToDisk(image, sfd.FileName);
            }

            return saved == System.Windows.Forms.DialogResult.OK;
        }


        private void btnExportAll_Click(object sender, RoutedEventArgs e)
        {
            var exportTargetFolder = GetFolderForExportAll();
            if (string.IsNullOrWhiteSpace(exportTargetFolder))
                return;

            if (System.Windows.MessageBox.Show("Are you sure you want to export " + cbMonikers.Items.Count + " Image Monikers? This may take a while..", "Please confirm Image Monikers Export", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int size = 175;
                int.TryParse(txtSize.Text, out size);

                foreach (var viewModel in cbMonikers.Items.OfType<ViewModel>())
                {
                    var image = GetImage(viewModel.Moniker, size);
                    SaveBitmapToDisk(image,
                        Path.Combine(exportTargetFolder, viewModel.Label + "_" + size + "x" + size + ".png"));
                }
            }
        }

        private static string GetFolderForExportAll()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var dialogResult = dialog.ShowDialog();

                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    return dialog.SelectedPath;
                else
                {
                    return string.Empty;
                }
            }
        }

        private static void SaveBitmapToDisk(BitmapSource image, string fileName)
        {
            var fileParentPath = Path.GetDirectoryName(fileName);

            if (Directory.Exists(fileParentPath) == false)
                Directory.CreateDirectory(fileParentPath);

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
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
