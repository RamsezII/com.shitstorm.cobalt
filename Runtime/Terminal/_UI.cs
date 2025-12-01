using _ARK_;
using _UTIL_;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _COBALT_
{
    partial class Terminal
    {
        public ScrollRect scrollview;
        public RectTransform rT_body, rT_scrollview, rT_stdin;
        [SerializeField] internal InputText input_stdout, input_realtime, input_prefixe, input_stdin;
        [SerializeField] TextMeshProUGUI linter_tmp;

        public RectTransform rT_selection;
        public RawImage img_selection;

        public float line_height;

        float initial_fontsize;
        public readonly ValueHandler<float> font_size = new(1);

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(OnValidate))]
        private void OnValidate()
        {
            if (Application.isPlaying)
                flag_stdin.Value = true;
        }
#endif

        //--------------------------------------------------------------------------------------------------------------

        void AwakeUI()
        {
            trad_title.SetTrad(typeof(Terminal).Name);

            rT_body = (RectTransform)transform.Find("rT/body");
            rT_scrollview = (RectTransform)rT_body.Find("scroll_view");
            scrollview = rT_scrollview.GetComponent<ScrollRect>();

            input_realtime = transform.Find("rT/body/realtime").GetComponent<InputText>();

            input_stdout = scrollview.content.Find("stdout").GetComponent<InputText>();
            rT_stdin = (RectTransform)scrollview.content.Find("stdin");
            input_prefixe = rT_stdin.Find("prefixe").GetComponent<InputText>();
            input_stdin = rT_stdin.Find("in").GetComponent<InputText>();

            linter_tmp = input_stdin.transform.Find("mask/text/lint").GetComponent<TextMeshProUGUI>();

            input_stdout.AwakeUI();
            input_realtime.AwakeUI();
            input_prefixe.AwakeUI();
            input_stdin.AwakeUI();

            rT_selection = (RectTransform)transform.Find("rT/body/selection");
            img_selection = rT_selection.GetComponent<RawImage>();

            Vector2 size = rt.rect.size;
            Vector2 psize = rt_parent.rect.size;
            rt.anchoredPosition = .5f * (psize - size);
        }

        //--------------------------------------------------------------------------------------------------------------

        void StartUI()
        {
            initial_fontsize = input_stdin.input_field.textComponent.fontSize;

            input_realtime.ResetText();
            input_stdin.ResetText();

            flag_progress.Value = true;
            flag_stdin.Value = true;

            input_stdin.input_field.onValueChanged.AddListener(OnChangeStdin);

            input_stdin.input_field.onValidateInput = OnValidateStdin;

            ArkMachine.user_name.AddListener(value => flag_stdin.Value = true);

            font_size.AddProcessor(value => Mathf.Clamp(value, .5f, 2));

            font_size.AddListener(value =>
            {
                value *= initial_fontsize;
                input_stdin.input_field.textComponent.fontSize = value;
                input_stdout.input_field.textComponent.fontSize = value;
                input_realtime.input_field.textComponent.fontSize = value;
                input_prefixe.input_field.textComponent.fontSize = value;
                linter_tmp.fontSize = value;
                input_stdin.input_field.textComponent.fontSize = value;

                flag_stdout.Value = true;
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnUpdateAlpha()
        {
            base.OnUpdateAlpha();

            Color text_color = new(1, 1, 1, anim_alpha);

            input_stdout.input_field.textComponent.color = text_color;
            input_realtime.input_field.textComponent.color = text_color;
            input_prefixe.input_field.textComponent.color = text_color;
            linter_tmp.color = text_color;
        }

        internal void OnTextClick(in InputText input_text, in PointerEventData eventData, in bool is_down)
        {
            return;
            ForceSelectStdin();
        }

        public void ForceSelectStdin()
        {
            input_stdin.input_field.ActivateInputField();
            input_stdin.input_field.Select();
        }
    }
}