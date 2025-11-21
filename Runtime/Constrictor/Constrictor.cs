using _SGUI_;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    public partial class Constrictor : SguiCodium
    {
        public HarbingerView harbinger_view;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            SoftwareButton os_button = OSView.instance.GetSoftwareButton<Constrictor>(force: true);
            os_button.onClick_left_empty += eventData => LoggerOverlay.Log($"Please specify a path when opening {typeof(Constrictor)}", logLevel: LoggerOverlay.LogLevel.Warning);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            harbinger_view = transform.Find("rT/body/_COBALT_.HarbingerView").GetComponent<HarbingerView>();
            base.Awake();
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

            instance = Util.InstantiateOrCreate<Constrictor>(SguiGlobal.instance.rt_windows2);
            instance.Init_folder(folder_path);
            instance.harbinger_view.shell.workdir = folder_path;

            return null;
        }
    }
}