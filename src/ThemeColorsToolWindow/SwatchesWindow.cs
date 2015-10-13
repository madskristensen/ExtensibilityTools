using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools.ThemeColorsToolWindow
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("a4acbaf7-9223-4fbe-8177-38a8cf4ab59a")]
    public class SwatchesWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwatchesWindow"/> class.
        /// </summary>
        public SwatchesWindow() : base(null)
        {
            this.Caption = "Theme Swatches";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new SwatchesWindowControl();
        }

        public static object TextBoxTextBrushKey => EnvironmentColors.ComboBoxTextBrushKey;

        public static object TextBoxBackgroundBrushKey => EnvironmentColors.ComboBoxBackgroundBrushKey;

        public static object TextBoxBorderBrushKey => EnvironmentColors.ComboBoxBorderBrushKey;

        public static object TextBrushKey => EnvironmentColors.BrandedUITextBrushKey;
    }
}
