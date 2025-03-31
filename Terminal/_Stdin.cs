namespace _COBALT_
{
    partial class Terminal
    {
        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            return addedChar;
        }

        public void RefreshStdin()
        {
            refresh_stdin.Update(false);

            input_realtime.AutoSize();
            input_prefixe.AutoSize();
            input_stdin.AutoSize();

            scroll_view.content.sizeDelta = new(0, input_stdout.text_height + input_realtime.text_height + input_stdin.text_height);
        }
    }
}