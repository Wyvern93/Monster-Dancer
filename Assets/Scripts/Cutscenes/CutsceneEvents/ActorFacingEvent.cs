using UnityEngine;
using System;

[Serializable]
public class ActorFacingEvent : CutsceneEvent
{
    public CutsceneActor actor;
    public bool facingRight;

    public ActorFacingEvent(CutsceneActor actor, bool facingRight)
    {
        this.actor = actor;
        this.facingRight = facingRight;
    }

    public ActorFacingEvent() { }
    public void Trigger()
    {
        actor.facingRight = facingRight;
    }
}