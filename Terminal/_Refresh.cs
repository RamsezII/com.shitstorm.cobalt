using _ARK_;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        public float line_height;

        //--------------------------------------------------------------------------------------------------------------

        public void RefreshStdout()
        {
            StringBuilder sb = new();
            lock (lines)
            {
                foreach (object line in lines)
                    sb.AppendLine(line.ToString());
            }
            stdout = sb.TroncatedForLog();

            input_stdout.input_field.text = stdout;
            input_stdout.AutoSize(true);

            RefreshStdin();
            flag_clampbottom.Update(true);
        }

        void RefreshRealtime()
        {
            input_realtime.AutoSize(true);
            rT_scrollview.sizeDelta = new Vector2(0, -input_realtime.text_height);
        }

        public void RefreshStdin()
        {
            string prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{shell.prefixe.SetColor("#73B2D9")}$";
            input_prefixe.input_field.text = prefixe;

            Vector2 prefered_dims = input_prefixe.input_field.textComponent.GetPreferredValues(prefixe + "_", scrollview.content.rect.width, float.PositiveInfinity);
            line_height = prefered_dims.y;

            float prefixe_width = prefered_dims.x;
            float stdin_height = Mathf.Max(input_stdin.text_height, scrollview.viewport.rect.height);

            input_prefixe.AutoSize(false);
            input_stdin.AutoSize(false);

            input_stdin.rT.sizeDelta = new(-prefixe_width, 0);
            rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, stdin_height);

            scrollview.content.sizeDelta = new(0, 1 + input_stdout.text_height + input_realtime.text_height + stdin_height);
        }

        public void ClearStdout()
        {
            scrollview.verticalNormalizedPosition = 0;
        }

        public void ClampBottom()
        {
            float bottom_view = -scrollview.viewport.rect.height - scrollview.content.anchoredPosition.y;
            float bottom_stdin = -input_stdout.text_height - input_stdin.text_height;

            if (bottom_stdin < bottom_view)
                scrollview.verticalNormalizedPosition = Mathf.InverseLerp(-scrollview.content.rect.height, -scrollview.viewport.rect.height, bottom_stdin - 2 * line_height);
        }
    }
}