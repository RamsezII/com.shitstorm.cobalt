using _SGUI_;
using System.IO;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    public partial class SguiNotepad : SguiWindow1
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
            if (!File.Exists(file_path))
                if (create_if_none)
                {
                    DirectoryInfo parent = Directory.GetParent(file_path);
                    if (!parent.Exists)
                        Directory.CreateDirectory(parent.FullName);
                    File.WriteAllText(file_path, string.Empty);
                }
                else
                {
                    instance = null;
                    return $"can not find file '{file_path}'\n";
                }
            instance = Util.InstantiateOrCreate<SguiNotepad>(parent: SguiGlobal.instance.rt_windows2);
            instance.Init_file(file_path);
            return null;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            script_view = GetComponentInChildren<ScriptView>();
            footer_tmp = transform.Find("rT/footer/text").GetComponent<TextMeshProUGUI>();

            base.OnAwake();

            trad_title.SetTrad("Shitpad");
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
            footer_tmp.text = file_path;
            this.file_path = file_path;
        }
    }
}