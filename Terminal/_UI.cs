namespace _COBALT_
{
    partial class Terminal
    {
        internal InputText input_out, input_realtime, input_in;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeUI()
        {
            input_out = transform.Find("rT/body/scroll_view/mask/layout/output").GetComponent<InputText>();
            input_realtime = transform.Find("rT/body/scroll_view/Viewport/Content/realtime").GetComponent<InputText>();
            input_in = transform.Find("rT/body/scroll_view/Viewport/Content/in").GetComponent<InputText>();
        }

        //--------------------------------------------------------------------------------------------------------------


    }
}