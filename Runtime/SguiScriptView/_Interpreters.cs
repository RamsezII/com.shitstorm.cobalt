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
            var button_interpreter = AddTopRightButton(default);

            button_interpreter.onList += (ContextList list) =>
            {
                foreach (var interpreter in CodeInterpreter.instances)
                {
                    var button = list.AddButton_string(interpreter.name);
                    button._button.onClick.AddListener(() =>
                    {
                        current_interpreter.Value = interpreter;
                        RefreshInputField();
                    });
                }
            };

            var button_execution = AddTopRightButton(new() { french = "Lancer", english = "Run", });

            button_execution.button.onClick.AddListener(() => current_interpreter._value.execution(input_field.text));

            current_interpreter.Value = CodeInterpreter.instances.First();

            current_interpreter.AddListener(value =>
            {
                button_execution.gameObject.SetActive(current_interpreter.Has && current_interpreter._value.execution != null);

                if (current_interpreter.HasNot)
                    button_interpreter.trad_label.SetTraductions(new()
                    {
                        french = "Interpréteur",
                        english = "Interpreter",
                    });
                else
                    button_interpreter.trad_label.SetTraductions(new()
                    {
                        french = $"Interpréteur : {value.name.Bold()}",
                        english = $"Interpreter: {value.name.Bold()}",
                    });
            });
        }
    }
}