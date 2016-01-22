using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools.EditorMargin
{
    class TextControl : DockPanel
    {
        Label _lblName;
        readonly Label _lblValue;

        public TextControl(string name, string value = "pending...")
        {
            var foreground = new SolidColorBrush((Color)FindResource(VsColors.CaptionTextKey));

            _lblName = new Label();
            _lblName.Padding = new Thickness(3, 3, 0, 3);
            _lblName.FontWeight = FontWeights.Bold;
            _lblName.Content = name + ": ";
            _lblName.SetResourceReference(TextBlock.ForegroundProperty, EnvironmentColors.ComboBoxFocusedTextBrushKey);
            Children.Add(_lblName);

            _lblValue = new Label();
            _lblValue.Padding = new Thickness(0, 3, 10, 3); ;
            _lblValue.Content = value;
            _lblValue.SetResourceReference(TextBlock.ForegroundProperty, EnvironmentColors.ComboBoxFocusedTextBrushKey);
            Children.Add(_lblValue);
        }

        public string Value
        {
            get
            {
                return _lblValue.Content.ToString();
            }
            set
            {
                _lblValue.Content = value;
            }
        }

        public void SetTooltip(string tooltip, bool preserveFormatting = false)
        {
            if (preserveFormatting)
            {
                Label label = new Label();
                label.Content = tooltip;
                label.FontFamily = new FontFamily("Lucida Console");
                _lblValue.ToolTip = label;
            }
            else
            {
                _lblValue.ToolTip = tooltip;
            }
        }
    }
}
