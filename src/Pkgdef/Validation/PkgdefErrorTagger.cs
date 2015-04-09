using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class PkgdefErrorTagger : ITagger<IErrorTag>
    {
        private IClassifier _classifier;
        private ErrorListProvider _errorlist;
        private ITextDocument _document;
        private IWpfTextView _view;

        public PkgdefErrorTagger(IWpfTextView view, IClassifierAggregatorService classifier, ErrorListProvider errorlist, ITextDocument document)
        {
            _view = view;
            _classifier = classifier.GetClassifier(view.TextBuffer);
            _errorlist = errorlist;
            _document = document;
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var span = spans[0];
            var line = span.Start.GetContainingLine();
            var classificationSpans = _classifier.GetClassificationSpans(line.Extent);

            ClearError(line);

            foreach (var cspan in classificationSpans)
            {
                if (cspan.ClassificationType.IsOfType(PredefinedClassificationTypeNames.SymbolDefinition))
                {
                    string text = cspan.Span.GetText();

                    if (text.StartsWith("$", StringComparison.Ordinal))
                    {
                        string word = text.Trim('$');

                        if (!CompletionItem.Items.Any(i => i.Name.Equals(word, StringComparison.OrdinalIgnoreCase)))
                            yield return CreateError(line, cspan.Span, "The keyword '$" + word + "$' doesn't exist");
                    }
                }

                if (cspan.ClassificationType.IsOfType(PkgdefClassificationTypes.Guid))
                {
                    string text = cspan.Span.GetText();
                    Guid guid;

                    if (!Guid.TryParse(text, out guid))
                    {
                        yield return CreateError(line, cspan.Span, "\"" + text + "\" is not a valid GUID.");
                    }
                }

                else if (cspan.ClassificationType.IsOfType(PkgdefClassificationTypes.RegKey))
                {
                    string lineText = line.GetText();

                    var match = Variables.RegKey.Match(lineText);
                    if (!match.Success)
                        break;

                    var group = match.Groups["path"];
                    string path = group.Value;

                    if (span.Snapshot.Length <= span.Start + group.Index + group.Length)
                        break;

                    var hit = new SnapshotSpan(span.Snapshot, span.Start + group.Index, group.Length);

                    if (path.Trim().Length < path.Length)
                        yield return CreateError(line, hit, "Remove whitespace around the registry key path");

                    else if (string.IsNullOrWhiteSpace(path))
                        yield return CreateError(line, cspan.Span, "You must specify a registry key path");

                    else if (!match.Value.EndsWith("]"))
                        yield return CreateError(line, hit, "Unclosed registry key entry. Add the missing ] character");

                    else if (cspan.Span.GetText().Contains("/"))
                        yield return CreateError(line, cspan.Span, "Use the backslash character as delimiter instead of forward slash.");
                }

                else if (cspan.ClassificationType.IsOfType(PredefinedClassificationTypeNames.String))
                {
                    string lineText = line.GetText();

                    foreach (Match match in Variables.String.Matches(lineText))
                    {
                        string text = match.Value;

                        var hit = new SnapshotSpan(cspan.Span.Snapshot, span.Start + match.Index, match.Length);

                        if (text.Length <= 1 || text[text.Length - 1] != '"')
                        {
                            yield return CreateError(line, hit, "Unclosed string. Add the missing \" character.");
                        }
                    }
                }
            }
        }

        private TagSpan<ErrorTag> CreateError(ITextSnapshotLine line, SnapshotSpan span, string message)
        {
            foreach (ErrorTask existing in _errorlist.Tasks)
            {
                if (existing.Line == line.LineNumber && existing.Text.EndsWith(message))
                    return null;
            }

            ErrorTask task = CreateErrorTask(line, span, "Extensibility Tools: " + message);
            _errorlist.Tasks.Add(task);

            return new TagSpan<ErrorTag>(span, new ErrorTag("Syntax error", message));
        }

        private ErrorTask CreateErrorTask(ITextSnapshotLine line, SnapshotSpan span, string text)
        {
            ErrorTask task = new ErrorTask
            {
                Text = text,
                Line = line.LineNumber,
                Column = span.Start.Position - line.Start.Position,
                Category = TaskCategory.Misc,
                ErrorCategory = TaskErrorCategory.Error,
                Priority = TaskPriority.Normal,
                Document = _document.FilePath
            };

            task.Navigate += task_Navigate;

            return task;
        }

        private void task_Navigate(object sender, EventArgs e)
        {
            ErrorTask task = (ErrorTask)sender;
            _errorlist.Navigate(task, new Guid("{00000000-0000-0000-0000-000000000000}"));

            var line = _view.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(task.Line);
            var point = new SnapshotPoint(line.Snapshot, line.Start.Position + task.Column);
            _view.Caret.MoveTo(point);
        }

        private void ClearError(ITextSnapshotLine line)
        {
            foreach (ErrorTask existing in _errorlist.Tasks)
            {
                if (existing.Line == line.LineNumber)
                {
                    _errorlist.Tasks.Remove(existing);
                    break;
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }
    }

}
