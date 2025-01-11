using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCameraPoint : MonoBehaviour
{
    public StageCameraPoint next;
    public StagePointType pointType;

    private bool actionFinished;
    private bool triggered;

    [HideInInspector] public float time = 0;

    private void Awake()
    {
        time = 0;
        actionFinished = false;
    }

    // We trigger this when we get here
    public void Trigger()
    {
        if (actionFinished) return;

        if (pointType == StagePointType.Anchor)
        {
            actionFinished = true;
            return;
        }

        else if (pointType == StagePointType.Elite && !triggered)
        {
            Stage.Instance.nextWaveIsElite = true;
            triggered = true;
        }
        else if (pointType == StagePointType.Boss && !triggered)
        {
            Stage.Instance.nextWaveIsBoss = true;
            triggered = true;
        }   
    }

    public void OnEventFinish()
    {
        actionFinished = true;
    }

    public bool CanMoveToNext()
    {
        if (pointType == StagePointType.Anchor)
        {
            // Move when reaching the position of the anchor
            return CanTrigger();
        }
        else if (pointType == StagePointType.Elite)
        {
            bool camera = CanTrigger();
            if (actionFinished && camera) return true;
            else return false;
        }
        else
        {
            return false;
        }
    }

    public bool CanTrigger()
    {
        return (Vector2)PlayerCamera.instance.transform.position == (Vector2)transform.position;
    }

    public void CalculateNextDistance()
    {
        if (next == null) return;
        float distance = Vector2.Distance(transform.position, next.transform.position);
        float speed = 0.6f;
        float seconds = distance / speed;

        next.time = seconds;
    }
}

public enum StagePointType
{
    Anchor, Elite, Boss
}