using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    static class PkgdefClassificationTypes
    {
        public const string EntryKey = "Pkgdef Entry Key";
        public const string RegistryPath = "Pkgdef Registry Path";
        public const string Guid = "Pkgdef Guid";

        [Export, Name(PkgdefClassificationTypes.EntryKey)]
        public static ClassificationTypeDefinition PkgdefDwordClassification { get; set; }

        [Export, Name(PkgdefClassificationTypes.RegistryPath)]
        public static ClassificationTypeDefinition PkgdefRegKeyClassification { get; set; }

        [Export, Name(PkgdefClassificationTypes.Guid)]
        public static ClassificationTypeDefinition PkgdefGuidClassification { get; set; }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = PkgdefClassificationTypes.EntryKey)]
    [Name(PkgdefClassificationTypes.EntryKey)]
    [Order(After = Priority.High)]
    [UserVisible(true)]
    internal sealed class PkgdefDwordFormatDefinition : ClassificationFormatDefinition
    {
        public PkgdefDwordFormatDefinition()
        {
            IsBold = true;
            DisplayName = PkgdefClassificationTypes.EntryKey;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = PkgdefClassificationTypes.RegistryPath)]
    [Name(PkgdefClassificationTypes.RegistryPath)]
    [Order(After = Priority.High)]
    [UserVisible(true)]
    internal sealed class PkgdefRegKeyFormatDefinition : ClassificationFormatDefinition
    {
        public PkgdefRegKeyFormatDefinition()
        {
            IsBold = true;
            DisplayName = PkgdefClassificationTypes.RegistryPath;
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
            DisplayName = PkgdefClassificationTypes.Guid;
        }
    }
}
