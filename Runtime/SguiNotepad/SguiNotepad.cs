using _SGUI_;
using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    public partial class SguiNotepad : SguiSoftware
    {
        public ScriptView script_view;
        [SerializeField] protected TextMeshProUGUI footer_tmp;
        [SerializeField] protected string file_path;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            OSView.instance.AddSoftwareButton<SguiNotepad>(new()
            {
                french = "Éditeur de texte",
                english = "Text editor",
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        public static string TryOpenNotepad(in string file_path, in bool create_if_none, out SguiNotepad instance)
        {
            instance = null;

            try
            {
                string full_path = Path.GetFullPath(file_path);
                if (!File.Exists(full_path))
                    if (create_if_none)
                    {
                        DirectoryInfo parent = new FileInfo(full_path).Directory;
                        if (parent != null && !parent.Exists)
                            parent.Create();
                        File.WriteAllText(full_path, string.Empty);
                    }
                    else
                        return $"can not find file '{full_path}'\n";

                instance = OSView.InstantiateSoftware<SguiNotepad>();
                instance.Init_file(full_path);
                return null;
            }
            catch (Exception exception)
            {
                if (instance != null)
                {
                    UnityEngine.Object.Destroy(instance.gameObject);
                    instance = null;
                }

                return $"could not open '{file_path}': {exception.Message}\n";
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            script_view = GetComponentInChildren<ScriptView>();
            footer_tmp = transform.Find("rT/footer/text").GetComponent<TextMeshProUGUI>();

            base.OnAwake();

            trad_title.SetText("Shitpad");
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            InitHeader_File();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected void Init_file(in string file_path)
        {
            FileInfo file = new(file_path);
            footer_tmp.text = file.FullName;
            this.file_path = file.FullName;
            script_view.file_path.Value = file;
        }
    }
}
