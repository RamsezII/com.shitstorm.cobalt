using _SGUI_.context_click;

namespace _COBALT_
{
    partial class ScriptView
    {

        //--------------------------------------------------------------------------------------------------------------

        void InitButton_Edition()
        {
            var button = header.AddButton(new() { french = "Édition", english = "Edition", });
            button.onList += (ContextList list) =>
            {
                var button_indent = list.AddButton_trad(new() { french = "Indenter", english = "Indent", });
                button_indent.label_right.SetText("Ctrl+K, Ctrl+D");
                button_indent.action += FormatDocument;
            };
        }
    }
}