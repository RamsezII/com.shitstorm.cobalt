using _ARK_;

namespace _SGUI_
{
    public partial class SguiCodium : SguiNotepad
    {

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();
            UsageManager.ToggleUser(this, true, UsageGroups.Typing, UsageGroups.IMGUI, UsageGroups.BlockPlayer, UsageGroups.Keyboard);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UsageManager.RemoveUser(this);
        }
    }
}