using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;

namespace MadsKristensen.ExtensibilityTools.EditorMargin
{
    class BottomMargin : DockPanel, IWpfTextViewMargin
    {
        public const string MarginName = "Extensibility Tools Margin";

        private IWpfTextView _textView;
        private bool _isDisposed = false;
        private IClassifier _classifier;
        private TextControl _lblClassification, _lblEncoding, _lblContentType, _lblSelection;
        private Brush _foregroundBrush, _backgroundBrush;
        private ITextDocument _doc;

        public BottomMargin(IWpfTextView textView, IClassifierAggregatorService classifier, ITextDocumentFactoryService documentService)
        {
            _textView = textView;
            _classifier = classifier.GetClassifier(textView.TextBuffer);
            _foregroundBrush = new SolidColorBrush((Color)FindResource(VsColors.CaptionTextKey));
            _backgroundBrush = new SolidColorBrush((Color)FindResource(VsColors.ScrollBarBackgroundKey));

            this.Background = _backgroundBrush;
            this.ClipToBounds = true;

            _lblEncoding = new TextControl("Encoding");
            this.Children.Add(_lblEncoding);

            _lblContentType = new TextControl("Content type");
            this.Children.Add(_lblContentType);

            _lblClassification = new TextControl("Classification");
            this.Children.Add(_lblClassification);

            _lblSelection = new TextControl("Selection");
            this.Children.Add(_lblSelection);

            UpdateClassificationLabel();
            UpdateContentTypeLabel();
            UpdateContentSelectionLabel();

            if (documentService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _doc))
            {
                _doc.FileActionOccurred += FileChangedOnDisk;
                UpdateEncodingLabel(_doc);
            }

            textView.Caret.PositionChanged += CaretPositionChanged;
        }

        private void FileChangedOnDisk(object sender, TextDocumentFileActionEventArgs e)
        {
            UpdateEncodingLabel(_doc);
        }

        private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateClassificationLabel();
            UpdateContentTypeLabel();
            UpdateContentSelectionLabel();
        }

        private void UpdateEncodingLabel(ITextDocument doc)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                byte[] preamble = doc.Encoding.GetPreamble();
                string bom = preamble != null && preamble.Length > 2 ? " - BOM" : string.Empty;

                _lblEncoding.Value = doc.Encoding.EncodingName + bom;
                _lblEncoding.SetTooltip("Codepage:         " + doc.Encoding.CodePage + Environment.NewLine +
                                        "Windows codepage: " + doc.Encoding.CodePage + Environment.NewLine +
                                        "Header name:      " + doc.Encoding.HeaderName + Environment.NewLine +
                                        "Body name:        " + doc.Encoding.BodyName,
                                        true);

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private void UpdateContentTypeLabel()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SnapshotPoint? point;
                ITextBuffer buffer = GetTextBuffer(out point);

                _lblContentType.Value = buffer.ContentType.TypeName;

                var typeNames = buffer.ContentType.BaseTypes.Select(t => t.DisplayName);

                if (typeNames.Any())
                {
                    _lblContentType.SetTooltip("base types: " + string.Join(", ", typeNames));
                }

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private void UpdateClassificationLabel()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_textView.TextBuffer.CurrentSnapshot.Length <= 1)
                    return;

                SnapshotPoint? point;
                ITextBuffer buffer = GetTextBuffer(out point);
                int position = point.Value.Position;

                if (position == buffer.CurrentSnapshot.Length)
                    position = position - 1;

                var span = new SnapshotSpan(buffer.CurrentSnapshot, position, 1);
                var cspans = _classifier.GetClassificationSpans(span);

                if (cspans.Count == 0)
                {
                    _lblClassification.Value = "None";
                    _lblClassification.SetTooltip("None");
                }
                else
                {
                    var ctype = cspans[0].ClassificationType;
                    string name = ctype.Classification;

                    if (name.Contains(" - "))
                    {
                        int index = name.IndexOf(" - ", StringComparison.Ordinal);
                        name = name.Substring(0, index).Trim();
                    }

                    _lblClassification.SetTooltip(ctype.Classification);
                    _lblClassification.Value = name;
                }

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private void UpdateContentSelectionLabel()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int start = _textView.Selection.Start.Position.Position;
                int end = _textView.Selection.End.Position.Position;

                if (end == start)
                {
                    _lblSelection.Value = start.ToString();
                }
                else
                {
                    _lblSelection.Value = $"{start}-{end} ({end - start} chars)";
                }

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private ITextBuffer GetTextBuffer(out SnapshotPoint? point)
        {
            IProjectionBuffer projection = _textView.TextBuffer as IProjectionBuffer;

            if (projection != null)
            {
                var snapshotPoint = _textView.Caret.Position.BufferPosition;

                foreach (ITextBuffer buffer in projection.SourceBuffers.Where(s => !s.ContentType.IsOfType("htmlx")))
                {
                    point = _textView.BufferGraph.MapDownToBuffer(snapshotPoint, PointTrackingMode.Negative, buffer, PositionAffinity.Predecessor);

                    if (point.HasValue)
                    {
                        return buffer;
                    }
                }
            }

            point = _textView.Caret.Position.BufferPosition;

            return _textView.TextBuffer;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }
        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }
        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return this.ActualHeight;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == BottomMargin.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;

                _doc.FileActionOccurred -= FileChangedOnDisk;
                _textView.Caret.PositionChanged -= CaretPositionChanged;
            }
        }
    }
}
