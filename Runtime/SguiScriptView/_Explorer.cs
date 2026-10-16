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
            SguiExplorerView.onContextList_file += (SguiExplorerView view, ContextList list, FileInfo file) =>
            {
                var button = list.AddButton_trad(new()
                {
                    french = $"Éditer ce script",
                    english = $"Edit script",
                });
                button._button.onClick.AddListener(() =>
                {
                    SguiLoggerOverlay.Log($"open script");
                });
            };
        }
    }
}