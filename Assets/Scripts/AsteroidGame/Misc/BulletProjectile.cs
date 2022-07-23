using System;
using AsteroidGame.PlayerScripts;
using AsteroidGame.UfoScripts;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Misc
{
    public class BulletProjectile : MonoBehaviour, IPoolable<float, float, ObjectTypes, IMemoryPool>, IDisposable
    {
        public ObjectTypes OriginType { get; private set; }
        
        private BoundHandler _boundHandler;
        private GameState _gameState;
        
        private float _speed;
        
        private IMemoryPool _pool;
        private float _lifespan;
        private IDisposable _spawnTimer;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            BoundHandler boundHandler,
            GameState gameState)
        {
            _boundHandler = boundHandler;
            _gameState = gameState;
        }

        private void Start()
        {
            DisposeIfGameNotRunning();
        }

        private void Update()
        {
            var projTransform = transform;
            
            var position = projTransform.position + projTransform.forward * (_speed * Time.deltaTime);
            projTransform.position = _boundHandler.EnforceBounds(position);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (IsUfoFiringBullet(other) || IsPlayerFiringBullet(other) || IsTeammateColliding(other))
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
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private bool IsTeammateColliding(Collider other)
        {
            return _gameState.GameMode.Value == 2 && other.GetComponent<PlayerFacade>() != null &&
                   OriginType == ObjectTypes.Player ||
                   OriginType == ObjectTypes.OtherPlayer;
        }

        private bool IsUfoFiringBullet(Collider other)
        {
            return other.GetComponent<Ufo>() != null &&
                   (OriginType == ObjectTypes.LargeUfo || OriginType == ObjectTypes.SmallUfo);
        }
        
        private bool IsPlayerFiringBullet(Collider other)
        {
            return other.GetComponent<PlayerFacade>() != null &&
                   (other.GetComponent<PlayerFacade>().PlayerType == ObjectTypes.Player &&
                    OriginType == ObjectTypes.Player ||
                    other.GetComponent<PlayerFacade>().PlayerType == ObjectTypes.OtherPlayer &&
                    OriginType == ObjectTypes.OtherPlayer);
        }
        
        private void DespawnProjectile()
        {
            _spawnTimer = Observable
                .Timer(TimeSpan.FromSeconds(_lifespan))
                .Subscribe(_ =>
                {
                    Dispose();
                })
                .AddTo(_disposables);        
        }
    }
}
