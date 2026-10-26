using _SGUI_;
using _SGUI_.context_click;
using _SGUI_.composer;
using System;
using System.IO;
using System.Linq;
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
                button.action += () => OpenFromExplorer(view, file);
            };
        }

        static ScriptView OpenFromExplorer(SguiExplorerView explorer, FileInfo file)
        {
            if (explorer == null || file == null)
                return null;

            var source = explorer.GetComponentInParent<SguiFrame>(true);
            if (source == null || source.oblivionized || source.composer == null || source.composer.oblivionized)
                return null;

            var composer = source.composer;
            var editors = composer.GetComponentsInChildren<ScriptView>(true).Where(view => view.composer == composer && !view.oblivionized).ToArray();
            var alreadyOpen = editors.FirstOrDefault(view => string.Equals(view.file?.FullName, file.FullName, Util.comp_path));
            if (alreadyOpen != null)
            {
                alreadyOpen.TakeFocus();
                return alreadyOpen;
            }

            var prefab = Util.LoadResourceByType<ScriptView>();
            var empty = editors.FirstOrDefault(view => view.file == null && string.IsNullOrEmpty(view.input_field.text));
            var existing = empty ?? editors.FirstOrDefault();
            var targetSplit = existing != null ? existing.pview : composer.SplitView(source.pview, SguiComposer.SplitAxis.Horizontal, 0.3f);
            var editor = targetSplit.AddTab(prefab);

            try
            {
                if (editor.LoadFile(file))
                {
                    if (empty != null)
                    {
                        targetSplit.MoveTabHere(editor.tab, empty.tab);
                        empty.RequestClose();
                    }

                    editor.TakeFocus();
                    return editor;
                }
            }
            catch (Exception exception)
            {
                SguiLoggerOverlay.Log($"Cannot open \"{file.FullName}\": {exception.Message}", source, logLevel: SguiLogLevel.Error);
            }

            editor.RequestClose();
            return null;
        }
    }
}
