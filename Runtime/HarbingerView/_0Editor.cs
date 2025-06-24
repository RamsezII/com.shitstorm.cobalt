#if UNITY_EDITOR
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        //--------------------------------------------------------------------------------------------------------------

        [ContextMenu(nameof(OnValidate))]
        private void OnValidate()
        {
            if (didStart)
                OnChange();
        }

        //--------------------------------------------------------------------------------------------------------------

        private void _OnGUI()
        {
            Rect rect = new(10, 30, 400, Screen.height - 60);

            // FOND NOIR OPAQUE
            Texture2D blackTex = Texture2D.blackTexture;
            GUI.DrawTexture(rect, blackTex, ScaleMode.StretchToFill, false, 0);

            GUILayout.BeginArea(rect, GUI.skin.box);

            if (last_reader != null)
            {
                GUILayout.Label(last_reader.completion_l);
                GUILayout.Label(last_reader.completion_r);
                GUILayout.Label(string.Empty);

                if (last_reader.completions_v != null)
                    foreach (var item in last_reader.completions_v)
                        GUILayout.Label(item);
            }

            GUILayout.EndArea();
        }
    }
}
#endif