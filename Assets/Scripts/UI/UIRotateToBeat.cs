using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class UIRotateToBeat : MonoBehaviour
{
    public bool clockwise;
    float beatTime;
    float speed = 45f;

    float angle;

    public void Update()
    {
        if (BeatManager.isBeat) StartCoroutine(Rotate());
    }

    public IEnumerator Rotate()
    {
        float beatDuration = BeatManager.GetBeatDuration();
        float time = 0;
        beatTime = 1;
        
        while (time <= beatDuration)
        {
            float beatProgress = time / beatDuration;
            beatTime = Mathf.Lerp(1, 0f, beatProgress);
            if (clockwise) angle -= speed * beatTime * Time.unscaledDeltaTime;
            if (!clockwise) angle -= speed * beatTime * Time.unscaledDeltaTime;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, angle);

            if (angle < 0) angle += 360;
            if (angle > 360) angle -= 360;

            time += Time.unscaledDeltaTime;
            yield return null;
        }
        yield break;
    }
}