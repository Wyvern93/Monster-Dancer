using System;

[Serializable]
public class DialogueEntry : CutsceneEvent
{
    public string name;
    public bool leftSide;
    public string text;
    public string leftPortrait, rightPortrait;

    public DialogueEntry(string name, bool leftSide, string text, string leftportrait, string rightPortrait)
    {
        this.name = name;
        this.leftSide = leftSide;
        this.text = text;
        this.leftPortrait = leftportrait;
        this.rightPortrait = rightPortrait;
    }

    public DialogueEntry() { }
}