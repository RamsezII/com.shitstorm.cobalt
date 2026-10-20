using _SGUI_.context_click;

namespace _COBALT_
{
    partial class ScriptView
    {

        //--------------------------------------------------------------------------------------------------------------

        void InitButton_Edition()
        {
            var button = header.AddButton(
                label: new() { french = "Édition", english = "Edition", },
                arrow: false
            );

            button.onList += (ContextList list) =>
            {
                var button_indent = list.AddButton_trad(new() { french = "Indenter", english = "Indent", });
                button_indent.label_right.SetText("Ctrl+K, Ctrl+D");
                button_indent._button.onClick.AddListener(FormatDocument);
            };
        }
    }
}