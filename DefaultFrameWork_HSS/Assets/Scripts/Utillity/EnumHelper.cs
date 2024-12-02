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
}
