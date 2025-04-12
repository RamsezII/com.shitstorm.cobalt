using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal class InputText : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        [HideInInspector] public Terminal terminal;
        [HideInInspector] public RectTransform rT, rT_parent;
        [HideInInspector] public TMP_InputField input_field;

        public float text_height;

        //--------------------------------------------------------------------------------------------------------------

        public void AwakeUI()
        {
            terminal = GetComponentInParent<Terminal>();
            rT = (RectTransform)transform;
            rT_parent = (RectTransform)transform.parent;
            input_field = GetComponent<TMP_InputField>();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void AutoSize(in bool apply)
        {
            if (string.IsNullOrWhiteSpace(input_field.text))
                text_height = 0;
            else
                text_height = input_field.textComponent.GetPreferredValues(input_field.text, rT_parent.rect.width, float.PositiveInfinity).y;

            if (apply)
                rT.sizeDelta = new(rT.sizeDelta.x, text_height);
        }

        public void ResetText()
        {
            if (!string.IsNullOrEmpty(input_field.text))
                input_field.text = string.Empty;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            terminal.OnTextClick(this, eventData, true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            terminal.OnTextClick(this, eventData, false);
        }
    }
}