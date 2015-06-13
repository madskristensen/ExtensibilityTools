using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType(PkgdefContentTypeDefinition.PkgdefContentType)]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (!ExtensibilityToolsPackage.Instance.Options.PkgdefEnableOutlining)
                return null;

            return buffer.Properties.GetOrCreateSingletonProperty(() => new OutliningTagger(buffer)) as ITagger<T>;
        }
    }
}
