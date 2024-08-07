using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    public Pool[] pools;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> objectPool))
        {
            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.TryGetValue(obj, out Queue<GameObject> objectPool))
        {
            objectPool.Enqueue(obj);
        }
        else
        {
            Destroy(obj); // If it's not in the pool dictionary, destroy it
        }
    }
}