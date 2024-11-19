using UnityEngine;

public class SlownessEffect : MonoBehaviour
{
    public int duration;
    public float amount;
    public bool isSlowed()
    {
        return duration > 0;
    }

    public float ApplySlow(float speed)
    {
        return speed * (1 - amount);
    }
}