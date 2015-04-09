using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    static class PkgdefClassificationTypes
    {
        public const string Dword = "Pkgdef_dword";
        public const string RegKey = "Pkgdef_regkey";
        public const string Guid = "Pkgdef_guid";

        [Export, Name(PkgdefClassificationTypes.Dword)]
        public static ClassificationTypeDefinition PkgdefDwordClassification { get; set; }

        [Export, Name(PkgdefClassificationTypes.RegKey)]
        public static ClassificationTypeDefinition PkgdefRegKeyClassification { get; set; }

        [Export, Name(PkgdefClassificationTypes.Guid)]
        public static ClassificationTypeDefinition PkgdefGuidClassification { get; set; }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = PkgdefClassificationTypes.Dword)]
    [Name(PkgdefClassificationTypes.Dword)]
    [Order(After = Priority.High)]
    [UserVisible(true)]
    internal sealed class PkgdefDwordFormatDefinition : ClassificationFormatDefinition
    {
        public PkgdefDwordFormatDefinition()
        {
            IsBold = true;
            DisplayName = "Pkgdef Entry Key";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = PkgdefClassificationTypes.RegKey)]
    [Name(PkgdefClassificationTypes.RegKey)]
    [Order(After = Priority.High)]
    [UserVisible(true)]
    internal sealed class PkgdefRegKeyFormatDefinition : ClassificationFormatDefinition
    {
        public PkgdefRegKeyFormatDefinition()
        {
            IsBold = true;
            DisplayName = "Pkgdef Registry Path";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = PkgdefClassificationTypes.Guid)]
    [Name(PkgdefClassificationTypes.Guid)]
    [Order(Before = Priority.Low)]
    [UserVisible(true)]
    internal sealed class PkgdefGuidFormatDefinition : ClassificationFormatDefinition
    {
        public PkgdefGuidFormatDefinition()
        {
            DisplayName = "Pkgdef Guid";
        }
    }
}
