using _SGUI_;
using System;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    partial class ArkJSonEditor
    {
        public bool TryParseNumberField(in string field_name, in object field_value, out SguiCustom_Slider slider, out SguiCustom_InputField inputfield)
        {
            bool is_int = field_value is int;
            bool is_float = field_value is float;
            bool is_number = is_int || is_float;

            slider = null;
            inputfield = null;

            if (field_name.StartsWith('_'))
                try
                {
                    string[] splits = field_name.Split('_', StringSplitOptions.None);
                    bool is_01 = splits[1].Equals("01", StringComparison.OrdinalIgnoreCase);

                    float min = is_01 ? 0 : splits[1].ToFloat();
                    float max = is_01 ? 1 : splits[2].ToFloat();

                    slider = AddButton<SguiCustom_Slider>();
                    slider.trad_label.SetTrad(splits[(is_01 ? 2 : 3)..].Join("_") + ":");
                    slider.slider.value = is_int ? (int)field_value : (float)field_value;
                    slider.slider.wholeNumbers = is_int;

                    slider.slider.minValue = min;
                    slider.slider.maxValue = max;

                    return true;
                }
                catch (Exception e)
                {
                    if (slider != null)
                        Destroy(slider.gameObject);
                    Debug.LogWarning(e.TrimmedMessage());
                }

            inputfield = AddButton<SguiCustom_InputField>();
            inputfield.trad_label.SetTrad(field_name + ":");
            inputfield.input_field.text = field_value.ToString();

            if (is_int)
                inputfield.input_field.contentType = TMP_InputField.ContentType.IntegerNumber;
            else if (is_float)
                inputfield.input_field.contentType = TMP_InputField.ContentType.DecimalNumber;

            return false;
        }
    }
}