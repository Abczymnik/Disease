using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodHitPool : MonoBehaviour
{
    public static BloodHitPool sharedInstance;
    [SerializeField]
    private List<GameObject> pooledObjects;
    [SerializeField]
    private GameObject objectToPool;
    [SerializeField]
    private int amountToPool;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tempObj;
        for(int i=0; i < amountToPool; i++)
        {
            tempObj = Instantiate(objectToPool);
            tempObj.transform.SetParent(transform);
            tempObj.SetActive(false);
            pooledObjects.Add(tempObj);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i=0; i < amountToPool; i++)
        {
            if(!pooledObjects[i].activeInHierarchy) { return pooledObjects[i]; }
        }

        return null;
    }
}
