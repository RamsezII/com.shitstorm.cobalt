using _SGUI_;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    internal partial class InputText : MonoBehaviour, IMGUI_global.IUser
    {
        public enum Types { Stdout, Realtime, Prefixe, Stdin, }

        [HideInInspector] public Terminal terminal;
        [HideInInspector] public RectTransform rT, rT_parent;
        [HideInInspector] public TMP_InputField input_field;

        public Types type;
        public float text_height;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            terminal = GetComponentInParent<Terminal>();
            rT = (RectTransform)transform;
            rT_parent = (RectTransform)transform.parent;
            input_field = GetComponent<TMP_InputField>();
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Start()
        {
            if (type == Types.Stdin)
            {
                input_field.onValidateInput = OnValidateStdin;

                input_field.onSelect.AddListener(text =>
                {
                    IMGUI_global.instance.users.RemoveElement(this);
                    IMGUI_global.instance.users.AddElement(this);
                });

                input_field.onDeselect.AddListener(text => IMGUI_global.instance.users.RemoveElement(this));
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        public void AutoSize(in bool apply)
        {
            if (string.IsNullOrWhiteSpace(input_field.text))
                text_height = 0;
            else
                text_height = input_field.textComponent.GetPreferredValues(input_field.text, rT_parent.rect.width, float.PositiveInfinity).y;

            if (apply)
                rT.sizeDelta = new(rT.sizeDelta.x, text_height);
        }

        void IMGUI_global.IUser.OnOnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.alt)
            {
                switch (e.keyCode)
                {
                    case KeyCode.LeftArrow:
                    case KeyCode.RightArrow:
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        e.Use(); // Empêche TMP d’interpréter
                        break;
                }
            }
        }
    }
}