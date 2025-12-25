using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    partial class SguiNotepad
    {
        enum Files_Dropdowns : byte
        {
            NewFile,
            OpenFile,
            OpenFolder,
            Save,
            SaveAs,
            _last_,
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnPopulateDropdowns()
        {
            base.OnPopulateDropdowns();

            List<TMP_Dropdown.OptionData> options = new();
            OnPopulateDropdown_Files(options);
            dropdown_files.dropdown.AddOptions(options);
        }

        protected virtual void OnPopulateDropdown_Files(List<TMP_Dropdown.OptionData> options)
        {
            for (Files_Dropdowns code = 0; code < Files_Dropdowns._last_; ++code)
                options.Add(new(code switch
                {
                    Files_Dropdowns.NewFile => "New File",
                    Files_Dropdowns.OpenFile => "Open File",
                    Files_Dropdowns.OpenFolder => "Open Folder",
                    Files_Dropdowns.Save => "Save",
                    Files_Dropdowns.SaveAs => "Save As",
                    _ => code.ToString(),
                }));
        }

        public virtual void OnClick_FilesDropdown(string item)
        {
            if (Enum.TryParse(item, true, out Files_Dropdowns code))
                switch (code)
                {
                    default:
                        Debug.LogWarning($"{GetType().FullName} : unused {nameof(code)}: '{code}'");
                        break;
                }
            else
                Debug.LogWarning($"{GetType().FullName} : unknown {nameof(code)}: '{item}'");
        }
    }
}