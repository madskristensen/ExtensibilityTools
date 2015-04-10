using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools.EditorMargin
{
    class TextControl : DockPanel
    {
        private Label _lblName, _lblValue;

        public TextControl(string name, string value = "pending...")
        {
            var foreground = new SolidColorBrush((Color)FindResource(VsColors.CaptionTextKey));

            _lblName = new Label();
            _lblName.Foreground = foreground;
            _lblName.Padding = new Thickness(3, 3, 0, 3);
            _lblName.FontWeight = FontWeights.Bold;
            _lblName.Content = name + ": ";
            this.Children.Add(_lblName);

            _lblValue = new Label();
            _lblValue.Foreground = _lblName.Foreground;
            _lblValue.Padding = new Thickness(0, 3, 10, 3); ;
            _lblValue.Content = value;
            this.Children.Add(_lblValue);
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

        public void SetTooltip(string tooltip)
        {
            _lblValue.ToolTip = tooltip;
        }
    }
}
