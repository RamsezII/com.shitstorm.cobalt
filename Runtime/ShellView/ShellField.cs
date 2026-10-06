using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _COBALT_
{
    public sealed class ShellField : TMP_InputField
    {
        ScrollRect scrollview;
        public RectTransform rT;
        public TextMeshProUGUI lint;

        //----------------------------------------------------------------------------------------------------------

        internal void Initialize()
        {
            scrollview = GetComponentInParent<ScrollRect>(true);
            rT = (RectTransform)transform;
            lint = transform.Find("area/lint").GetComponent<TextMeshProUGUI>();
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            scrollview.OnScroll(eventData);
        }
    }
}