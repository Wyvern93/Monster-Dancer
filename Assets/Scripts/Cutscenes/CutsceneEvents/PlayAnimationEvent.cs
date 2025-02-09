public class PlayAnimationEvent : CutsceneEvent
{
    public string animation;

    public PlayAnimationEvent(string animation)
    { this.animation = animation; }

    public PlayAnimationEvent() { }
}