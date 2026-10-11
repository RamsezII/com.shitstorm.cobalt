using _ARK_;
using _CMD_;
using _SGUI_;
using _SGUI_.composer;
using System.Collections.Generic;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _SGUI_.context_click;

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

        public LintTheme lint_theme;
        [System.NonSerialized] public Shell shell;
        ShellWriter output_writer, error_writer;
        string prefixe_text = string.Empty, prefixe_lint = string.Empty;

        //----------------------------------------------------------------------------------------------------------

        protected override void OnInitialize()
        {
            base.OnInitialize();

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

            if (lint_theme == null)
                lint_theme = LintTheme.theme_dark;
            output_writer = new ShellWriter(text => AddLine(text));
            error_writer = new ShellWriter(text => AddLine(text, text.SetColor(lint_theme.error)));
            shell = new Shell("Commandes du jeu", output_writer, error_writer) { WorkingDirectory = NUCLEOR.DFHome.FullName };

            stdout_field.rT.anchoredPosition = new Vector2(0, -offset_top_h);
            stdin_field.onValidateInput += OnValidateStdin_char;
            stdin_field.onValueChanged.AddListener(OnStdinChanged);
            stdin_field.onSelect.AddListener(OnSelectStdin);
            stdin_field.onDeselect.AddListener(OnDeselectStdin);

            ResetStdin();
            RefreshStdout();
        }

        //----------------------------------------------------------------------------------------------------------

        bool SguiDragManager.IAcceptDraggable.TryAcceptDraggable(in PointerEventData eventData, in SguiDragManager.IDraggable draggable, in bool onDrop)
        {
            if (shell != null && !shell.IsExecuting)
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

        public override void OnTabContextList(in PointerEventData eventData, in ContextList list)
        {
            base.OnTabContextList(eventData, list);

            list.AddLine_IfAny();
            InitThemesButton(list);
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once -= RefreshStdout_direct;

            stdin_field.onValidateInput -= OnValidateStdin_char;
            stdin_field.onValueChanged.RemoveListener(OnStdinChanged);
            stdin_field.onSelect.RemoveListener(OnSelectStdin);
            stdin_field.onDeselect.RemoveListener(OnDeselectStdin);

            output_writer?.Dispose();
            error_writer?.Dispose();
            shell = null;

            if (IMGUI_global.instance != null)
            {
                IMGUI_global.instance.clipboard_users.RemoveElement(OnClipboardOperation);
                IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
            }

            instances.Remove(this);
        }
    }
}
