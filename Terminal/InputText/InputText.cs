using TMPro;
using UnityEngine;

namespace _COBALT_
{
    internal class InputText : MonoBehaviour
    {
        public enum Types { Stdout, Realtime, Prefixe, Stdin, }

        [HideInInspector] public RectTransform rT;
        [HideInInspector] public TMP_InputField input_field;

        public Types type;
        public float text_height;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            rT = (RectTransform)transform;
            input_field = GetComponent<TMP_InputField>();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void AutoSize(in bool apply)
        {
            if (string.IsNullOrWhiteSpace(input_field.text))
                text_height = 0;
            else
                text_height = input_field.textComponent.preferredHeight;

            if (apply)
                rT.sizeDelta = new(0, text_height);
        }
    }
}