using System;
using UnityEngine;

[Serializable]
public class DialogueEntry : CutsceneEvent
{
    public string name;
    public bool leftSide;
    public string text;
    public Sprite leftPortrait, rightPortrait;

    public DialogueEntry(string name, bool leftSide, string text, Sprite leftportrait, Sprite rightPortrait)
    {
        this.name = name;
        this.leftSide = leftSide;
        this.text = text;
        this.leftPortrait = leftportrait;
        this.rightPortrait = rightPortrait;
    }

    public DialogueEntry() { }
}