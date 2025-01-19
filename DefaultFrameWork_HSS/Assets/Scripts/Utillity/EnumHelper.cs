using System;
using System.Reflection;

namespace HSS
{
    public static class EnumHelper
    {
        public static string enumName(this Enum en) => GetEnum(en);
        public static string GetEnum(Enum en)
        {
            Type type = en.GetType();
            FieldInfo field = type.GetField(en.ToString());
            if(field.GetCustomAttributes(typeof(EnumName),false) is EnumName[] attrs && attrs.Length > 0)
                return attrs[0].Value;

            return en.ToString();
        }
    }

    public class EnumName : Attribute
    {
        private string _value;

        public EnumName(string value) => _value = value;

        public string Value => _value;
    }

    public enum Language
    {
        [EnumName("English")] en = 0,
        [EnumName("한국어")] ko = 1,
        [EnumName("日本語")] ja = 2,
        [EnumName("Español")] es = 3,    // - 스페인어 (es)
        [EnumName("Português")] pt = 4,  // - 포르투갈어 (pt)
        [EnumName("Français")] fr = 5,   // - 프랑스어 (fr)
        [EnumName("Deutsch")] de = 6,    // - 독일어 (de)
        [EnumName("Русский")] ru = 7,    // - 러시아어 (ru)
    }
}
