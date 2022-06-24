using System;
using UnityEngine;
using Zenject;

namespace Misc
{
    public enum BulletProjectileTypes
    {
        FromEnemy,
        FromPlayer
    }
    
    public class BulletProjectile : MonoBehaviour, IPoolable<float, float, BulletProjectileTypes, IMemoryPool>, IDisposable
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnDespawned()
        {
            throw new System.NotImplementedException();
        }

        public void OnSpawned(float p1, float p2, BulletProjectileTypes p3, IMemoryPool p4)
        {
            throw new System.NotImplementedException();
        }
        
        public class Factory : PlaceholderFactory<float, float, BulletProjectileTypes, BulletProjectile>
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
