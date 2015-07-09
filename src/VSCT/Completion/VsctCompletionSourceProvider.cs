using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Vsct
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(VsctContentTypeDefinition.VsctContentType)]
    [Name("Vsct Completion")]
    class VsctCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        IClassifierAggregatorService ClassifierAggregatorService = null;

        [Import]
        ITextStructureNavigatorSelectorService NavigatorService = null;

        [Import]
        IGlyphService GlyphService = null;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new VsctCompletionSource(textBuffer, ClassifierAggregatorService, NavigatorService, GlyphService);
        }
    }
}