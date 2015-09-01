using System;
using System.Linq;
using System.IO;
using System.Text;
using EnvDTE;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class ShowProjectInformation : BaseCommand
    {
        private ShowProjectInformation(IServiceProvider serviceProvider)
       : base(serviceProvider)
        {
        }

        public static ShowProjectInformation Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new ShowProjectInformation(provider);
        }

        protected override void SetupCommands()
        {
            AddCommand(PackageGuids.guidExtensibilityToolsCmdSet, PackageIds.cmdShowInformation, ShowInformation, BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            /* Nothing to do here */
        }

        private void ShowInformation(object sender, EventArgs e)
        {
            Project project = GetProject();

            if (project == null)
                return;

            string file = Path.Combine(Path.GetTempPath(), "vs-project-info.txt");

            WriteFile(file, project);
            DTE.ItemOperations.OpenFile(file);
        }

        private void WriteFile(string fileName, Project project)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name:\t\t\t {project.Name}");
            sb.AppendLine($"UniqueName:\t\t {project.UniqueName}");
            sb.AppendLine($"FullName:\t\t {project.FullName}");
            sb.AppendLine($"FileName:\t\t {project.FileName}");
            sb.AppendLine($"Kind:\t\t\t {project.Kind}");
            sb.AppendLine($"ExtenderCATID:\t {project.Properties}");

            if (project.Properties != null)
            {
                sb.AppendLine();
                sb.AppendLine("Properties:");
                var properties = project.Properties.OfType<Property>().OrderBy(p => p.Name);

                foreach (Property prop in properties)
                {
                    string displayValue = "<no value>";

                    if (prop.Value != null && !string.IsNullOrWhiteSpace(prop.Value.ToString()))
                        displayValue = prop.Value.ToString();

                    sb.AppendLine($"\t{prop.Name.PadRight(40, ' ')}:\t{displayValue}");
                }
            }

            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        }

        private Project GetProject()
        {
            var item = GetSelectedItem();

            if (item == null)
                return null;

            return item.Object as Project;
        }
    }
}
