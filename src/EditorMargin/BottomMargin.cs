using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;

namespace MadsKristensen.ExtensibilityTools.EditorMargin
{
    class BottomMargin : DockPanel, IWpfTextViewMargin
    {
        public const string MarginName = "Extensibility Tools Margin";

        readonly IWpfTextView _textView;
        bool _isDisposed = false;
        IClassifier _classifier;
        TextControl _lblClassification, _lblEncoding, _lblContentType, _lblSelection, _lblRoles;
        readonly ITextDocument _doc;

        public BottomMargin(IWpfTextView textView, IClassifierAggregatorService classifier, ITextDocumentFactoryService documentService)
        {
            _textView = textView;
            _classifier = classifier.GetClassifier(textView.TextBuffer);

            SetResourceReference(BackgroundProperty, EnvironmentColors.ScrollBarBackgroundBrushKey);

            ClipToBounds = true;

            _lblEncoding = new TextControl("Encoding");
            Children.Add(_lblEncoding);

            _lblContentType = new TextControl("Content type");
            Children.Add(_lblContentType);

            _lblClassification = new TextControl("Classification");
            Children.Add(_lblClassification);

            _lblSelection = new TextControl("Selection");
            Children.Add(_lblSelection);

            _lblRoles = new TextControl("Roles");
            Children.Add(_lblRoles);

            UpdateClassificationLabel();
            UpdateContentTypeLabel();
            UpdateContentSelectionLabel();
            UpdateRolesLabel();

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
                try
                {
                    byte[] preamble = doc.Encoding.GetPreamble();
                    string bom = preamble != null && preamble.Length > 2 ? " - BOM" : string.Empty;

                    _lblEncoding.Value = doc.Encoding.EncodingName + bom;
                    _lblEncoding.SetTooltip("Codepage:         " + doc.Encoding.CodePage + Environment.NewLine +
                                            "Windows codepage: " + doc.Encoding.CodePage + Environment.NewLine +
                                            "Header name:      " + doc.Encoding.HeaderName + Environment.NewLine +
                                            "Body name:        " + doc.Encoding.BodyName,
                                            true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }

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
                    _lblContentType.SetTooltip("base types: " + string.Join(", ", typeNames) + Environment.NewLine +
                                               "Snapshot: " + buffer.CurrentSnapshot.Version);
                }

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private void UpdateClassificationLabel()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private void UpdateContentSelectionLabel()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
        }

        private void UpdateRolesLabel()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (_textView.Roles.Any())
                    {
                        var roles = _textView.Roles.Select(r => r);
                        var content = string.Join(Environment.NewLine, roles);

                        _lblRoles.SetTooltip(content);
                        _lblRoles.Value = roles.Last();
                    }
                    else
                    {
                        _lblRoles.Value = "n/a";
                        _lblRoles.ToolTip = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
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

                (_classifier as IDisposable)?.Dispose();
            }
        }
    }
}
