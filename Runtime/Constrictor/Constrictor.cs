using _SGUI_;
using System;
using System.IO;

namespace _COBALT_
{
    public partial class Constrictor : SguiEditor
    {
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
            instance = Util.InstantiateOrCreate<Constrictor>(SGUI_global.instance.rT);
            instance.Init(folder_path);
            return null;
        }
    }
}