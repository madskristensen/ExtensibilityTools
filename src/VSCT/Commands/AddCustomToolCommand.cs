using System;
using System.IO;
using EnvDTE;
using MadsKristensen.ExtensibilityTools.VSCT.Generator;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

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
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdAddCustomTool, ShowMessageBox, AddCustomToolItemBeforeQueryStatus);
        }

        private void AddCustomToolItemBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand) sender;
            button.Visible = false;
            
            _item = ProjectHelpers.GetSelectedItem() as ProjectItem;
            if (_item == null)
                return;

            if (_item.ContainingProject.Kind != PrjKind.prjKindCSharpProject && _item.ContainingProject.Kind != PrjKind.prjKindVBProject)
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
