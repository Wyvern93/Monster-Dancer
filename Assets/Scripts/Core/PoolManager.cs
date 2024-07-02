using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;

    private Dictionary<Type, Pool> pools;
    private void Awake()
    {
        instance = this;
        pools = new Dictionary<System.Type, Pool>();
    }

    public static void CreatePool(Type objectType, GameObject prefab, int initialSize)
    {
        if (!instance.pools.ContainsKey(objectType))
        {
            Pool newPool = new Pool(prefab.gameObject, initialSize);
            instance.pools[objectType] = newPool;
        }
    }

    public static T Get<T>() where T : MonoBehaviour
    {
        var type = typeof(T);
        if (instance.pools.ContainsKey(type))
        {
            return instance.pools[type].Get().GetComponent<T>();
        }
        else
        {
            Debug.LogError($"Pool for type {type} doesn't exist.");
            return null;
        }
    }

    public static void Return(GameObject obj, Type objtype)
    {
        if (instance.pools.ContainsKey(objtype))
        {
            instance.pools[objtype].Return(obj.gameObject);
        }
        else
        {
            Debug.LogError($"Pool for type {objtype} doesn't exist.");
        }
    }
}
