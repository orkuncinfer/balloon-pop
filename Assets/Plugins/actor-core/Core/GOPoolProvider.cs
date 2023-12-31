using UnityEngine;
using System.Collections.Generic;

public static class GOPoolProvider
{
    private static GameObjectPool _pool;

    public static GameObject Retrieve(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (_pool == null)
        {
            GameObject poolObject = new GameObject("Global Object Pool");
            _pool = poolObject.AddComponent<GameObjectPool>();
        }

        return _pool.RetrieveFromPool(prefab, position, rotation, parent);
    }
}