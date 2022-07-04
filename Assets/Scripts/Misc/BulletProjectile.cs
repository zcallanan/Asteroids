using System;
using PlayerScripts;
using UfoScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class BulletProjectile : MonoBehaviour, IPoolable<float, float, ObjectTypes, IMemoryPool>, IDisposable
    {
        public ObjectTypes OriginType { get; private set; }
        
        private BoundHandler _boundHandler;
        
        private float _speed;
        
        private IMemoryPool _pool;
        private float _lifespan;
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
            if (other.GetComponent<Ufo>() != null && (OriginType == ObjectTypes.LargeUfo || OriginType == ObjectTypes.SmallUfo))
            {
                return;
            }
            
            if (other.GetComponent<PlayerFacade>() != null && OriginType == ObjectTypes.Player )
            {
                return;
            }
            
            _spawnTimer.Dispose();
        
            Dispose();
        }
        
        public void OnSpawned(float speed, float lifespan, ObjectTypes originType, IMemoryPool pool)
        {
            _speed = speed;
            _lifespan = lifespan;
            _pool = pool;
            OriginType = originType;

            DespawnProjectile();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }

        public class Factory : PlaceholderFactory<float, float, ObjectTypes, BulletProjectile>
        {
        }
        
        private void DespawnProjectile()
        {
            _spawnTimer = Observable
                .Timer(TimeSpan.FromSeconds(_lifespan))
                .Subscribe(_ =>
                {
                    Dispose();
                })
                .AddTo(this);        
        }
    }
}
