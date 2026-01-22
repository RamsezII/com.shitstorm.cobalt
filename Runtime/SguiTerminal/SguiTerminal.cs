using _ARK_;
using _SGUI_;
using _SGUI_.context_click;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : SguiWindow1
    {
        static readonly List<SguiTerminal> selected_stack = new();

        public ShellView shellView;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            selected_stack.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            var button = OSView.instance.AddSoftwareButton<SguiTerminal>(new("Terminal"));

            ArkShortcuts.AddShortcut<Keyboard>(
                shortcutName: typeof(SguiTerminal).FullName,
                action: () =>
                {
                    foreach (var inst in instances._collection)
                        if (inst is SguiTerminal term)
                        {
                            OSView.instance.ToggleSelf(true);
                            term.TakeFocus();
                            return;
                        }
                    button.InstantiateSoftware();
                },
                bindings: "o"
            );

            SguiExplorerView.onContextClick_file += (ContextList list, FileInfo file) =>
            {
                var button = list.AddButton(new()
                {
                    french = "Éxécuter dans un terminal",
                    english = "Execute a terminal",
                });

                button.button.onClick.AddListener(() =>
                {
                    SguiTerminal terminal = (SguiTerminal)OSView.instance.softwaresButtons[typeof(SguiTerminal)].InstantiateSoftware();
                    NUCLEOR.instance.sequencer_parallel.AddRoutine(Util.EWaitForFrames(3, "execute in a terminal", terminal, () =>
                    {
                        string line = $"run_script \"{file.FullName.NormalizePath()}\"";
                        terminal.shellView.ExecuteLine(line);
                    }));
                });
            };

            SguiExplorerView.onContextClick_directory += (ContextList list, DirectoryInfo dir) =>
            {
                {
                    var button = list.AddButton(new()
                    {
                        french = $"Ouvrir ce dossier dans",
                        english = $"Open this directory in",
                    });

                    button.SetupSublist(sublist =>
                    {
                        {
                            var button = sublist.AddButton_label("Shitpad");
                        }

                        {
                            var button = sublist.AddButton_label("Shitcodium");
                        }
                    });
                }
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            shellView = GetComponentInChildren<ShellView>(true);
            base.OnAwake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();
            selected_stack.Remove(this);
            selected_stack.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            selected_stack.Remove(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            shellView.stdin_field.Select();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnFocus(in bool has_focus)
        {
            base.OnFocus(has_focus);
            if (has_focus)
                NUCLEOR.instance.sequencer_parallel.AddRoutine(Util.EWaitForFrames(2, "select stdinfield on focus", this, shellView.stdin_field.Select));
        }

        public override void OnResized()
        {
            base.OnResized();
            shellView.ResizeStdin();
        }
    }
}