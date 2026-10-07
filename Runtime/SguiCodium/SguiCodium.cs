using _SGUI_;
using _SGUI_.composer;
using _SGUI_.tab_control;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    public partial class SguiCodium : SguiFrame
    {
        public ScriptView scriptview;
        SguiTabController tabController;
        public readonly Dictionary<SguiTabButton, FileInfo> tabs__files = new();
        [SerializeField] SguiTabButton empty_tab;

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnInitialize()
        {
            tabController = GetComponentInChildren<SguiTabController>(true);
            scriptview = GetComponentInChildren<ScriptView>(true);

            base.OnInitialize();
            foreach (var shellView in GetComponentsInChildren<ShellView>(true)) shellView.Initialize();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

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