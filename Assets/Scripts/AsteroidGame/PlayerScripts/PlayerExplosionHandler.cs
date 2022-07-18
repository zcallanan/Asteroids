using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerExplosionHandler : IInitializable
    {
        private readonly Player _player;
        private readonly Explosion.Factory _explosionFactory;
        private readonly GameState _gameState;

        private Collider _collider;
        private Explosion _explosion;

        private Color _startColor;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerExplosionHandler(
            Player player,
            Explosion.Factory explosionFactory,
            GameState gameState)
        {
            _player = player;
            _explosionFactory = explosionFactory;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                CheckIfPlayersSpawned();

                DisposeIfGameNotRunning();
            }
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
        
        private void CheckIfPlayersSpawned()
        {
            _gameState.ArePlayersSpawned
                .Subscribe(playersSpawned =>
                {
                    if (playersSpawned)
                    {
                        InitializePlayerExplosionHandler();
                    }
                })
                .AddTo(_disposables);
        }

        private void InitializePlayerExplosionHandler()
        {
            _startColor = new Color(0, 1, 1, 1f);
            
            ExplodeOnTriggerEnter();

            DelayThenDespawnExplosion();
        }

        private void CreateExplosion()
        {
            if (IsExplodingFromFiredBullets())
            {
                return;
            }
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _player.Position;
            
            var expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            var expMain = expParticleSystem.main;
            
            expMain.startSpeed = 1f;
            expMain.startColor = _startColor;

            expParticleSystem.Clear();
            expParticleSystem.Play();
        }

        private bool IsExplodingFromFiredBullets()
        {
            if (_collider.GetComponent<BulletProjectile>())
            {
                var originType = _collider.GetComponent<BulletProjectile>().OriginType;

                if (originType == (_player.PlayerType == ObjectTypes.Player ? ObjectTypes.Player : ObjectTypes.OtherPlayer))
                {
                    return true;
                }
            }

            return false;
        }

        private void ExplodeOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    if (_player.CanCollide)
                    {
                        _collider = collider;
                    
                        CreateExplosion();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void DelayThenDespawnExplosion()
        {
            _player.GameObj
                .OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    DespawnDelay();
                })
                .AddTo(_disposables);
        }

        private void DespawnDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(1))
                .Subscribe(_ => _explosion.Dispose())
                .AddTo(_disposables);
        }
    }
}
