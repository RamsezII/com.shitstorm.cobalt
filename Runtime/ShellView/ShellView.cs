using _ARK_;
using _COBRA_;
using _SGUI_;
using System.Collections.Generic;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _COBALT_
{
    public sealed partial class ShellView : ArkComponent2, SguiDragManager.IAcceptDraggable
    {
        [AutoStaticsCleanup] public static readonly HashSet<ShellView> instances = new();

        public _SGUI_.composer.SguiFrame window;
        public SguiTerminal terminal;
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
        bool initialized;

        //----------------------------------------------------------------------------------------------------------

        internal void Initialize()
        {
            if (initialized) return;
            initialized = true;
            window = GetComponentInParent<_SGUI_.composer.SguiFrame>(true);
            terminal = GetComponentInParent<SguiTerminal>(true);

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
            window.isFocused.AddListener(OnFocus);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (IMGUI_global.instance != null)
            {
                IMGUI_global.instance.clipboard_users.RemoveElement(OnClipboardOperation);
                IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
            }

            window.isFocused.RemoveListener(OnFocus);
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

        bool SguiDragManager.IAcceptDraggable.TryAcceptDraggable(in PointerEventData eventData, in SguiDragManager.IDraggable draggable, in bool onDrop)
        {
            if (shell.status._value.code == CMD_STATUS.WAIT_FOR_STDIN)
                switch (draggable.DragData)
                {
                    case string str:
                        if (onDrop)
                        {
                            window.TakeFocus();
                            stdin_field.Select();

                            string insert = str.ForceCharacterWrap();
                            stdin_field.text = stdin_field.text[..stdin_field.caretPosition] + insert + stdin_field.text[stdin_field.caretPosition..];
                            stdin_field.caretPosition += insert.Length;
                        }
                        return true;
                }

            return false;
        }

        void OnFocus(bool hasFocus)
        {
            UsageManager.ToggleUser(this, hasFocus, UsageGroups.Typing, UsageGroups.TrueMouse);
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
