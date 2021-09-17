using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject bulletObjectToPool;
    public int bulletAmountToPool;
    public GameObject asteroidObjectToPool;
    public int asteroidAmountToPool;
    private int totalAmountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();

        putObjectToPool(bulletObjectToPool, bulletAmountToPool);
        putObjectToPool(asteroidObjectToPool, asteroidAmountToPool);

        totalAmountToPool = bulletAmountToPool + asteroidAmountToPool;
    }

    private void putObjectToPool(GameObject objectToPool, int amountToPool)
    {
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject(string objName)
    {
        for (int i = 0; i < totalAmountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name.IndexOf(objName) != -1)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }
}
