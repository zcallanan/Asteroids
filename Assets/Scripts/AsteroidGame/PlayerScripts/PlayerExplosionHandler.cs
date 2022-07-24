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
        private readonly PlayerCollisionHandler _playerCollisionHandler;

        private Collider _collider;
        private Explosion _explosion;

        private Color _startColor;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerExplosionHandler(
            Player player,
            Explosion.Factory explosionFactory,
            GameState gameState,
            PlayerCollisionHandler playerCollisionHandler)
        {
            _player = player;
            _explosionFactory = explosionFactory;
            _gameState = gameState;
            _playerCollisionHandler = playerCollisionHandler;
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
            if (_playerCollisionHandler.IsCollidingWithPlayerBullets(_collider))
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

        private void ExplodeOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    _collider = collider;
                    
                    if (_collider.GetComponent<PlayerFacade>())
                    {
                        PreventTeamCollisionExplosion();
                    }
                    else
                    {
                        CreateExplosion();

                    }
                })
                .AddTo(_disposables);
        }

        private void PreventTeamCollisionExplosion()
        {
            if (_gameState.GameMode.Value != 2)
            {
                CreateExplosion();
            }
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
