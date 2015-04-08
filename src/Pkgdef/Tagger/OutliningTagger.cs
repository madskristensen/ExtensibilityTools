using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType(PkgdefContentTypeDefinition.PkgdefContentType)]
    internal sealed class OutliningTagger : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new RegionTagger(buffer)) as ITagger<T>;
        }
    }

    internal sealed class RegionTagger : ITagger<IOutliningRegionTag>
    {
        string hoverText = "hover text"; //the contents of the tooltip for the collapsed span
        ITextBuffer buffer;
        ITextSnapshot snapshot;
        IEnumerable<Region> regions;

        public RegionTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.regions = new List<Region>();
            this.ReParse();
            this.buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            IEnumerable<Region> currentRegions = this.regions;
            ITextSnapshot currentSnapshot = this.snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;

            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber && region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    string text = startLine.GetText();

                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(currentSnapshot, region.StartOffset, region.EndOffset - region.StartOffset),
                        new OutliningRegionTag(false, true, text, hoverText));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e.After != buffer.CurrentSnapshot)
                return;

            this.ReParse();
        }

        void ReParse()
        {
            ITextSnapshot newSnapshot = buffer.CurrentSnapshot;
            List<Region> newRegions = new List<Region>();
            Region currentRegion = null;
            ITextSnapshotLine prev = null;

            foreach (var line in newSnapshot.Lines)
            {
                string text = line.GetText();

                if (!string.IsNullOrWhiteSpace(text) && text[0] == '[' && currentRegion == null)
                {
                    currentRegion = new Region()
                    {
                        StartLine = line.LineNumber,
                        StartOffset = line.Start.Position
                    };
                }
                else if (currentRegion != null)
                {
                    if (line.LineNumber == newSnapshot.LineCount - 1 && !string.IsNullOrWhiteSpace(text))
                    {
                        currentRegion.EndLine = line.LineNumber;
                        currentRegion.EndOffset = line.End.Position;
                        newRegions.Add(currentRegion);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(text) || text[0] == '[')
                    {
                        currentRegion.EndLine = prev.LineNumber;
                        currentRegion.EndOffset = prev.End.Position;
                        newRegions.Add(currentRegion);

                        currentRegion = null;
                    }

                    if (!string.IsNullOrWhiteSpace(text) && text[0] == '[')
                    {
                        currentRegion = new Region()
                        {
                            StartLine = line.LineNumber,
                            StartOffset = line.Start.Position
                        };
                    }
                }

                prev = line;
            }

            this.snapshot = newSnapshot;
            this.regions = newRegions.Where(line => line.StartLine != line.EndLine);

            if (!regions.Any())
                return;

            if (this.TagsChanged != null)
            {
                var start = regions.First().StartOffset;
                var end = regions.Last().EndOffset;
                this.TagsChanged(this, new SnapshotSpanEventArgs(
                    new SnapshotSpan(this.snapshot, Span.FromBounds(start, end - start))));
            }
        }
    }

    class Region
    {
        public int StartLine { get; set; }
        public int StartOffset { get; set; }
        public int EndLine { get; set; }
        public int EndOffset { get; set; }
    }
}
