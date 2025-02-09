public class WaitForActorToEndEvent : CutsceneEvent
{
    public CutsceneActor actor;

    public WaitForActorToEndEvent(CutsceneActor actor)
    {
        this.actor = actor;
    }

    public WaitForActorToEndEvent() { }
    public bool HasFinished()
    {
        return actor.isActionFinished;
    }
}