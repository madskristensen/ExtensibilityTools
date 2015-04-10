using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(PkgdefContentTypeDefinition.PkgdefContentType)]
    class PkgdefClassifierProvider : IClassifierProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (!ExtensibilityToolsPackage.Options.PkgdefEnableColorizer)
                return null;

            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new PkgdefClassifier(Registry));
        }
    }
}
