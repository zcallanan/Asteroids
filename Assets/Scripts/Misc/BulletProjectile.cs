using System;
using UnityEngine;
using Zenject;

namespace Misc
{
    public enum BulletProjectileTypes
    {
        FromUfo,
        FromPlayer
    }
    
    public class BulletProjectile : MonoBehaviour, IPoolable<float, float, BulletProjectileTypes, IMemoryPool>
    {
        private BoundHandler _boundHandler;

        private float _whenSpawned;
        private float _speed;
        private float _lifespan;

        private IMemoryPool _pool;
        
        [Inject]
        public void Construct(BoundHandler boundHandler)
        {
            _boundHandler = boundHandler;
        }
        
        public void Update()
        {
            var projTransform = transform;
            
            var position = projTransform.position + projTransform.forward * (_speed * Time.deltaTime);
            projTransform.position = _boundHandler.EnforceBounds(position); 

            if (Time.realtimeSinceStartup - _whenSpawned >= _lifespan)
            {
                _pool.Despawn(this);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            _pool.Despawn(this);
        }
        
        public void OnSpawned(float speed, float lifespan, BulletProjectileTypes types, IMemoryPool pool)
        {
            _speed = speed;
            _lifespan = lifespan;
            _pool = pool;
            
            _whenSpawned = Time.realtimeSinceStartup;
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public class Factory : PlaceholderFactory<float, float, BulletProjectileTypes, BulletProjectile>
        {
        }
    }
}
