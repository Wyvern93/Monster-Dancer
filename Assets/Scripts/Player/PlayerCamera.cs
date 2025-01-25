using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCamera : MonoBehaviour
{
    protected float ScreenShakeStrength;
    protected float ScreenShakeTime;

    private Vector3 targetCameraPos;
    protected Vector2 ScreenShakeDir;
    protected Vector3 CameraOffset;
    public bool followPlayer = true;
    public Vector2 camDir;
    public Vector2 camVelocity;

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
        if (followPlayer && Player.instance != null && Stage.Instance != null)
        {
            if (Stage.Instance.currentBoss != null)
            {
                Vector3 target = (new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, -60) + Stage.Instance.currentBoss.transform.position) / 2f;//(new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, -60) + Stage.Instance.currentBoss.transform.position) / 2f;
                targetCameraPos = Vector3.Lerp(targetCameraPos, target, Time.deltaTime * 8f);
                camVelocity = Vector2.zero;
            }
            else if (Stage.Instance.currentStagePoint != null)
            {
                targetCameraPos = Vector2.MoveTowards(targetCameraPos, Stage.Instance.currentStagePoint.transform.position, Time.deltaTime * 0.1f * 6f);
                camVelocity = camDir * (0.6f * Time.deltaTime);
            }
        }
        targetCameraPos.z = -60;

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