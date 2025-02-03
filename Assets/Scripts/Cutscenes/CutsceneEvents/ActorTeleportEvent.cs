using UnityEngine;
using System;

[Serializable]
public class ActorTeleportEvent : CutsceneEvent
{
    public CutsceneActor actor;
    public Vector2 target;

    public ActorTeleportEvent(CutsceneActor actor, Vector2 target)
    {
        this.actor = actor;
        this.target = target;
    }

    public ActorTeleportEvent() { }
    public void Trigger()
    {
        actor.TeleportTo(target);
    }
}