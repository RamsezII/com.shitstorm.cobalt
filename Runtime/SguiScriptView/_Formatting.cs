using _SGUI_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        float formatShortcutStartedAt = float.NegativeInfinity;

        //----------------------------------------------------------------------------------------------------------

        void FormatDocument()
        {
            try
            {
                string source = input_field.text;
                int anchor = input_field.selectionStringAnchorPosition;
                int focus = input_field.selectionStringFocusPosition;
                string formatted = _BOA_.Shell.FormatCode(source, ref anchor, ref focus);
                if (formatted == source)
                    return;

                SguiCompletor.instance?.ResetIntellisense();
                input_field.SetTextWithoutNotify(formatted);
                input_field.selectionStringAnchorPosition = anchor;
                input_field.selectionStringFocusPosition = focus;
                input_field.ForceLabelUpdate();
                OnValueChanged(formatted);
            }
            catch (Exception exception)
            {
                text_error.gameObject.SetActive(true);
                text_error.text = exception.Message;
                Debug.LogException(exception, this);
            }
        }
    }
}
