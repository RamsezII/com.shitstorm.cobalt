using System.Collections.Generic;
using _COBRA_;
using _SGUI_;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_SguiCustom()
        {
            const string
                opt_slider = "--slider",
                opt_input = "--input-field",
                opt_dropdown = "--dropdown";

            Command.static_domain.AddRoutine(
                "open-custom",
                opts: static exe =>
                {
                    int opt_i = 0;
                    while (exe.line.TryRead_one_of_the_flags(exe, out string flag, opt_slider, opt_input, opt_dropdown))
                    {
                        SguiCustomButton.Infos infos = new();

                        switch (flag.ToLower())
                        {
                            case opt_slider:
                                infos.code = SguiCustomButton.Codes.Slider;
                                break;

                            case opt_input:
                                infos.code = SguiCustomButton.Codes.InputField;
                                break;

                            case opt_dropdown:
                                infos.code = SguiCustomButton.Codes.Dropdown;
                                break;
                        }

                        if (infos.code != SguiCustomButton.Codes._last_)
                            exe.opts.Add(opt_i++.ToString(), infos);
                    }
                },
                routine: ERoutine
            );

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                SguiCustom clone = SguiWindow.InstantiateWindow<SguiCustom>();

                foreach (var pair in exe.opts)
                    clone.AddButton((SguiCustomButton.Infos)pair.Value);

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