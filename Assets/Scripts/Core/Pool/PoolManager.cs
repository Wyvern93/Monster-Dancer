using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public static void ResetPools()
    {
        
        foreach (KeyValuePair<Type,Pool> poolType in instance.pools)
        {
            RemovePool(poolType.Key);
        }
        instance.pools.Clear();
    }

    public static void RemovePool(Type objectType)
    {
        if (instance.pools.ContainsKey(objectType))
        {
            Pool pool = instance.pools[objectType];
            pool.DestroyPool();
        }
    }

    public static T Get<T>() where T : Component
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
