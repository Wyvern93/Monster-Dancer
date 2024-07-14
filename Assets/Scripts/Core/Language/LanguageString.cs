using System;
using UnityEngine;

[Serializable]
public class LanguageString
{
    [SerializeField] public string id;
    [SerializeField] public string text;

    public LanguageString(string id, string text)
    {
        this.id = id;
        this.text = text;
    }
}