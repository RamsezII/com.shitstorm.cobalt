using _SGUI_;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    public partial class Constrictor : SguiEditor
    {
        public Terminal terminal;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            SguiGlobal.instance.button_codium.software_type = typeof(Constrictor);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            sgui_softwarebutton = SguiGlobal.instance.button_codium;
            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            if (terminal == null)
                terminal = InstantiateWindow<Terminal>(true, true, true);
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
            instance = Util.InstantiateOrCreate<Constrictor>(SguiGlobal.instance.rT_2D);
            instance.Init_folder(folder_path);
            return null;
        }
    }
}