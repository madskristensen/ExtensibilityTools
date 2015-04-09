using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(PkgdefContentTypeDefinition.PkgdefContentType)]
    [Name("Pkgdef Completion")]
    class PkgdefCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        IClassifierAggregatorService ClassifierAggregatorService = null;

        [Import]
        ITextStructureNavigatorSelectorService NavigatorService = null;

        [Import]
        IGlyphService GlyphService = null;

        private static ImageSource _glyph;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
           return new PkgdefCompletionSource(textBuffer, ClassifierAggregatorService, NavigatorService, GlyphService);
        }
    }
}