using _SGUI_;
using System.Reflection;
using System;
using TMPro;
using System.Linq;

namespace _COBALT_
{
    partial class ArkJSonEditor
    {
        SguiCustom_Toggle AddBool(FieldInfo field, object target, in bool value, Action on_change, ref Action on_save)
        {
            var toggle = AddButton<SguiCustom_Toggle>();
            toggle.trad_label.SetTrad(field.Name + ":");
            toggle.toggle.isOn = value;
            toggle.toggle.onValueChanged.AddListener(_ => on_change());
            on_save += () => field.SetValue(target, toggle.toggle.isOn);
            return toggle;
        }

        SguiCustom_Abstract AddInt(FieldInfo field, object target, in int value, Action on_change, ref Action on_save)
        {
            if (TryParseNumberField(field.Name, value, out var slider, out var inputfield))
            {
                slider.slider.onValueChanged.AddListener(_ => on_change());
                on_save += () => field.SetValue(target, (int)slider.slider.value);
                return slider;
            }
            else
            {
                inputfield.input_field.onValueChanged.AddListener(_ => on_change());
                on_save += () => field.SetValue(target, int.Parse(inputfield.input_field.text));
                return inputfield;
            }
        }

        SguiCustom_Abstract AddFloat(FieldInfo field, object target, in float value, Action on_change, ref Action on_save)
        {
            if (TryParseNumberField(field.Name, value, out var slider, out var inputfield))
            {
                slider.slider.onValueChanged.AddListener(_ => on_change());
                on_save += () => field.SetValue(target, slider.slider.value);
                return slider;
            }
            else
            {
                inputfield.input_field.onValueChanged.AddListener(_ => on_change());
                on_save += () => field.SetValue(target, float.Parse(inputfield.input_field.text));
                return inputfield;
            }
        }

        SguiCustom_Dropdown AddEnum(FieldInfo field, object target, in Enum value, Action on_change, ref Action on_save)
        {
            Type type = field.FieldType;

            var dropdown = AddButton<SguiCustom_Dropdown>();
            dropdown.trad_label.SetTrad(field.Name + ":");
            dropdown.dropdown.ClearOptions();
            dropdown.dropdown.AddOptions(Enum.GetNames(type).ToList());

            int enum_i = Convert.ToInt32(value);
            dropdown.dropdown.value = Enum.GetValues(type).Cast<int>().ToList().IndexOf(enum_i);
            dropdown.dropdown.RefreshShownValue();

            dropdown.dropdown.onValueChanged.AddListener(_ => on_change());

            on_save += () =>
            {
                int index = dropdown.dropdown.value;
                string enum_name = dropdown.dropdown.options[index].text;
                field.SetValue(target, Enum.Parse(type, enum_name));
            };

            return dropdown;
        }

        SguiCustom_InputField AddString(FieldInfo field, object target, in string value, Action on_change, ref Action on_save)
        {
            var input_str = AddButton<SguiCustom_InputField>();
            input_str.trad_label.SetTrad(field.Name + ":");
            input_str.input_field.text = value;
            input_str.input_field.contentType = TMP_InputField.ContentType.Standard;
            input_str.input_field.onValueChanged.AddListener(_ => on_change());
            on_save += () => field.SetValue(target, input_str.input_field.text);
            return input_str;
        }
    }
}