using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class BulletProjectile : MonoBehaviour, IPoolable<float, float, ObjectTypes, IMemoryPool>
    {
        private BoundHandler _boundHandler;

        private float _speed;
        
        private ObjectTypes _originType;

        private IMemoryPool _pool;
        private IDisposable _spawnTimer;
        
        [Inject]
        public void Construct(
            BoundHandler boundHandler)
        {
            _boundHandler = boundHandler;
        }

        public void Update()
        {
            var projTransform = transform;
            
            var position = projTransform.position + projTransform.forward * (_speed * Time.deltaTime);
            projTransform.position = _boundHandler.EnforceBounds(position);
        }

        public void OnTriggerEnter(Collider other)
        {
            _spawnTimer.Dispose();
            
            _pool.Despawn(this);
        }
        
        public void OnSpawned(float speed, float lifespan, ObjectTypes originType, IMemoryPool pool)
        {
            _speed = speed;
            _pool = pool;
            _originType = originType;

            _spawnTimer = Observable
                .Timer(TimeSpan.FromSeconds(lifespan))
                .Subscribe(_ =>
                {
                    _pool?.Despawn(this);
                })
                .AddTo(this);
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public class Factory : PlaceholderFactory<float, float, ObjectTypes, BulletProjectile>
        {
        }
    }
}
