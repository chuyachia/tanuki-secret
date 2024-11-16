using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject _prefab;
    private List<GameObject> _pool = new List<GameObject>();

    public ObjectPool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new List<GameObject>();
    }

    public GameObject Get()
    {
        GameObject instance;
        int last = _pool.Count - 1;
        if (last >= 0)
        {
            instance = _pool[last];
            instance.gameObject.SetActive(true);
            _pool.RemoveAt(last);
        }
        else
        {
            instance = Object.Instantiate(_prefab);
        }
        return instance;
    }

    public void Reclaim(GameObject instance)
    {
        _pool.Add(instance);
        instance.gameObject.SetActive(false);
    }

    public void DestroyAll()
    {
        foreach (GameObject instance in _pool)
        {
            Object.Destroy(instance);
        }
    }
}