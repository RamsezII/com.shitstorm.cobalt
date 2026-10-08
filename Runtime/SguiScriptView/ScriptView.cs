using _ARK_;
using _SGUI_.composer;
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

        private void OnValidate()
        {
            if (didStart)
                RefreshInputField();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
#if UNITY_EDITOR
            ArkUI._VisibleInEditor.Add(gameObject);
#endif

            input_field.text = string.Empty;
            text_lint.text = string.Empty;

            base.Awake();

            AwakeTopRightButtons();
            InitInterpreters();
            InitThemes();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            StartFileLoading();
            StartInputField();
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ArkUI._VisibleInEditor.Remove(gameObject);
        }
#endif
    }
}
