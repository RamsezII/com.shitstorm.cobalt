#if UNITY_EDITOR
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        [ContextMenu(nameof(OnValidate))]
        private void OnValidate()
        {
            if (didStart)
                lint_theme.RebuildDictionary();
        }
    }
}
#endif