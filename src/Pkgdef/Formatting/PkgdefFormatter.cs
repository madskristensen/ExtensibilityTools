using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    internal sealed class PkgdefFormatter : IOleCommandTarget
    {
        private IWpfTextView _view;
        private static Regex _separateRegPaths = new Regex(@"([\s]{1,2})\[", RegexOptions.Compiled);
        private static Regex _stripEmptyLines = new Regex(@"([\s]{3,})", RegexOptions.Compiled);
        private static Regex _trimLineStart = new Regex(@"^([ \t]+)(?<content>\S)", RegexOptions.Compiled | RegexOptions.Multiline);

        public PkgdefFormatter(IWpfTextView textView)
        {
            _view = textView;
        }

        public IOleCommandTarget Next { get; set; }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                    FormatSpan(0, _view.TextBuffer.CurrentSnapshot.Length);
                    break;
                case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                    FormatSelection();
                    break;
                }
            }

            return Next.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private void FormatSelection()
        {
            int start = _view.Selection.Start.Position.Position;
            int length = _view.Selection.End.Position.Position - start;

            FormatSpan(start, length);
        }

        private void FormatSpan(int start, int length)
        {
            var text = _view.TextBuffer.CurrentSnapshot.GetText(start, length);

            // Trim the beginning of each line
            text = _trimLineStart.Replace(text, "${content}");

            // Adds newlines before a registry path
            text = _separateRegPaths.Replace(text, Environment.NewLine + Environment.NewLine + "[");

            // Removes duplicate empty lines
            text = _stripEmptyLines.Replace(text, Environment.NewLine + Environment.NewLine);

            using (var edit = _view.TextBuffer.CreateEdit())
            {
                edit.Replace(start, length, text.TrimEnd());
                edit.Apply();
            }
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)prgCmds[0].cmdID)
                {
                case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                    prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                    return VSConstants.S_OK;
                }
            }

            return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}