using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_
{
    partial class Terminal
    {
        public ScrollRect scrollview;
        public RectTransform rT_scrollview, rT_stdin;
        [SerializeField] internal InputText input_stdout, input_realtime, input_prefixe, input_stdin;

        public RectTransform rT_selection;
        public RawImage img_selection;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeUI()
        {
            rT_scrollview = (RectTransform)transform.Find("rT/body/scroll_view");
            scrollview = rT_scrollview.GetComponent<ScrollRect>();

            input_realtime = transform.Find("rT/body/realtime").GetComponent<InputText>();

            input_stdout = scrollview.content.Find("stdout").GetComponent<InputText>();
            rT_stdin = (RectTransform)scrollview.content.Find("stdin");
            input_prefixe = rT_stdin.Find("prefixe").GetComponent<InputText>();
            input_stdin = rT_stdin.Find("in").GetComponent<InputText>();

            input_stdout.type = InputText.Types.Stdout;
            input_realtime.type = InputText.Types.Realtime;
            input_prefixe.type = InputText.Types.Prefixe;
            input_stdin.type = InputText.Types.Stdin;

            rT_selection = (RectTransform)transform.Find("rT/body/selection");
            img_selection = rT_selection.GetComponent<RawImage>();

            input_stdin.input_field.onValidateInput += OnValidateStdin;
        }

        //--------------------------------------------------------------------------------------------------------------


    }
}