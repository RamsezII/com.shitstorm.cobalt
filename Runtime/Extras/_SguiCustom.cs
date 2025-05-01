using System;
using System.Collections.Generic;
using _COBRA_;
using _SGUI_;
using _UTIL_;

namespace _COBALT_
{
    partial class CmdUI
    {
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
                        SguiCustomButton.Infos infos = new();

                        if (exe.line.TryReadArgument(out string arg, out _))
                            infos.label = new(arg);

                        Type type = null;

                        switch (flag.ToLower())
                        {
                            case opt_slider:
                                type = typeof(SguiCustomButton_Slider);
                                break;

                            case opt_input:
                                type = typeof(SguiCustomButton_InputField);
                                break;

                            case opt_dropdown:
                                type = typeof(SguiCustomButton_Dropdown);
                                {
                                    infos.items = new();
                                    while (exe.line.TryRead_one_of_the_flags(exe, out _, opt_i, opt_item))
                                        if (exe.line.TryReadArgument(out string item, out _))
                                            infos.items.Add(new(item));
                                }
                                break;
                        }

                        exe.opts.Add(opt_counter++.ToString(), (type, infos));
                    }
                },
                routine: ERoutine
            );

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                SguiCustom clone = SguiWindow.InstantiateWindow<SguiCustom>();

                foreach (var value in exe.opts.Values)
                {
                    var (type, infos) = ((Type type, SguiCustomButton.Infos infos))value;
                    SguiCustomButton button = clone.AddButton(type);

                    switch (button)
                    {
                        case SguiCustomButton_Dropdown dropdown:
                            dropdown.dropdown.AddOptions(infos.items);
                            break;
                    }
                }

                while (clone != null)
                {
                    if (exe.line.signal.HasFlag(SIGNALS.KILL))
                    {
                        clone.Oblivionize();
                        clone = null;
                        yield break;
                    }
                    yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                }
            }
        }
    }
}