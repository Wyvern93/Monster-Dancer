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

    }
}