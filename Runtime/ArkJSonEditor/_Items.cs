using _SGUI_;
using System.Reflection;
using System;
using TMPro;
using System.Linq;

namespace _COBALT_
{
    partial class ArkJSonEditor
    {
        SguiCustom_Abstract AddBool(FieldInfo field, object target, in bool value, ref Action on_save)
        {
            var toggle = AddButton<SguiCustom_Toggle>();
            toggle.toggle.isOn = value;
            on_save += () => field.SetValue(target, toggle.toggle.isOn);
            return toggle;
        }

        SguiCustom_Abstract AddInt(FieldInfo field, object target, in int value, ref Action on_save)
        {
            var input_int = AddButton<SguiCustom_InputField>();
            input_int.input_field.text = value.ToString();
            input_int.input_field.contentType = TMP_InputField.ContentType.IntegerNumber;
            on_save += () => field.SetValue(target, int.Parse(input_int.input_field.text));
            return input_int;
        }

        SguiCustom_Abstract AddFloat(FieldInfo field, object target, in float value, ref Action on_save)
        {
            var input_float = AddButton<SguiCustom_InputField>();
            input_float.input_field.text = value.ToString();
            input_float.input_field.contentType = TMP_InputField.ContentType.DecimalNumber;
            on_save += () => field.SetValue(target, float.Parse(input_float.input_field.text));
            return input_float;
        }

        SguiCustom_Abstract AddEnum(FieldInfo field, object target, in Enum value, ref Action on_save)
        {
            Type type = field.FieldType;

            var dropdown = AddButton<SguiCustom_Dropdown>();
            dropdown.dropdown.ClearOptions();
            dropdown.dropdown.AddOptions(Enum.GetNames(type).ToList());

            int enum_i = Convert.ToInt32(value);
            dropdown.dropdown.value = Enum.GetValues(type).Cast<int>().ToList().IndexOf(enum_i);
            dropdown.dropdown.RefreshShownValue();

            on_save += () =>
            {
                int index = dropdown.dropdown.value;
                string enum_name = dropdown.dropdown.options[index].text;
                field.SetValue(target, Enum.Parse(type, enum_name));
            };

            return dropdown;
        }

        SguiCustom_Abstract AddString(FieldInfo field, object target, in string value, ref Action on_save)
        {
            var input_str = AddButton<SguiCustom_InputField>();
            input_str.input_field.text = value;
            input_str.input_field.contentType = TMP_InputField.ContentType.Standard;
            on_save += () => field.SetValue(target, input_str.input_field.text);
            return input_str;
        }
    }
}