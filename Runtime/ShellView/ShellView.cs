using _ARK_;
using _COBRA_;
using _SGUI_;
using _SGUI_.composer;
using _SGUI_.context_click;
using System.Collections.Generic;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _COBALT_
{
    public sealed partial class ShellView : SguiFrame, SguiDragManager.IAcceptDraggable
    {
        [AutoStaticsCleanup] public new static readonly HashSet<ShellView> instances = new();

        public ShellField stdout_field, stdin_field;
        public TextMeshProUGUI tmp_progress;
        public ScrollRect scrollview;

        [SerializeField] float stdin_h, stdout_h;
        [SerializeField] bool flag_history;

        float
            offset_top_h = 2,
            offset_bottom_h = 5;

        public LintTheme lint_theme = LintTheme.theme_dark;
        public Shell shell;

        //----------------------------------------------------------------------------------------------------------

        protected override void OnInitialize()
        {
            base.OnInitialize();

            shell?.Dispose();
            shell = null;

            instances.Add(this);

            stdout_field.Initialize();
            stdin_field.Initialize();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();

            if (IMGUI_global.instance != null)
            {
                IMGUI_global.instance.clipboard_users.AddElement(OnClipboardOperation);
                IMGUI_global.instance.inputs_users.AddElement(OnImguiInputs);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (IMGUI_global.instance != null)
            {
                IMGUI_global.instance.clipboard_users.RemoveElement(OnClipboardOperation);
                IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
            }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            stdout_field.rT.anchoredPosition = new Vector2(0, -offset_top_h);
            stdin_field.onValidateInput += OnValidateStdin_char;
            stdin_field.onValueChanged.AddListener(OnStdinChanged);
            stdin_field.onSelect.AddListener(OnSelectStdin);
            stdin_field.onDeselect.AddListener(OnDeselectStdin);

            shell = new BoaShell("shell_view");
            shell.ToggleTick(true);

            shell.stdout += AddLine;
            shell.stderr += OnShellStderr;
            shell.status.AddListener(OnShellStatus);

            ResetStdin();
            RefreshStdout();
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnTabContextList(in ContextList list)
        {
            base.OnTabContextList(list);

            list.AddLine();

            var button_interpreters = list.AddButton_trad(new()
            {
                french = "Interpréteur",
                english = "Interpreter",
            });

            button_interpreters.SetupSublist(sublist =>
            {
                foreach (var interpreter in CodeInterpreter.instances)
                    sublist.AddButton_string(interpreter.name);
            });
        }

        bool SguiDragManager.IAcceptDraggable.TryAcceptDraggable(in PointerEventData eventData, in SguiDragManager.IDraggable draggable, in bool onDrop)
        {
            if (shell.status._value.code == CMD_STATUS.WAIT_FOR_STDIN)
                switch (draggable.DragData)
                {
                    case string str:
                        if (onDrop)
                        {
                            TakeFocus();
                            stdin_field.Select();

                            string insert = str.ForceCharacterWrap();
                            stdin_field.text = stdin_field.text[..stdin_field.caretPosition] + insert + stdin_field.text[stdin_field.caretPosition..];
                            stdin_field.caretPosition += insert.Length;
                        }
                        return true;
                }

            return false;
        }

        protected override void OnToggleFocus(bool hasFocus)
        {
            base.OnToggleFocus(hasFocus);
            UsageManager.ToggleUser(this, hasFocus, UsageGroups.Typing, UsageGroups.TrueMouse);
            if (hasFocus)
                NUCLEOR.instance.routinizer.AddRoutine(Util.EWaitForFrames(2, "select stdinfield on focus", this, () =>
                {
                    if (this != null && isFocused._value) stdin_field.Select();
                }));
        }

        public override void OnResized()
        {
            base.OnResized();
            ResizeStdin();
        }

        //----------------------------------------------------------------------------------------------------------

        void OnShellStderr(object data, string lint)
        {
            string text = data is string value ? value : data?.ToString() ?? "null";
            lint ??= text.SetColor(Color.yellow);
            AddLine(text, lint);
        }

        void OnShellStatus(ExecutionStatus status)
        {
            string title = status.code == CMD_STATUS.WAIT_FOR_STDIN
                ? shell.GetType().Name
                : $"{shell.GetType().Name}:{status.code}";

            SguiLoggerOverlay.Log($"change title to shell status: \"{title}\"", this);

            switch (status.code)
            {
                case CMD_STATUS.WAIT_FOR_STDIN:
                case CMD_STATUS.BLOCKED:
                    ResetStdin();
                    break;
            }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once -= RefreshStdout_direct;

            stdin_field.onValidateInput -= OnValidateStdin_char;
            stdin_field.onValueChanged.RemoveListener(OnStdinChanged);
            stdin_field.onSelect.RemoveListener(OnSelectStdin);
            stdin_field.onDeselect.RemoveListener(OnDeselectStdin);

            if (shell != null)
            {
                shell.stdout -= AddLine;
                shell.stderr -= OnShellStderr;
                shell.status.RemoveListener(OnShellStatus);
                shell.Dispose();
                shell = null;
            }

            if (IMGUI_global.instance != null)
            {
                IMGUI_global.instance.clipboard_users.RemoveElement(OnClipboardOperation);
                IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
            }

            instances.Remove(this);
        }
    }
}
