using _ARK_;
using _SGUI_;
using System.IO;
using System.Reflection;
using System;

namespace _COBALT_
{
    internal partial class ArkJSonEditor : SguiCustom
    {
        public void EditJSon(in object json)
        {
            ArkJSon arkjson = json as ArkJSon;
            string title = Path.GetFileName(arkjson.GetFilePath());
            trad_title.SetTrad(title);

            FieldInfo[] target_fields = arkjson.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            Action on_save = null, on_change = () => trad_title.SetTrad(title + "*");
            onAction_confirm += () =>
            {
                on_save?.Invoke();
                arkjson.SaveArkJSon(true);
                return true;
            };

            for (int i = 0; i < target_fields.Length; ++i)
            {
                FieldInfo field = target_fields[i];

                if (field.IsNotSerialized)
                    continue;

                Type type = field.FieldType;
                object value = field.GetValue(arkjson);

                SguiCustom_Abstract button = value switch
                {
                    bool _bool => AddBool(field, arkjson, _bool, on_change, ref on_save),
                    int _int => AddInt(field, arkjson, _int, on_change, ref on_save),
                    float _float => AddFloat(field, arkjson, _float, on_change, ref on_save),
                    Enum _enum => AddEnum(field, arkjson, _enum, on_change, ref on_save),
                    string _str => AddString(field, arkjson, _str, on_change, ref on_save),
                    _ => null,
                };

                if (button == null)
                {
                    button = AddButton<SguiCustom_Label>();
                    button.trad_label.SetTrads(new()
                    {
                        english = $"Could not parse field: {{ \"{field.Name}\" : \"{value}\" }} ({type})",
                        french = $"Impossible de parser le champ: {{ \"{field.Name}\" : \"{value}\" }} ({type})",
                    });
                }
            }
        }
    }
}