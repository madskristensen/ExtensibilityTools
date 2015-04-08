using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class PkgdefClassifier : IClassifier
    {
        private IClassificationType _dword, _comment, _regkey, _string, _equals, _keyword;

        public PkgdefClassifier(IClassificationTypeRegistryService registry)
        {
            _dword = registry.GetClassificationType(PkgdefClassificationTypes.Dword);
            _comment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            _regkey = registry.GetClassificationType(PkgdefClassificationTypes.RegKey);
            _string = registry.GetClassificationType(PredefinedClassificationTypeNames.String);
            _equals = registry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
            _keyword = registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolDefinition);
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> list = new List<ClassificationSpan>();

            string text = span.GetText();

            foreach (Match match in Variables.Comment.Matches(text))
            {
                var comment = match.Groups["comment"];
                SnapshotSpan commentSpan = new SnapshotSpan(span.Snapshot, span.Start + comment.Index, comment.Length);
                list.Add(new ClassificationSpan(commentSpan, _comment));

                if (match.Index == 0)
                    return list;
            }

            foreach (Match match in Variables.Keyword.Matches(text))
            {
                SnapshotSpan keywordSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                list.Add(new ClassificationSpan(keywordSpan, _keyword));
            }

            foreach (Match match in Variables.RegKey.Matches(text))
            {
                SnapshotSpan regSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                list.Add(new ClassificationSpan(regSpan, _regkey));
            }

            foreach (Match match in Variables.String.Matches(text))
            {
                SnapshotSpan stringSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                list.Add(new ClassificationSpan(stringSpan, _string));
            }

            foreach (Match match in Variables.Dword.Matches(text))
            {
                var dword = match.Groups["dword"];
                SnapshotSpan dwordSpan = new SnapshotSpan(span.Snapshot, span.Start + dword.Index, dword.Length);
                list.Add(new ClassificationSpan(dwordSpan, _dword));

                var equals = match.Groups["operator"];
                SnapshotSpan equalsSpan = new SnapshotSpan(span.Snapshot, span.Start + equals.Index, equals.Length);
                list.Add(new ClassificationSpan(equalsSpan, _equals));
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
