using _ARK_;
using _COBALT_.scriptview;
using _SGUI_.context_click;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        [SerializeField] TopRightButton prefab_topRightButton;

        //--------------------------------------------------------------------------------------------------------------

        void InitTopRightButtons()
        {
            prefab_topRightButton.gameObject.SetActive(false);
        }

        void InitButton_Edition()
        {
            var button = AddTopRightButton(new() { french = "Édition", english = "Edition", });
            button.onList += (ContextList list) =>
            {
                var button_indent = list.AddButton_trad(new() { french = "Indenter", english = "Indent", });
                button_indent.label_right.SetText("Ctrl+K, Ctrl+D");
                button_indent._button.onClick.AddListener(FormatDocument);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        TopRightButton AddTopRightButton(in Traductions label)
        {
            TopRightButton clone = Instantiate(prefab_topRightButton, parent: prefab_topRightButton.transform.parent);
            clone.gameObject.SetActive(true);
            clone.trad_label.SetTraductions(label);
            return clone;
        }
    }
}