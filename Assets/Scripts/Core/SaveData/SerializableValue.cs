using System;
using UnityEngine;

[Serializable]
public class SerializableValue
{
    [SerializeField] private string Key;
    [SerializeField] private string value;
    [SerializeField] private string valueType;

    public string key => Key;

    public SerializableValue(string key, object value)
    {
        this.Key = key;
        this.value = ObjectParser.ParseObjectAsString(value);
        this.valueType = value.GetType().ToString();
    }

    public void SetValue(object value)
    {
        this.value = ObjectParser.ParseObjectAsString(value);
    }

    public object GetValue()
    {
        return ObjectParser.ParseStringAsObject(value, valueType);
    }
}