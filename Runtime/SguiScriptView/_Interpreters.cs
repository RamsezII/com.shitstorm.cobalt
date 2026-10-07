using _ARK_;
using _SGUI_.context_click;
using _UTIL_;
using System.Linq;

namespace _COBALT_
{
    partial class ScriptView
    {
        readonly ValueNotifier<CodeInterpreter> current_interpreter = new();

        //--------------------------------------------------------------------------------------------------------------

        void InitInterpreters()
        {
            var button = AddTopRightButton(default);

            current_interpreter.AddListener(value =>
            {
                if (current_interpreter.HasNot)
                    button.trad_label.SetTraductions(new()
                    {
                        french = "Interpréteur",
                        english = "Interpreter",
                    });
                else
                    button.trad_label.SetTraductions(new()
                    {
                        french = $"Interpréteur : {value.name.Bold()}",
                        english = $"Interpreter: {value.name.Bold()}",
                    });
            });

            button.onList += (ContextList list) =>
            {
                foreach (var interpreter in CodeInterpreter.instances)
                {
                    var button = list.AddButton_string(interpreter.name);
                    button._button.onClick.AddListener(() =>
                    {
                        current_interpreter.Value = interpreter;
                        AutoRefreshInputField();
                    });
                }
            };

            current_interpreter.Value = CodeInterpreter.instances.First();
        }
    }
}