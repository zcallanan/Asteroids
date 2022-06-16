using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pools
{
    public class PoolFactory<T> : MonoBehaviour where T: MonoBehaviour 
    {
        [SerializeField] private List<T> pooledObjectPrefabs;
        [SerializeField] int amountToPool;
        public static PoolFactory<T> SharedInstance;
        private List<T> _pooledObjects;
        private string _objectVariantTag;

        private void Awake()
        {
            SharedInstance = this;
        }

        private void Start()
        {
            _pooledObjects = new List<T>();
            foreach (T prefab in pooledObjectPrefabs)
            {
                for (int i = 0; i < amountToPool; i++)
                {
                    T pooledObject = Instantiate(prefab);
                    pooledObject.gameObject.SetActive(false);
                    _pooledObjects.Add(pooledObject);
                }
            }

            if (pooledObjectPrefabs.Count == 0)
            {
                Debug.Log("pooledObjectPrefabs is empty.");
            }
        }

        private T GetObjectFromPoolOrInstantiate()
        {
            var disabledObjects = _objectVariantTag == null
                ? _pooledObjects.FindAll(obj => !obj.gameObject.activeInHierarchy)
                : _pooledObjects.FindAll(obj =>
                    !obj.gameObject.activeInHierarchy && obj.gameObject.CompareTag(_objectVariantTag));
            
            if (disabledObjects.Count == 0)
            {
                var pooledObject = Instantiate(_objectVariantTag == null
                    ? pooledObjectPrefabs[Random.Range(0, pooledObjectPrefabs.Count)]
                    : pooledObjectPrefabs[int.Parse(_objectVariantTag)]);
                pooledObject.gameObject.SetActive(false);
                _pooledObjects.Add(pooledObject);
                return pooledObject;
            }
            return disabledObjects[Random.Range(0, disabledObjects.Count)];
        }

        public T GetPooledObject(string objectVariantTag = null)
        {
            _objectVariantTag = objectVariantTag;
            var randomObjectFromPool = GetObjectFromPoolOrInstantiate();

            while (randomObjectFromPool.gameObject.activeInHierarchy)
            {
                randomObjectFromPool = GetObjectFromPoolOrInstantiate();  
            }
            return randomObjectFromPool;
        }
    }
}
