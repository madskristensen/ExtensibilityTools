using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace MadsKristensen.ExtensibilityTools.VsixManifest.Commands
{
    sealed class AddResxGeneratorCommand: BaseCommand
    {
        private const string CUSTOM_TOOL_NAME = ResxFileGenerator.Name;
        private ProjectItem _item;

        private AddResxGeneratorCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public static AddResxGeneratorCommand Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new AddResxGeneratorCommand(provider);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdResxGenerator, Execute, BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand) sender;
            button.Visible = false;

            UIHierarchyItem uiItem = GetSelectedItem();

            if (uiItem == null)
                return;

            _item = uiItem.Object as ProjectItem;
            if (_item == null)
                return;

            if (_item.ContainingProject.Kind != PrjKind.prjKindCSharpProject && _item.ContainingProject.Kind != PrjKind.prjKindVBProject)
                return;

            string fullPath = _item.Properties.Item("FullPath").Value.ToString();
            string ext = Path.GetExtension(fullPath);

            if (!ext.Equals(".vsixmanifest", StringComparison.OrdinalIgnoreCase))
                return;

            button.Checked = _item.Properties.Item("CustomTool").Value.ToString() == CUSTOM_TOOL_NAME;
            button.Visible = true;
        }

        private void Execute(object sender, EventArgs e)
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
