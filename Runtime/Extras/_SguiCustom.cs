using System;
using System.Collections.Generic;
using _COBRA_;
using _SGUI_;
using _UTIL_;
using TMPro;

namespace _COBALT_
{
    partial class CmdUI
    {
        public struct CustomButtonInfos
        {
            public Traductions label;
            public Type type;
            public List<TMP_Dropdown.OptionData> items;
        }

        //--------------------------------------------------------------------------------------------------------------

        static void Init_SguiCustom()
        {
            const string
                opt_slider = "--slider",
                opt_input = "--input-field",
                opt_dropdown = "--dropdown",
                opt_item = "--item",
                opt_i = "-i";

            Command.static_domain.AddRoutine(
                "open-custom",
                opts: static exe =>
                {
                    int opt_counter = 0;
                    while (exe.line.TryRead_one_of_the_flags(exe, out string flag, opt_slider, opt_input, opt_dropdown))
                    {
                        CustomButtonInfos infos = new();

                        if (exe.line.TryReadArgument(out string arg, out _))
                            infos.label = new(arg);

                        switch (flag.ToLower())
                        {
                            case opt_slider:
                                infos.type = typeof(SguiCustomButton_Slider);
                                break;

                            case opt_input:
                                infos.type = typeof(SguiCustomButton_InputField);
                                break;

                            case opt_dropdown:
                                infos.type = typeof(SguiCustomButton_Dropdown);
                                {
                                    infos.items = new();
                                    while (exe.line.TryRead_one_of_the_flags(exe, out _, opt_i, opt_item))
                                        if (exe.line.TryReadArgument(out string item, out _))
                                            infos.items.Add(new(item));
                                }
                                break;
                        }

                        exe.opts.Add(opt_counter++.ToString(), infos);
                    }
                },
                routine: ERoutine
            );

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                SguiCustom clone = SguiWindow.InstantiateWindow<SguiCustom>();

                clone.onButton_confirm += () => exe.Stdout(clone.GetResults());

                foreach (var value in exe.opts.Values)
                {
                    CustomButtonInfos infos = (CustomButtonInfos)value;
                    SguiCustomButton button = clone.AddButton(infos.type);

                    button.label.SetTrads(infos.label);

                    switch (button)
                    {
                        case SguiCustomButton_Slider slider:
                            break;

                        case SguiCustomButton_InputField input:
                            break;

                        case SguiCustomButton_Dropdown dropdown:
                            dropdown.dropdown.AddOptions(infos.items);
                            break;
                    }
                }

                try
                {
                    while (clone != null && !clone.oblivionized)
                    {
                        if (exe.line.signal.HasFlag(SIGNALS.KILL))
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