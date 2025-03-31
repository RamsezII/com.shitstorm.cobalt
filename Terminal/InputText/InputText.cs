using TMPro;
using UnityEngine;

namespace _COBALT_
{
    internal class InputText : MonoBehaviour
    {
        public enum Types { Stdout, Realtime, Prefixe, Stdin, }

        public Types type;
        [HideInInspector] public Terminal terminal;
        [HideInInspector] public RectTransform rT, rT_parent;
        [HideInInspector] public TMP_InputField input_field;
        public float text_height;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            terminal = GetComponentInParent<Terminal>();
            rT = (RectTransform)transform;
            input_field = GetComponent<TMP_InputField>();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void AutoSize()
        {
            if (string.IsNullOrWhiteSpace(input_field.text))
                text_height = 0;
            else
                text_height = input_field.textComponent.preferredHeight;

            if (type == Types.Stdin)
            {
                float prefixe_width = terminal.input_prefixe.input_field.textComponent.GetPreferredValues(terminal.input_prefixe.input_field.text + "_", terminal.scroll_view.content.rect.width, float.PositiveInfinity).x;

                float line_height = input_field.textComponent.GetPreferredValues("A", terminal.scroll_view.content.rect.width - prefixe_width, float.PositiveInfinity).y;

                text_height += terminal.scroll_view.viewport.rect.height - line_height;

                rT.sizeDelta = new(-prefixe_width, text_height);

                terminal.rT_stdin.sizeDelta = new(0, text_height);
            }
            else if (type != Types.Prefixe)
                rT.sizeDelta = new(0, text_height);
        }
    }
}