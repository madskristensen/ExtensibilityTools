using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(PkgdefContentTypeDefinition.PkgdefContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class PkgdefTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        ICompletionBroker CompletionBroker = null;

        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        private ErrorListProvider _errorList;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);

            view.TextBuffer.Properties.GetOrCreateSingletonProperty(() => view);
            _errorList = view.TextBuffer.Properties.GetOrCreateSingletonProperty(() => new ErrorListProvider(ServiceProvider));

            if (_errorList == null)
                return;

            PkgdefCompletionController completion = new PkgdefCompletionController(view, CompletionBroker);
            IOleCommandTarget completionNext;
            textViewAdapter.AddCommandFilter(completion, out completionNext);
            completion.Next = completionNext;

            PkgdefFormatter formatter = new PkgdefFormatter(view);
            IOleCommandTarget formatterNext;
            textViewAdapter.AddCommandFilter(formatter, out formatterNext);
            formatter.Next = formatterNext;

            view.Closed += OnViewClosed;
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;
            view.Closed -= OnViewClosed;

            if (_errorList != null)
            {
                _errorList.Tasks.Clear();
                _errorList.Dispose();
            }
        }
    }
}
