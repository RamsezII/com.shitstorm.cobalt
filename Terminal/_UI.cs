using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_
{
    partial class Terminal
    {
        public ScrollRect scroll_view;
        public VerticalLayoutGroup content_layout;
        public RectTransform rT_stdin;
        [SerializeField] internal InputText input_stdout, input_realtime, input_prefixe, input_stdin;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeUI()
        {
            scroll_view = transform.Find("rT/body/scroll_view").GetComponent<ScrollRect>();
            content_layout = scroll_view.content.GetComponent<VerticalLayoutGroup>();

            input_stdout = content_layout.transform.Find("stdout").GetComponent<InputText>();
            input_realtime = content_layout.transform.Find("realtime").GetComponent<InputText>();
            rT_stdin = (RectTransform)content_layout.transform.Find("stdin");
            input_prefixe = rT_stdin.Find("prefixe").GetComponent<InputText>();
            input_stdin = rT_stdin.Find("in").GetComponent<InputText>();

            input_stdout.type = InputText.Types.Stdout;
            input_realtime.type = InputText.Types.Realtime;
            input_prefixe.type = InputText.Types.Prefixe;
            input_stdin.type = InputText.Types.Stdin;

            input_stdin.input_field.onValidateInput += OnValidateStdin;

            input_realtime.input_field.text = null;
        }

        //--------------------------------------------------------------------------------------------------------------


    }
}