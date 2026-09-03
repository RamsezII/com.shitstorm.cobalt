using _ARK_;
using _UTIL_;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView : IHomeTexts
    {
        [NJEdit] readonly List<string> history = new(history_max);

        const byte history_max = 50;
        [SerializeField] int history_index = -1;

        //--------------------------------------------------------------------------------------------------------------

        void AddToHistory(in string line)
        {
            if (history.Contains(line))
                history.Remove(line);
            else if (history.Count >= history_max)
                history.RemoveAt(0);

            history.Add(line);

            foreach (ShellView shell_view in instances)
                if (shell_view != null)
                    shell_view.ResetHistoryNav();
        }

        void ResetHistoryNav() => history_index = history.Count;

        bool TryNavHistory(in int increment, out string value)
        {
            if (history.Count == 0)
            {
                history_index = -1;
                value = null;
                return false;
            }

            int count_mod = 1 + history.Count;

            history_index += increment;

            history_index %= count_mod;
            if (history_index < 0)
                history_index += count_mod;

            if (history_index == history.Count)
                value = string.Empty;
            else
                value = history[history_index];

            return true;
        }
    }
}
