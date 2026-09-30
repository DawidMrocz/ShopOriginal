using System;
using System.ComponentModel;

namespace Jobs.Extensions
{
    public static class ExnumExtension
    {
        public static string GetDesrciptionOrValueString(this Enum @enum)
        {
            var attrDescription = @enum.GetAttribute<DescriptionAttribute>();

            return attrDescription == null ? @enum.ToString() : attrDescription.Description;
        }


        public static T GetAttribute<T>(this Enum @enum) where T : Attribute
        {
            var type = @enum.GetType();
            var memInfo = type.GetMember(@enum.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(T), false);
                if (attrs != null && attrs.Length > 0)
                    return (T)attrs[0];
            }
            return null;
        }
    }
}
