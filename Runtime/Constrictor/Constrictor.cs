using _SGUI_;
using System.IO;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    public partial class Constrictor : SguiEditor
    {
        [SerializeField] protected TextMeshProUGUI lint_tmp;

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            lint_tmp = main_input_field.transform.Find("text_area/text/lint").GetComponent<TextMeshProUGUI>();
        }

        //--------------------------------------------------------------------------------------------------------------

        public static string TryOpenConstrictor(in string folder_path, in bool create_if_none, out Constrictor instance)
        {
            if (!Directory.Exists(folder_path))
                if (create_if_none)
                    Directory.CreateDirectory(folder_path);
                else
                {
                    instance = null;
                    return $"can not find directory '{folder_path}'\n";
                }
            instance = Util.InstantiateOrCreate<Constrictor>(SguiGlobal.instance.rT);
            instance.Init_folder(folder_path);
            return null;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnValueChange(string text)
        {
            base.OnValueChange(text);
            lint_tmp.text = text;
        }
    }
}