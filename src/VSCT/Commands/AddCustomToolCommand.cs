using System;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.ExtensibilityTools.VSCT
{
    class AddCustomToolCommand
    {
        private const string CUSTOM_TOOL_NAME = "VsctGenerator";
        private IServiceProvider _provider;
        private DTE2 _dte;
        private ProjectItem _item;

        private AddCustomToolCommand(IServiceProvider provider)
        {
            _provider = provider;
            _dte = provider.GetService(typeof(DTE)) as DTE2;

            SetupCommands();
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

        private void SetupCommands()
        {
            OleMenuCommandService commandService = _provider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                CommandID addCustomToolID = new CommandID(GuidList.guidExtensibilityToolsCmdSet, PackageCommands.cmdAddCustomTool);
                OleMenuCommand addCustomToolItem = new OleMenuCommand(ShowMessageBox, addCustomToolID);
                addCustomToolItem.BeforeQueryStatus += AddCustomToolItemBeforeQueryStatus;
                commandService.AddCommand(addCustomToolItem);
            }
        }

        private void AddCustomToolItemBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand)sender;
            button.Visible = false;

            UIHierarchyItem uiItem = GetSelectedItem();

            if (uiItem == null)
                return;

            _item = uiItem.Object as ProjectItem;

            string fullPath = _item.Properties.Item("FullPath").Value.ToString();
            string ext = Path.GetExtension(fullPath);

            if (!ext.Equals(".vsct", StringComparison.OrdinalIgnoreCase))
                return;

            button.Checked = _item.Properties.Item("CustomTool").Value.ToString() == CUSTOM_TOOL_NAME;
            button.Visible = true;
        }

        private UIHierarchyItem GetSelectedItem()
        {
            var items = (Array)_dte.ToolWindows.SolutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selItem in items)
            {
                return selItem;
            }

            return null;
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
