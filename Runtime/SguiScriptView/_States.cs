using _UTIL_;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        [SerializeField] bool dirty;

        //--------------------------------------------------------------------------------------------------------------

        void RefreshTitle()
        {
            if (file == null)
                tab.trad_title.SetTraductions(new() { french = "Sans Titre*", english = "Untitled*", });
            else if (dirty)
                tab.trad_title.SetText($"{file.Name} *");
            else
                tab.trad_title.SetText(file.Name);
        }
    }
}