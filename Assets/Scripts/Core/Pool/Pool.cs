using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    public GameObject prefab;
    public int initialSize;

    private Queue<GameObject> objects;

    public Pool (GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        objects = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            objects.Enqueue(obj);
        }
    }

    public void ExpandPool(int targetSize)
    {
        int amount = targetSize - initialSize;
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            objects.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (objects.Count > 0)
        {
            GameObject obj = objects.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }

    public void DestroyPool()
    {
        foreach (GameObject obj in objects)
        {
            GameObject.Destroy(obj);
        }
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        objects.Enqueue(obj);
    }
}
