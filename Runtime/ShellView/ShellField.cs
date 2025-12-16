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

        protected override void Awake()
        {
            scrollview = GetComponentInParent<ScrollRect>();
            rT = (RectTransform)transform;
            lint = transform.Find("area/lint").GetComponent<TextMeshProUGUI>();

            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            scrollview.OnScroll(eventData);
        }
    }
}