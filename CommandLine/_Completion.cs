using System.Collections.Generic;
using System;

namespace _COBALT_
{
    partial class CommandLine
    {
        public void ComputeCompletion_tab(in string argument, in IEnumerable<string> candidates)
        {
            List<string> list = ECompletionCandidates_tab(argument, candidates);
            if (list.Count == 0)
                return;

            string candidate = list[cpl_index % list.Count];
            text = text[..start_i] + candidate + text[read_i..];
            cursor_i = start_i + candidate.Length;
        }

        List<string> ECompletionCandidates_tab(string argument, IEnumerable<string> candidates)
        {
            List<string> list = new();
            foreach (string candidate in candidates)
            {
                int last = 0, ic = 0, matches = 0;
                while (ic < argument.Length)
                {
                    int i = candidate.IndexOf(argument[ic..++ic], last, candidate.Length - last, StringComparison.OrdinalIgnoreCase);
                    if (i >= 0)
                    {
                        last = i + 1;
                        ++matches;
                    }
                }
                if (matches == argument.Length)
                    list.Add(candidate);
            }
            return list;
        }
    }
}