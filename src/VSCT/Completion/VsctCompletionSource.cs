using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Shell.Interop;
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
        private static IVsImageService2 _imageService;

        public VsctCompletionSource(ITextBuffer buffer, IClassifierAggregatorService classifier, ITextStructureNavigatorSelectorService navigator, IGlyphService glyphService)
        {
            _buffer = buffer;
            _classifier = classifier.GetClassifier(buffer);
            _navigator = navigator;
            _glyphService = glyphService;
            _defaultGlyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic);
            _builtInGlyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.TotalGlyphItems);
            _imageService = ExtensibilityToolsPackage.GetGlobalService(typeof(SVsImageService)) as IVsImageService2;
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
                    //string id = null;
                    //string attrName = null;
                    string lineText = line.GetText();

                    //Match idMatch = Regex.Match(lineText, " id=\"(?<id>[^\"]+)\"");

                    //if (idMatch.Success)
                    //    id = idMatch.Groups["id"].Value;

                    Match guidMatch = Regex.Match(lineText, " guid=\"(?<guid>[^\"]+)\"");

                    if (guidMatch.Success)
                        guid = guidMatch.Groups["guid"].Value;

                    XmlDocument doc = ReadXmlDocument(snapshot);

                    if (doc == null)
                        return;

                    if ((current == "id" || current == "idCommandList"))
                    {
                        if (guid == "guidSHLMainMenu")
                        {
                            foreach (string key in VsctBuiltInCache._dic.Keys)
                            {
                                list.Add(CreateCompletion(key, key, _builtInGlyph, VsctBuiltInCache._dic[key]));
                            }
                        }
                        else if (guid == "ImageCatalogGuid" && _imageService != null)
                        {
                            PropertyInfo[] monikers = typeof(KnownMonikers).GetProperties(BindingFlags.Static | BindingFlags.Public);
                            foreach (var monikerName in monikers)
                            {
                                ImageMoniker moniker = (ImageMoniker)monikerName.GetValue(null, null);
                                var glyph = GetImage(moniker);
                                list.Add(CreateCompletion(monikerName.Name, monikerName.Name, glyph));
                            }
                        }
                        else
                        {
                            GetNameValueCompletion(list, doc, "//GuidSymbol[@name='" + guid + "']//IDSymbol");
                        }
                    }
                    else if (current == "guid" || current == "package")
                    {
                        list.Add(CreateCompletion("guidSHLMainMenu", "guidSHLMainMenu", _builtInGlyph, "Guid for Shell's group and menu ids."));
                        GetNameValueCompletion(list, doc, "//GuidSymbol");
                    }
                    else if (current == "editor")
                    {
                        list.Add(CreateCompletion("guidVSStd97", "guidVSStd97", _builtInGlyph));
                        list.Add(CreateCompletion("guidVSStd2K", "guidVSStd2K", _builtInGlyph));
                        GetNameValueCompletion(list, doc, "//GuidSymbol");
                    }
                    else if (current == "usedList")
                    {
                        extent = span.Span;
                        GetUsedListCompletion(list, doc, "//GuidSymbol[@name='" + guid + "']//IDSymbol");
                    }
                    else if (current == "href")
                    {
                        list.Add(CreateCompletion("stdidcmd.h", "stdidcmd.h", _defaultGlyph));
                        list.Add(CreateCompletion("vsshlids.h", "vsshlids.h", _defaultGlyph));
                        list.Add(CreateCompletion("KnownImageIds.vsct", "KnownImageIds.vsct", _defaultGlyph));
                    }
                    else if (current == "context")
                    {
                        list.Add(CreateCompletion("GUID_TextEditorFactory", "GUID_TextEditorFactory", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_CodeWindow", "UICONTEXT_CodeWindow", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_Debugging", "UICONTEXT_Debugging", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_DesignMode", "UICONTEXT_DesignMode", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_Dragging", "UICONTEXT_Dragging", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_EmptySolution", "UICONTEXT_EmptySolution", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_FullScreenMode", "UICONTEXT_FullScreenMode", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_NoSolution", "UICONTEXT_NoSolution", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_NotBuildingAndNotDebugging", "UICONTEXT_NotBuildingAndNotDebugging", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_SolutionBuilding", "UICONTEXT_SolutionBuilding", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_SolutionExists", "UICONTEXT_SolutionExists", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_SolutionExistsAndNotBuildingAndNotDebugging", "UICONTEXT_SolutionExistsAndNotBuildingAndNotDebugging", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_SolutionHasMultipleProjects", "UICONTEXT_SolutionHasMultipleProjects", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_SolutionHasSingleProject", "UICONTEXT_SolutionHasSingleProject", _defaultGlyph));
                        list.Add(CreateCompletion("UICONTEXT_ToolboxInitialized", "UICONTEXT_ToolboxInitialized", _defaultGlyph));
                    }
                }
            }

            if (list.Count > 0)
            {
                var applicableTo = snapshot.CreateTrackingSpan(extent, SpanTrackingMode.EdgeInclusive);
                completionSets.Add(new VsctCompletionSet("All", "All", applicableTo, list, Enumerable.Empty<Intel.Completion>()));
            }
        }

        public static ImageSource GetImage(ImageMoniker moniker)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags;
            imageAttributes.ImageType = (uint)_UIImageType.IT_Bitmap;
            imageAttributes.Format = (uint)_UIDataFormat.DF_WPF;
            imageAttributes.LogicalHeight = 16;
            imageAttributes.LogicalWidth = 16;
            imageAttributes.StructSize = Marshal.SizeOf(typeof(ImageAttributes));

            IVsUIObject result = _imageService.GetImage(moniker, imageAttributes);

            Object data;
            result.get_Data(out data);

            if (data == null)
                return null;

            return data as System.Windows.Media.Imaging.BitmapSource;
        }

        private static XmlDocument ReadXmlDocument(ITextSnapshot snapshot)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(Regex.Replace(snapshot.GetText(), " xmlns(:[^\"]+)?=\"([^\"]+)\"", string.Empty));
                return doc;
            }
            catch (Exception)
            {
                return null;
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

        private void GetUsedListCompletion(List<Intel.Completion> list, XmlDocument doc, string xpath)
        {
            var nodes = doc.SelectNodes(xpath);
            if (nodes.Count == 0)
                return;

            List<string> names = new List<string>();

            foreach (XmlNode node in nodes)
            {
                XmlAttribute name = node.Attributes["name"];

                if (name != null)
                {
                    names.Add(name.InnerText);
                }
            }

            string displayText = String.Join(", ", names);
            list.Add(CreateCompletion(displayText, displayText, _defaultGlyph));
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