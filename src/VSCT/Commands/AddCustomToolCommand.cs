using System;
using System.IO;
using EnvDTE;
using MadsKristensen.ExtensibilityTools.VSCT.Generator;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class AddCustomToolCommand : BaseCommand
    {
        private const string CUSTOM_TOOL_NAME = VsctCodeGenerator.GeneratorName;
        private ProjectItem _item;

        private AddCustomToolCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public static AddCustomToolCommand Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new AddCustomToolCommand(provider);
        }

        protected override void SetupCommands()
        {
            AddCommand(GuidList.guidExtensibilityToolsCmdSet, PackageCommands.cmdAddCustomTool, ShowMessageBox, AddCustomToolItemBeforeQueryStatus);
        }

        private void AddCustomToolItemBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand) sender;
            button.Visible = false;

            UIHierarchyItem uiItem = GetSelectedItem();

            if (uiItem == null)
                return;

            _item = uiItem.Object as ProjectItem;
            if (_item == null)
                return;

            string fullPath = _item.Properties.Item("FullPath").Value.ToString();
            string ext = Path.GetExtension(fullPath);

            if (!ext.Equals(".vsct", StringComparison.OrdinalIgnoreCase))
                return;

            button.Checked = _item.Properties.Item("CustomTool").Value.ToString() == CUSTOM_TOOL_NAME;
            button.Visible = true;
        }

        private void ShowMessageBox(object sender, EventArgs e)
        {
            bool synOn = _item.Properties.Item("CustomTool").Value.ToString() == CUSTOM_TOOL_NAME;

            if (synOn)
            {
                _item.Properties.Item("CustomTool").Value = "";
            }
            else
            {
                _item.Properties.Item("CustomTool").Value = CUSTOM_TOOL_NAME;
            }
        }
    }
}
