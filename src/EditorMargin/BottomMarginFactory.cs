using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.EditorMargin
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(BottomMargin.MarginName)]
    [Order(After = PredefinedMarginNames.BottomControl)]
    [MarginContainer(PredefinedMarginNames.Bottom)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]
    class BottomMarginFactory : IWpfTextViewMarginProvider
    {
        [Import]
        IClassifierAggregatorService _classifierService = null;

        [Import]
        public ITextDocumentFactoryService _documentService = null;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            return new BottomMargin(wpfTextViewHost.TextView, _classifierService, _documentService);
        }
    }
}
