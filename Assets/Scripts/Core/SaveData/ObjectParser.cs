using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParser
{
    public static string ParseObjectAsString(object value)
    {
        string valuetype = value.GetType().ToString();
        if (value is string)
        {
            valuetype = "System.String";
            return (string)value;
        }
        else if (value is bool)
        {
            valuetype = "System.Boolean";
            return (bool)value ? "true" : "false";
        }
        else if (value is Vector2)
        {
            valuetype = "UnityEngine.Vector2";
            Vector2 temp = (Vector2)value;
            return $"{temp.x},{temp.y}";
        }
        else if (value is int)
        {
            valuetype = "System.Int32";
            return value.ToString();
        }
        else if (value is float)
        {
            valuetype = "System.Single";
            return value.ToString();
        }
        else if (value is List<float>)
        {
            valuetype = "System.Collections.Generic.List`1[System.Single]";
            string val = "";
            int count = ((List<float>)value).Count;
            for (int i = 0; i < count; i++)
            {
                val += ((List<float>)value)[i];
                if (i < count - 1) val += ",";
            }
            return val;
        }
        return value.ToString();
    }

    public static object ParseStringAsObject(string value, string valuetype)
    {
        if (valuetype == "System.String")
        {
            return value;
        }
        else if (valuetype == "System.Boolean")
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
        else if (valuetype == "UnityEngine.Vector2")
        {
            string[] parts = value.Split(',');
            if (parts.Length == 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
            {
                return new Vector2(x, y);
            }
        }
        else if (valuetype == "System.Int32")
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
        }
        else if (valuetype == "System.Single")
        {
            if (float.TryParse(value, out float floatValue))
            {
                return floatValue;
            }
        }
        else if (valuetype is "System.Collections.Generic.List`1[System.Single]")
        {
            List<float> list = new List<float>();
            string[] strings = value.Split(",");
            foreach (string s in strings)
            {
                float.TryParse(s, out float number);
                list.Add(number);
            }
            return list;
        }
        return null; // Invalid value or type
    }
}