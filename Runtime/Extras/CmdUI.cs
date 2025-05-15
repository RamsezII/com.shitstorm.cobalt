using _ARK_;
using _COBRA_;
using _SGUI_;
using _UTIL_;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal static partial class CmdUI
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Init_Tests();
            Init_ShowDialog();
            Init_OpenCodium();
            Init_ShowOpen();
            Init_SguiCustom();

            Command.static_domain.AddDomain(new("event-system")).AddAction(
                "show-selected",
                action: static exe =>
                {
                    if (EventSystem.current == null)
                        exe.error = "no event system";
                    else if (EventSystem.current.currentSelectedGameObject == null)
                        exe.error = "no selected object";
                    else
                        exe.Stdout(EventSystem.current.currentSelectedGameObject.transform.GetPath(true));
                },
                args: null);

            Command.static_domain.AddAction(
                "edit-conf-file",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.line.TryReadArgument(out string file_path, out bool seems_valid, strict: true, path_mode: FS_TYPES.FILE))
                        exe.args.Add(file_path);
                },
                action: static exe =>
                {
                    string file_path = (string)exe.args[0];
                    file_path = exe.shell.PathCheck(file_path, PathModes.ForceFull);

                    string file_name = Path.GetFileName(file_path);
                    string type_name = file_name[..^ArkJSon.arkjson.Length];
                    string assembly_name = file_name[..type_name.IndexOf('.')];
                    string type_full_name = $"{type_name}, shitstorm.{assembly_name}";

                    Type type = Type.GetType(type_full_name);
                    SguiWindow.InstantiateWindow<SguiCustom>().EditArkJSon(file_path, type);
                });

            Command.static_domain.AddAction(
                "edit-asset-infos",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.line.TryReadArgument(out string file_path, out bool seems_valid, strict: true, path_mode: FS_TYPES.FILE))
                        exe.args.Add(file_path);
                },
                action: static exe =>
                {
                    string file_path = (string)exe.args[0];
                    file_path = exe.shell.PathCheck(file_path, PathModes.ForceFull);

                    string file_name = Path.GetFileName(file_path);
                    string type_name = file_name[..^ArkJSon.arkjson.Length];
                    string assembly_name = file_name[..type_name.IndexOf('.')];
                    string type_full_name = $"{type_name}, shitstorm.{assembly_name}";

                    Type type = Type.GetType(type_full_name);
                    SguiWindow.InstantiateWindow<SguiCustom>().EditArkJSon(file_path, type);
                });
        }
    }
}