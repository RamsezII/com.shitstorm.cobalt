#if UNITY_EDITOR
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        [ContextMenu(nameof(OnValidate))]
        private void OnValidate()
        {
            if (didStart)
                _OnValidate();
        }

        protected virtual void _OnValidate()
        {
            OnChange(input_field.text);
        }
    }
}
#endif