using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [SerializeField] protected int PoolCapacity;
    [SerializeField] protected Transform Container;

    private List<GameObject> _pool = new List<GameObject>();

    protected void Init(GameObject objectPrefab)
    {
        for (int i = 0; i < PoolCapacity; i++)
        {
            var spawnedObject = Instantiate(objectPrefab, Container);
            spawnedObject.SetActive(false);

            _pool.Add(spawnedObject);
        }
    }

    protected bool TryGetObject(out GameObject result)
    {
        result = _pool.Find(p => p.activeSelf == false);

        return result != null;
    }

    protected bool ReturnObject(GameObject item)
    {
        bool isFree = _pool.Contains(item);
        if (isFree)
        {
            item.SetActive(false);
            item.transform.SetParent(Container);
        }
        return isFree;
    }
}
