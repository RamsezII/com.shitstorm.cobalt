using System;
using System.Collections.Generic;
using _COBRA_;
using _SGUI_;
using _ARK_;
using TMPro;

namespace _COBALT_
{
    partial class CmdUI
    {
        public struct CustomButtonInfos
        {
            public Traductions label;
            public Type type;
            public float slider_min, slider_max;
            public bool slider_is_int;
            public bool input_is_passward;
            public List<TMP_Dropdown.OptionData> dropdown_items;
        }

        //--------------------------------------------------------------------------------------------------------------

        static void Init_SguiCustom()
        {
            const string
                type_slider = "slider",
                type_input = "input-field",
                type_dropdown = "dropdown",
                type_toggle = "toggle",
                type_button = "button",
                opt_item = "--item",
                opt_i = "-i",
                opt_title = "--title",
                opt_t = "-t",
                flag_int = "--whole-numbers",
                opt_min = "--min",
                opt_max = "--max";

            Command.static_domain.AddRoutine(
                "sgui-custom",
                min_args: 1,
                max_args: 10,
                opts: static exe =>
                {
                    if (exe.line.TryRead_one_of_the_flags(exe, out _, opt_t, opt_title))
                        if (exe.line.TryReadArgument(out string arg, out _))
                            exe.opts.Add(opt_t, arg);
                },
                args: static exe =>
                {
                    while (exe.line.TryReadArgument(out string button_type, out _, strict: true, lint: false, completions: new string[] { type_slider, type_input, type_dropdown, type_toggle, type_button, }))
                    {
                        exe.line.LintToThisPosition(exe.line.linter.type);

                        CustomButtonInfos infos = new();

                        if (exe.line.TryReadArgument(out string label, out _))
                            infos.label = new(label);

                        switch (button_type.ToLower())
                        {
                            case type_slider:
                                infos.type = typeof(SguiCustom_Slider);
                                {
                                    if (exe.line.TryRead_one_flag(exe, flag_int))
                                        infos.slider_is_int = true;

                                    if (exe.line.TryRead_one_flag(exe, opt_min))
                                        exe.line.TryReadFloat(out infos.slider_min);

                                    if (exe.line.TryRead_one_flag(exe, opt_max))
                                        exe.line.TryReadFloat(out infos.slider_max);
                                }
                                break;

                            case type_input:
                                infos.type = typeof(SguiCustom_InputField);
                                if (exe.line.TryRead_one_flag(exe, "-p", "--password"))
                                    infos.input_is_passward = true;
                                break;

                            case type_dropdown:
                                infos.type = typeof(SguiCustom_Dropdown);
                                {
                                    infos.dropdown_items = new();
                                    while (exe.line.TryRead_one_of_the_flags(exe, out _, opt_i, opt_item))
                                        if (exe.line.TryReadArgument(out string item, out _))
                                            infos.dropdown_items.Add(new(item));
                                }
                                break;

                            case type_toggle:
                                infos.type = typeof(SguiCustom_Toggle);
                                break;

                            case type_button:
                                infos.type = typeof(SguiCustom_Button);
                                break;
                        }

                        exe.args.Add(infos);
                    }
                },
                routine: ERoutine
            );

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                SguiCustom clone = SguiWindow.InstantiateWindow<SguiCustom>();

                if (exe.opts.TryGetValue_str(opt_t, out string title))
                    clone.trad_title.SetTrad(title);

                clone.onAction_confirm += () =>
                {
                    List<object> results = new();

                    for (int i = 0; i < clone.clones.Count; i++)
                        switch (clone.clones[i])
                        {
                            case SguiCustom_Slider slider:
                                results.Add(slider.slider.value);
                                break;

                            case SguiCustom_InputField inputfield:
                                results.Add(inputfield.input_field.text);
                                break;

                            case SguiCustom_Dropdown dropdown:
                                results.Add(dropdown.dropdown.GetSelectedName());
                                break;

                            case SguiCustom_Toggle toggle:
                                results.Add(toggle.toggle.isOn);
                                break;

                            default:
                                results.Add(null);
                                break;
                        }

                    exe.Stdout(results);
                };

                foreach (var arg in exe.args)
                {
                    CustomButtonInfos infos = (CustomButtonInfos)arg;
                    SguiCustom_Abstract button = clone.AddButton(infos.type);

                    button.trad_label.SetTrads(infos.label);

                    switch (button)
                    {
                        case SguiCustom_Slider slider:
                            slider.slider.wholeNumbers = infos.slider_is_int;
                            slider.slider.minValue = infos.slider_min;
                            slider.slider.maxValue = infos.slider_max;
                            break;

                        case SguiCustom_InputField inputfield:
                            if (infos.input_is_passward)
                                inputfield.input_field.contentType = TMP_InputField.ContentType.Password;
                            break;

                        case SguiCustom_Dropdown dropdown:
                            dropdown.dropdown.AddOptions(infos.dropdown_items);
                            break;
                    }
                }

                try
                {
                    while (clone != null && !clone.oblivionized)
                    {
                        if (exe.line.flags.HasFlag(SIG_FLAGS.KILL))
                        {
                            clone.Oblivionize();
                            clone = null;
                            yield break;
                        }
                        yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                    }

                    if (clone != null)
                        clone = null;
                }
                finally
                {
                    if (clone != null)
                        clone.Oblivionize();
                }
            }
        }
    }
}