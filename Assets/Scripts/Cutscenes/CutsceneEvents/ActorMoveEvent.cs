using UnityEngine;
using System;

[Serializable]
public class ActorMoveEvent : CutsceneEvent
{
    public CutsceneActor actor;
    public Vector2 target;

    public ActorMoveEvent(CutsceneActor actor, Vector2 target)
    {
        this.actor = actor;
        this.target = target;
    }

    public ActorMoveEvent() { }
    public void Trigger()
    {
        actor.MoveTo(target);
    }
}