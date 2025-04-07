﻿using _ARK_;
using _UTIL_;
using TMPro;
using UnityEngine;
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
        public readonly OnValue<float> font_size = new(1);

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(OnValidate))]
        private void OnValidate()
        {
            if (Application.isPlaying)
                flag_stdin.Update(true);
        }
#endif

        //--------------------------------------------------------------------------------------------------------------

        void AwakeUI()
        {
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
        }

        //--------------------------------------------------------------------------------------------------------------

        void StartUI()
        {
            initial_fontsize = input_stdin.input_field.textComponent.fontSize;

            input_realtime.input_field.text = null;

            flag_progress.Update(true);

            input_stdin.ResetText();
            flag_stdin.Update(true);

            input_stdin.input_field.onValueChanged.AddListener(OnChangeStdin);

            input_stdin.input_field.onValidateInput = OnValidateStdin;

            transform.Find("rT/header/buttons/close/button").GetComponent<Button>().onClick.AddListener(() => isActive.Update(false));
            transform.Find("rT/header/buttons/hide/button").GetComponent<Button>().onClick.AddListener(() => isActive.Update(false));

            MachineSettings.machine_name.AddListener(value => flag_stdin.Update(true));

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

                flag_stdout.Update(true);
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
    }
}