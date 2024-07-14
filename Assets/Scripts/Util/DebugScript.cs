using UnityEngine;
using UnityEngine.InputSystem;

public class DebugScript : MonoBehaviour
{
    public void Start()
    {
        UIManager.Instance.PlayerUI.CreatePools();
    }
    public void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            for (int i = 0; i < 100; i++)
            {
                UIManager.Instance.PlayerUI.SpawnDamageText(Vector2.zero, 1, DamageTextType.Normal);
            }
        }
    }
}