using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Operations;
using Intel = Microsoft.VisualStudio.Language.Intellisense;

namespace MadsKristensen.ExtensibilityTools.Vsct
{
    class VsctCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;
        private IClassifier _classifier;
        private ITextStructureNavigatorSelectorService _navigator;
        private ImageSource _defaultGlyph;
        private ImageSource _builtInGlyph;
        private IGlyphService _glyphService;

        public VsctCompletionSource(ITextBuffer buffer, IClassifierAggregatorService classifier, ITextStructureNavigatorSelectorService navigator, IGlyphService glyphService)
        {
            _buffer = buffer;
            _classifier = classifier.GetClassifier(buffer);
            _navigator = navigator;
            _glyphService = glyphService;
            _defaultGlyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.GlyphItemPublic);
            _builtInGlyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.TotalGlyphItems);
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                return;

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            SnapshotSpan extent = FindTokenSpanAtPosition(session).GetSpan(snapshot);
            var line = triggerPoint.Value.GetContainingLine().Extent;

            var spans = _classifier.GetClassificationSpans(line);

            List<Intel.Completion> list = new List<Completion>();
            int position = session.TextView.Caret.Position.BufferPosition;

            foreach (var span in spans)
            {
                if (span.ClassificationType.IsOfType("XML Attribute Value"))
                {
                    if (!span.Span.Contains(extent.Start))
                        continue;

                    if (!span.Span.Contains(extent))
                        continue;

                    var attrs = spans.Where(s => s.ClassificationType.IsOfType("XML Attribute"));
                    var currentAttr = attrs.Where(s => s.Span.Start <= span.Span.Start).OrderByDescending(s => s.Span.Start.Position).FirstOrDefault();

                    if (currentAttr == null)
                        return;

                    string current = currentAttr.Span.GetText();
                    string guid = null;
                    string id = null;
                    string lineText = line.GetText();

                    Match idMatch = Regex.Match(lineText, " id=\"(?<id>[^\"]+)\"");

                    if (idMatch.Success)
                        id = idMatch.Groups["id"].Value;

                    Match guidMatch = Regex.Match(lineText, " guid=\"(?<guid>[^\"]+)\"");

                    if (guidMatch.Success)
                        guid = guidMatch.Groups["guid"].Value;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Regex.Replace(snapshot.GetText(), " xmlns(:[^\"]+)?=\"([^\"]+)\"", string.Empty));

                    if (current == "id" && !string.IsNullOrEmpty(guid))
                    {
                        if (guid == "guidSHLMainMenu")
                        {
                            foreach (string key in VsctBuiltInCache._dic.Keys)
                            {
                                list.Add(CreateCompletion(key, key, _builtInGlyph, VsctBuiltInCache._dic[key]));
                            }
                        }
                        else
                        {
                            GetNameValueCompletion(list, doc, "//GuidSymbol[@name='" + guid + "']//IDSymbol");
                        }
                    }
                    else if (current == "guid")
                    {
                        list.Add(CreateCompletion("guidSHLMainMenu", "guidSHLMainMenu", _builtInGlyph, "Guid for Shell's group and menu ids."));
                        GetNameValueCompletion(list, doc, "//GuidSymbol");
                    }
                }
            }

            if (list.Count > 0)
            {
                var applicableTo = snapshot.CreateTrackingSpan(extent, SpanTrackingMode.EdgeInclusive);
                completionSets.Add(new CompletionSet("All", "All", applicableTo, list, Enumerable.Empty<Intel.Completion>()));
            }
        }

        private void GetNameValueCompletion(List<Intel.Completion> list, XmlDocument doc, string xpath)
        {
            var nodes = doc.SelectNodes(xpath);
            foreach (XmlNode node in nodes)
            {
                XmlAttribute name = node.Attributes["name"];

                if (name != null)
                {
                    string description = null;
                    XmlAttribute valueAttr = node.Attributes["value"];

                    if (valueAttr != null)
                        description = valueAttr.InnerText;

                    list.Add(CreateCompletion(name.InnerText, name.InnerText, _defaultGlyph, description));
                }
            }
        }

        private Completion CreateCompletion(string name, string insertion, ImageSource glyph = null, string description = null)
        {
            if (glyph == null)
                glyph = _defaultGlyph;

            return new Completion(name, insertion, description, glyph, null);
        }

        private ITrackingSpan FindTokenSpanAtPosition(ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = _navigator.GetTextStructureNavigator(_buffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}