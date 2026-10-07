using _ARK_;
using _COBALT_.scriptview;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        [SerializeField] TopRightButton prefab_topRightButton;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeTopRightButtons()
        {
            prefab_topRightButton.gameObject.SetActive(false);
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