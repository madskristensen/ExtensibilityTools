using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Operations;
using Intel = Microsoft.VisualStudio.Language.Intellisense;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class PkgdefCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;
        private IClassifier _classifier;
        private ITextStructureNavigatorSelectorService _navigator;
        private ImageSource _defaultGlyph;
        private ImageSource _snippetGlyph;
        private IGlyphService _glyphService;

        public PkgdefCompletionSource(ITextBuffer buffer, IClassifierAggregatorService classifier, ITextStructureNavigatorSelectorService navigator, IGlyphService glyphService)
        {
            _buffer = buffer;
            _classifier = classifier.GetClassifier(buffer);
            _navigator = navigator;
            _glyphService = glyphService;
            _defaultGlyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.GlyphItemPublic);
            _snippetGlyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemPublic);
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
                if (span.ClassificationType.IsOfType(PredefinedClassificationTypeNames.SymbolDefinition) && span.Span.GetText().StartsWith("$"))
                {
                    if (!span.Span.Contains(extent.Start))
                        continue;

                    extent = span.Span;

                    foreach (var key in CompletionItem.Items)
                        list.Add(CreateCompletion(key.Name, "$" + key.Name + "$", _defaultGlyph, key.Description));
                }
                else if (extent.GetText().StartsWith("$"))
                {
                    if (!span.Span.Contains(extent.Start))
                        continue;

                    extent = new SnapshotSpan(snapshot, extent.Start, 1);

                    foreach (var key in CompletionItem.Items)
                        list.Add(CreateCompletion(key.Name, "$" + key.Name + "$", _defaultGlyph, key.Description));
                }
                else if (span.ClassificationType.IsOfType(PkgdefClassificationTypes.Guid))
                {
                    if (!span.Span.Contains(extent.Start))
                        continue;

                    extent = span.Span;

                    list.Add(CreateCompletion("<New GUID>", "{" + Guid.NewGuid() + "}"));

                    AddAllGuids(snapshot, list);
                }
                else if (position > 0 && snapshot.GetText(position - 1, 1) == "{")
                {
                    if (!span.Span.Contains(position - 1))
                        continue;

                    int length = 1;

                    if (snapshot.Length > position + 1 && snapshot.GetText(position, 1) == "}")
                        length = 2;

                    extent = new SnapshotSpan(snapshot, position - 1, length);

                    list.Add(CreateCompletion("<New GUID>", "{" + Guid.NewGuid() + "}"));

                    AddAllGuids(snapshot, list);
                }
            }

            if (spans.Count == 0 && extent.GetText() == "?")
            {
                HandleSnippets(list);
            }

            if (list.Count > 0)
            {
                var entries = list.OrderBy(entry => entry.Description);
                var applicableTo = snapshot.CreateTrackingSpan(extent, SpanTrackingMode.EdgeInclusive);
                completionSets.Add(new CompletionSet("All", "All", applicableTo, entries, Enumerable.Empty<Intel.Completion>()));
            }
        }

        private void HandleSnippets(List<Intel.Completion> list)
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly).ToLowerInvariant();
            string snippetDir = Path.Combine(folder, "Pkgdef\\Completion\\Snippets");

            foreach (string snippet in Directory.EnumerateFiles(snippetDir, "*.txt"))
            {
                string name = Path.GetFileNameWithoutExtension(snippet);
                string insertion = File.ReadAllText(snippet);
                list.Add(CreateCompletion(name, insertion, _snippetGlyph));
            }
        }

        private void AddAllGuids(ITextSnapshot snapshot, List<Intel.Completion> list)
        {
            var guidSpans = _classifier.GetClassificationSpans(new SnapshotSpan(snapshot, 0, snapshot.Length)).Where(g => g.ClassificationType.IsOfType(PkgdefClassificationTypes.Guid));
            Dictionary<string, Tuple<string, ImageSource>> dic = new Dictionary<string, Tuple<string, ImageSource>>();
            var unknown = _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupModule, StandardGlyphItem.TotalGlyphItems);

            foreach (var cspan in guidSpans)
            {
                string guid = cspan.Span.GetText();
                string lineText = snapshot.GetLineFromPosition(cspan.Span.Start.Position).GetText();
                Guid check;

                if (!dic.ContainsKey(guid) && Guid.TryParse(guid, out check))
                {
                    dic[guid] = null;
                }

                if (lineText.Contains("\"Package\"") || lineText.Contains("\\Packages\\") || lineText.Contains("\\AutoLoadPackages\\") || lineText.Contains("\"ResourcePackage\"") || lineText.Contains("\"ProjectFactoryPackage\""))
                    dic[guid] = Tuple.Create("Package", _glyphService.GetGlyph(StandardGlyphGroup.GlyphJSharpProject, StandardGlyphItem.GlyphItemPublic));

                if (lineText.Contains("\"LinkedEditorGuid\"") || lineText.Contains("\\Editors\\"))
                    dic[guid] = Tuple.Create("Editor", _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupStruct, StandardGlyphItem.TotalGlyphItems));

                if (lineText.Contains("\"Page\"") || lineText.Contains("\"Category\""))
                    dic[guid] = Tuple.Create("Options page", _glyphService.GetGlyph(StandardGlyphGroup.GlyphDialogId, StandardGlyphItem.GlyphItemPublic));

                if (lineText.Contains("\\Services\\"))
                    dic[guid] = Tuple.Create("Service", _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.TotalGlyphItems));

                if (lineText.Contains("\\codeBase\\"))
                    dic[guid] = Tuple.Create("Assembly", _glyphService.GetGlyph(StandardGlyphGroup.GlyphAssembly, StandardGlyphItem.GlyphItemFriend));

                if (lineText.Contains("\\Projects\\"))
                    dic[guid] = Tuple.Create("Project", _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupModule, StandardGlyphItem.GlyphItemFriend));

                if (lineText.Contains("\\TemplateDirs\\"))
                    dic[guid] = Tuple.Create("Template", _glyphService.GetGlyph(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemFriend));

                if (lineText.Contains("\\CLSID\\"))
                    dic[guid] = Tuple.Create("CLSID", unknown);

                if (lineText.Contains("\\ProjectGenerators\\"))
                    dic[guid] = Tuple.Create("Project generators", unknown);
            }

            foreach (string guid in dic.Keys)
            {
                string description = "Unknown type";
                ImageSource glyph = unknown;

                if (dic[guid] != null)
                {
                    description = dic[guid].Item1;

                    if (dic[guid].Item2 != null)
                        glyph = dic[guid].Item2;
                }

                list.Add(CreateCompletion(guid, guid, glyph, description));
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