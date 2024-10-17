using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    protected float ScreenShakeStrength;
    protected float ScreenShakeTime;

    private Vector3 targetCameraPos;
    protected Vector2 ScreenShakeDir;
    protected Vector3 CameraOffset;
    public bool followPlayer = true;

    public static PlayerCamera instance;
    public void Awake()
    {
        instance = this;
    }
    public void SetOnPlayer()
    {
        targetCameraPos = new Vector3(transform.position.x, transform.position.y, -60);
        transform.position = targetCameraPos;
    }

    public void SetCameraPos(Vector3 pos)
    {
        targetCameraPos = new Vector3(pos.x, pos.y, -60);
        transform.position = targetCameraPos;
    }

    public static void TriggerCameraShake(float strength, float time)
    {
        if (strength > instance.ScreenShakeStrength) instance.ScreenShakeStrength = strength;
        if (time > instance.ScreenShakeTime) instance.ScreenShakeTime = time;
    }

    public void ResetOffset()
    {
        ScreenShakeStrength = 0;
        ScreenShakeTime = 0;
        CameraOffset = Vector3.zero;
    }

    protected void HandleCamera()
    {
        if (followPlayer && Player.instance != null && Map.Instance != null)
        {
            if (Map.Instance.currentBoss != null)
            {
                Vector3 target = (new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, -60) + Map.Instance.currentBoss.transform.position) / 2f;
                target.z = -60;
                targetCameraPos = Vector3.Lerp(targetCameraPos, target, Time.deltaTime * 8f);
            }
            else
            {
                targetCameraPos = Vector3.Lerp(targetCameraPos, new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, -60), Time.deltaTime * 8f);
            }
        }

        // Camera ScreenShake
        if (ScreenShakeTime > 0)
        {
            ScreenShakeTime = Mathf.MoveTowards(ScreenShakeTime, 0, Time.deltaTime);
            float currentShakeStrength = ScreenShakeStrength * ScreenShakeTime;
            ScreenShakeDir = Random.insideUnitCircle * currentShakeStrength;
        }
        CameraOffset = Vector3.MoveTowards(CameraOffset, ScreenShakeDir, Time.deltaTime * 24f);

        Camera.main.transform.position = targetCameraPos + CameraOffset;
    }

    public void Update()
    {
        HandleCamera();
    }
}