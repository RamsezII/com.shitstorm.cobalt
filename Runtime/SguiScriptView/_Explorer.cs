using _SGUI_;
using _SGUI_.context_click;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitExplorerExtensions()
        {
            SguiExplorerView.onContextClick_directory += (ContextList list, DirectoryInfo dir) =>
            {
                var button = list.AddButton_trad(new()
                {
                    french = $"Ouvrir ce dossier dans",
                    english = $"Open this directory in",
                });

                button.SetupSublist(sublist =>
                {
                    {
                        var button = sublist.AddButton_string("Shitpad");
                    }

                    {
                        var button = sublist.AddButton_string("Shitcodium");
                    }
                });
            };
        }
    }
}