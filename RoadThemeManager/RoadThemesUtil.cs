using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RoadsUnited_Core
{
    public static class RoadThemesUtil
    {
        public static Q ReadPrivate<T, Q>(T o, string fieldName)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo fieldInfo = null;
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo2 = array[i];
                if (fieldInfo2.Name == fieldName)
                {
                    fieldInfo = fieldInfo2;
                    break;
                }
            }
            return (Q)((object)fieldInfo.GetValue(o));
        }
        public static void WritePrivate<T, Q>(T o, string fieldName, object value)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo fieldInfo = null;
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo2 = array[i];
                if (fieldInfo2.Name == fieldName)
                {
                    fieldInfo = fieldInfo2;
                    break;
                }
            }
            fieldInfo.SetValue(o, value);
        }

        public static string GetDescription(RoadThemePack pack)
        {
            if (pack != null)
            {
                return pack.themeDescription;
            }
            return "No Theme Description";
        }
    }
}
