using System.Collections.Generic;
using TMPro;

namespace _SGUI_
{
    partial class SguiCodium
    {
        enum HelpDropdowns : byte
        {
            Settings,
            _last_,
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnPopulateDropdown_Help(List<TMP_Dropdown.OptionData> options)
        {
            base.OnPopulateDropdown_Help(options);

            for (HelpDropdowns code = 0; code < HelpDropdowns._last_; ++code)
                options.Add(new(code switch
                {
                    _ => code.ToString(),
                }));
        }
    }
}