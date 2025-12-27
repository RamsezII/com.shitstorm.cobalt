using _SGUI_;
using _SGUI_.tab_control;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    public partial class SguiCodium : SguiWindow1
    {
        public ScriptView scriptview;
        public ShellView shellView;
        public SguiExplorerView explorerview;
        SguiTabController tabController;
        public readonly Dictionary<SguiTabButton, FileInfo> tabs__files = new();
        [SerializeField] SguiTabButton empty_tab;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            OSView.instance.AddSoftwareButton<SguiCodium>(new()
            {
                french = $"Éditeur de code",
                english = $"Code editor",
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            tabController = GetComponentInChildren<SguiTabController>(true);
            shellView = GetComponentInChildren<ShellView>(true);
            scriptview = GetComponentInChildren<ScriptView>(true);
            explorerview = GetComponentInChildren<SguiExplorerView>(true);

            base.OnAwake();

            trad_title.SetTrad("ShitCodium");

            empty_tab = tabController.AddTab();
            empty_tab.text.text = "Untitled";
        }

        //--------------------------------------------------------------------------------------------------------------

        public void AddFile(in FileInfo file)
        {
            var tab = tabController.AddTab();
            tab.text.text = file.Name;
        }
    }
}