using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace MadsKristensen.ExtensibilityTools.Vsct
{
    class VsctCompletionSet : CompletionSet
    {
        private List<Completion> _allCompletions;
        private List<Completion> _filteredCompletions;

        public VsctCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, List<Completion> completions, IEnumerable<Completion> completionBuilders)
            : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
            _allCompletions = completions;
            _filteredCompletions = completions;
        }

        public override IList<Completion> Completions
        {
            get
            {
                return _filteredCompletions;
            }
        }

        public override void SelectBestMatch()
        {
            ITextSnapshot snapshot = ApplicableTo.TextBuffer.CurrentSnapshot;
            string typedText = ApplicableTo.GetText(snapshot).Trim();

            if (string.IsNullOrWhiteSpace(typedText))
            {
                if (_filteredCompletions.Any())
                    SelectionStatus = new CompletionSelectionStatus(_filteredCompletions.First(), true, true);

                return;
            }   

            foreach (Completion comp in _filteredCompletions)
            {
                int index = comp.DisplayText.IndexOf(typedText, StringComparison.OrdinalIgnoreCase);

                if (index == 0)
                {
                    SelectionStatus = new CompletionSelectionStatus(comp, true, true);
                    return;
                }
                else if (index > -1)
                {
                    SelectionStatus = new CompletionSelectionStatus(comp, true, true);
                    return;
                }
            }
        }

        public override void Filter()
        {
            ITextSnapshot snapshot = ApplicableTo.TextBuffer.CurrentSnapshot;
            string typedText = ApplicableTo.GetText(snapshot).Trim();

            List<Completion> temp = new List<Completion>();

            foreach (Completion comp in _allCompletions)
            {
                int index = comp.DisplayText.IndexOf(typedText, StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                    temp.Add(comp);
            }

            _filteredCompletions = temp;
        }
    }
}
