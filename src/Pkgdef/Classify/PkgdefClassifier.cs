using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class PkgdefClassifier : IClassifier
    {
        private IClassificationType _entryKey, _comment, _registryPath, _string, _operator, _keyword, _guid;

        public PkgdefClassifier(IClassificationTypeRegistryService registry)
        {
            _entryKey = registry.GetClassificationType(PkgdefClassificationTypes.EntryKey);
            _comment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            _registryPath = registry.GetClassificationType(PkgdefClassificationTypes.RegistryPath);
            _string = registry.GetClassificationType(PredefinedClassificationTypeNames.String);
            _operator = registry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
            _keyword = registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolDefinition);
            _guid = registry.GetClassificationType(PkgdefClassificationTypes.Guid);
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span2)
        {
            IList<ClassificationSpan> list = new List<ClassificationSpan>();
            var lines = span2.Snapshot.Lines;

            foreach (var line in lines)
            {
                SnapshotSpan span = line.Extent;
                string text = span.GetText();                
                bool isCommentLine = false;

                foreach (Match match in Variables.Comment.Matches(text))
                {
                    var comment = match.Groups["comment"];
                    SnapshotSpan commentSpan = new SnapshotSpan(span.Snapshot, span.Start + comment.Index, comment.Length);
                    list.Add(new ClassificationSpan(commentSpan, _comment));

                    isCommentLine = match.Index == 0;
                }

                if (isCommentLine)
                    continue;

                foreach (Match match in Variables.Keyword.Matches(text))
                {
                    SnapshotSpan keywordSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                    list.Add(new ClassificationSpan(keywordSpan, _keyword));
                }

                foreach (Match match in Variables.RegistryPath.Matches(text))
                {
                    SnapshotSpan regSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                    list.Add(new ClassificationSpan(regSpan, _registryPath));
                }

                foreach (Match match in Variables.String.Matches(text))
                {
                    SnapshotSpan stringSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                    list.Add(new ClassificationSpan(stringSpan, _string));
                }

                foreach (Match match in Variables.Guid.Matches(text))
                {
                    SnapshotSpan guidSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                    list.Add(new ClassificationSpan(guidSpan, _guid));
                }

                foreach (Match match in Variables.EntryKey.Matches(text))
                {
                    var dword = match.Groups["dword"];
                    SnapshotSpan dwordSpan = new SnapshotSpan(span.Snapshot, span.Start + dword.Index, dword.Length);
                    list.Add(new ClassificationSpan(dwordSpan, _entryKey));

                    var equals = match.Groups["operator"];
                    SnapshotSpan equalsSpan = new SnapshotSpan(span.Snapshot, span.Start + equals.Index, equals.Length);
                    list.Add(new ClassificationSpan(equalsSpan, _operator));
                }
            }

            return list;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add { }
            remove { }
        }
    }
}
