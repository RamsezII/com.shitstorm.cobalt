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

            AddTopRightButton(new()
            {
                french = "Thème",
                english = "Theme",
            }).onList += (ContextList list) =>
            {
                SguiLoggerOverlay.Log($"test", this, timer: 5);
                var button_light = list.AddButton_trad(new()
                {
                    french = "Clair",
                    english = "Light",
                });

                var button_dark = list.AddButton_trad(new()
                {
                    french = "Sombre",
                    english = "Dark",
                });
            };

            AddTopRightButton(new()
            {
                french = "Interpréteur",
                english = "Interpreter",
            }).onList += (ContextList list) =>
            {
                foreach (var (ext, interpreter) in CodeInterpreter.instances)
                    list.AddButton_string(ext);
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