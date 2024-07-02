using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEffect : MonoBehaviour
{
    public void End()
    {
        PoolManager.Return(gameObject, GetType());
    }
}
