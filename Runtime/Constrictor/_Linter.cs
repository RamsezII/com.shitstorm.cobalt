using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Constrictor
    {


        //--------------------------------------------------------------------------------------------------------------

        protected override void OnLint()
        {
            base.OnLint();

            string text = main_input_field.text;
            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = text.Split('\n');
            int character_count = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                character_count += line.Length;

                lines[i] = terminal.linter.GetLint(terminal.shell, line, out Command.Signal signal, SIG_FLAGS.LIST);
                Debug.Log(signal.text);
            }

            lint_tmp.text = lines.Join("\n");
        }
    }
}