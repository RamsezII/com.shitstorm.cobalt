using TMPro;
using UnityEngine;

namespace _COBALT_
{
    internal class InputText : MonoBehaviour
    {
        [HideInInspector] public RectTransform rT, parent_body_rT;
        [HideInInspector] public TMP_InputField input_field;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            rT = (RectTransform)transform;
            parent_body_rT = (RectTransform)rT.parent;
            input_field = GetComponent<TMP_InputField>();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void AutoSize(in string text)
        {
            Vector2 preferred_values = input_field.textComponent.GetPreferredValues(text, parent_body_rT.rect.width, float.PositiveInfinity);
            rT.sizeDelta = preferred_values;
        }
    }
}