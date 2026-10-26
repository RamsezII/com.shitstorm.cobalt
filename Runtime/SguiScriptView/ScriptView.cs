using _ARK_;
using _SGUI_;
using _SGUI_.composer;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_
{
    public sealed partial class ScriptView : SguiFrame
    {
        [SerializeField] ScrollRect scrollview;
        [SerializeField] TMP_InputField input_field;
        [SerializeField] TMP_Text text_placeholder, text_lint, text_error;

        [SerializeField, UField]
        bool
             use_intellisense = true,
             space_confirms_completion = false;

        //--------------------------------------------------------------------------------------------------------------

        public override SguiFrame OpenInNewComposer(SguiComposer composer)
        {
            var split_explorer = composer.AddSplitView();
            var split_shell = composer.SplitView(split_explorer, axis: SguiComposer.SplitAxis.Horizontal, ratio: 0.25f);
            var split_editor = composer.SplitView(split_shell, axis: SguiComposer.SplitAxis.Vertical, ratio: 0.3f);

            var explorer = split_explorer.AddTab(Util.LoadResourceByType<SguiExplorerWindow>());
            var shell = split_shell.AddTab(Util.LoadResourceByType<ShellView>());

            var dscripts = GetScriptsDir();
            explorer.view.opened_folders.Add(dscripts.FullName);
            explorer.view.selected_fsi.Value = dscripts;
            explorer.view.RebuildHierarchy();

            return split_editor.AddTab(this);
        }

        private void OnValidate()
        {
            if (didStart)
                RefreshInputField();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnInitialize()
        {
            base.OnInitialize();

#if UNITY_EDITOR
            ArkUI._VisibleInEditor.Add(gameObject);
#endif

            input_field.text = string.Empty;
            text_lint.text = string.Empty;

            InitInputField();
            InitFileButton();
            InitButton_Edition();
            InitThemes();
            InitExecutionButton();

            RefreshTitle();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            NUCLEOR.delegates.OnApplicationFocus += OnNucleorFocus;
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnNucleorFocus()
        {
            ReloadCurrentFile();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnToggleFocus(bool focus)
        {
            base.OnToggleFocus(focus);
            if (focus)
                ReloadCurrentFile();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.OnApplicationFocus -= OnNucleorFocus;

            base.OnDestroy();

#if UNITY_EDITOR
            ArkUI._VisibleInEditor.Remove(gameObject);
#endif
        }
    }
}
