using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(PkgdefContentTypeDefinition.PkgdefContentType)]
    [TagType(typeof(ErrorTag))]
    class PkgdefErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        IClassifierAggregatorService _classifierAggregatorService = null;

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (!ExtensibilityToolsPackage.Options.PkgdefEnableValidation)
                return null;
                
            if (!buffer.Properties.TryGetProperty(typeof(ErrorListProvider), out ErrorListProvider errorlist))
                return null;
                
            if (!buffer.Properties.TryGetProperty(typeof(IWpfTextView), out IWpfTextView view))
                return null;

            ITextDocument document;
            if (TextDocumentFactoryService.TryGetTextDocument(buffer, out document) && errorlist != null && view != null)
            {
                return new PkgdefErrorTagger(view, _classifierAggregatorService, errorlist, document) as ITagger<T>;
            }

            return null;
        }
    }
}
