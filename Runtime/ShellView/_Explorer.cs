using _ARK_;
using _SGUI_;
using _SGUI_.context_click;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitExplorerExtensions()
        {
            SguiExplorerView.onContextClick_file += (ContextList list, FileInfo file) =>
            {
                var button = list.AddButton_trad(new()
                {
                    french = "Éxécuter dans un terminal",
                    english = "Execute a terminal",
                });

                button._button.onClick.AddListener(() =>
                {
                    SguiTerminal terminal = (SguiTerminal)OSView.instance.softwaresButtons[typeof(SguiTerminal)].InstantiateSoftware();
                    NUCLEOR.instance.routinizer.AddRoutine(Util.EWaitForFrames(3, "execute in a terminal", terminal, () =>
                    {
                        string line = $"run_script \"{file.FullName.NormalizePath()}\"";
                        terminal.shellView.ExecuteLine(line);
                    }));
                });
            };
        }
    }
}