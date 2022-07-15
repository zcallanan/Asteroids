using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerFiringHandler : IInitializable
    {
        private readonly Player _player;
        private readonly BulletProjectile.Factory _bulletProjectileFactory;
        private readonly InputState _inputState;
        private readonly Settings _settings;
        private readonly GameState _gameState;

        private bool _firingDisabled;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerFiringHandler(
            Player player,
            BulletProjectile.Factory bulletProjectileFactory,
            InputState inputState,
            Settings settings,
            GameState gameState)
        {
            _player = player;
            _bulletProjectileFactory = bulletProjectileFactory;
            _inputState = inputState;
            _settings = settings;
            _gameState = gameState;
        }

        public void Initialize()
        {
            RemoveThrottleOnFiringFromSceneTransition();
            
            FireProjectileAndEnforceCooldownDelay();

            DisposeIfGameNotRunning();
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
        
        private void RemoveThrottleOnFiringFromSceneTransition()
        {
            _gameState.IsFiringEnabled
                .Throttle(TimeSpan.FromSeconds(_settings.firingEnabledDelay))
                .Subscribe(isFiringEnabled =>
                {
                    if (!isFiringEnabled)
                    {
                        _gameState.IsFiringEnabled.Value = true;
                    }
                })
                .AddTo(_disposables);
        }

        private void FireProjectileAndEnforceCooldownDelay()
        {
            _inputState.IsFiring.Subscribe(hasFired =>
            {
                if (hasFired && !_player.IsDead && !_firingDisabled && _gameState.IsFiringEnabled.Value)
                {
                    FireProjectileBullets();
                    _firingDisabled = true;

                    FiringCooldownDelay();
                }
            })
                .AddTo(_disposables);
        }

        private void FiringCooldownDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.fireCooldown))
                .Subscribe(_ => _firingDisabled = false)
                .AddTo(_disposables);
        }

        private void FireProjectileBullets()
        {
            var bulletProjectile = _bulletProjectileFactory.Create(
                _settings.projectileSpeed, _settings.projectileLifespan, ObjectTypes.Player);

            var transform = bulletProjectile.transform;
            transform.position = _player.Position;
            transform.rotation = _player.Transform.rotation;
            transform.localScale = .1f * Vector3.one;
            
            bulletProjectile.name = "Player Projectile";
        }

        [Serializable]
        public class Settings
        {
            public float fireCooldown;
            public float projectileSpeed;
            public float projectileLifespan;
            public float firingEnabledDelay;
        }

        
    }
}
