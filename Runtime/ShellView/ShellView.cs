using _ARK_;
using _COBRA_;
using _SGUI_;
using _UTIL_;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_
{
    public sealed partial class ShellView : MonoBehaviour, SguiDragManager.IAcceptDraggable
    {
        public static readonly HashSet<ShellView> instances = new();

        public SguiWindow window;
        public SguiTerminal terminal;
        public ShellField stdout_field, stdin_field;
        public TextMeshProUGUI tmp_progress;
        public ScrollRect scrollview;
        public RectTransform content_rT;
        public Scrollbar scrollbar;

        [SerializeField] float stdin_h, stdout_h;
        [SerializeField] bool flag_history;

        float
            offset_top_h = 2,
            offset_bottom_h = 5;

        public LintTheme lint_theme = LintTheme.theme_dark;
        public Shell shell;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            instances.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            InitShellHistory();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            window = GetComponentInParent<SguiWindow>(true);
            terminal = GetComponentInParent<SguiTerminal>(true);

            stdout_field = transform.Find("scrollview/viewport/content/std_out").GetComponent<ShellField>();
            tmp_progress = stdout_field.transform.Find("std_progress").GetComponent<TextMeshProUGUI>();
            stdin_field = tmp_progress.transform.Find("std_in").GetComponent<ShellField>();

            scrollview = transform.Find("scrollview").GetComponent<ScrollRect>();

            content_rT = (RectTransform)transform.Find("scrollview/viewport/content");

            scrollbar = transform.Find("scrollview/scrollbar").GetComponent<Scrollbar>();

            shell?.Dispose();
            shell = null;

            stdin_field.onSelect.AddListener(OnSelectStdin);

            instances.Add(this);
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            IMGUI_global.instance.clipboard_users.AddElement(OnClipboardOperation);
            IMGUI_global.instance.inputs_users.AddElement(OnImguiInputs);
        }

        private void OnDisable()
        {
            IMGUI_global.instance.clipboard_users.RemoveElement(OnClipboardOperation);
            IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            stdout_field.rT.anchoredPosition = new Vector2(0, -offset_top_h);
            stdin_field.onValidateInput += OnValidateStdin_char;
            stdin_field.onValueChanged.AddListener(OnStdinChanged);
            stdin_field.onSelect.AddListener(OnSelectStdin);
            stdin_field.onDeselect.AddListener(OnDeselectStdin);

            shell = new BoaShell("shell_view");
            shell.ToggleTick(true);

            shell.stdout += AddLine;
            shell.stderr += (data, lint) =>
            {
                string text = data is string s ? s : data.ToString();
                lint ??= text.SetColor(Color.yellow);
                AddLine(text, lint);
            };

            shell.status.AddListener(status =>
            {
                string title = status.code == CMD_STATUS.WAIT_FOR_STDIN
                    ? shell.GetType().Name
                    : $"{shell.GetType().Name}:{status.code}";

                if (terminal != null)
                    terminal.trad_title.SetTrad(title);

                switch (status.code)
                {
                    case CMD_STATUS.WAIT_FOR_STDIN:
                        ResetStdin();
                        break;

                    case CMD_STATUS.BLOCKED:
                        ResetStdin();
                        break;

                    case CMD_STATUS.NETWORKING:
                        break;
                }
            });

            ResetStdin();
            RefreshStdout();
        }

        //----------------------------------------------------------------------------------------------------------

        bool SguiDragManager.IAcceptDraggable.TryAcceptDraggable(in SguiDragManager.IDraggable draggable, in bool onDrop)
        {
            if (!onDrop)
                return true;

            if (shell.status._value.code == CMD_STATUS.WAIT_FOR_STDIN)
            {
                string stdin = stdin_field.text;
                string insert = $"\"{draggable.DragData}\"".ForceCharacterWrap();
                stdin = stdin[..stdin_field.caretPosition] + insert + stdin[stdin_field.caretPosition..];
                stdin_field.text = stdin;
                stdin_field.caretPosition += insert.Length;
            }

            window.TakeFocus();
            stdin_field.Select();

            return true;
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            shell?.Dispose();
            shell = null;
            IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
            instances.Remove(this);
        }
    }
}