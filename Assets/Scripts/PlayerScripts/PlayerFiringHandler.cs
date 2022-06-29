using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFiringHandler : IInitializable, IDisposable
    {
        private readonly Player _player;
        private readonly BulletProjectile.Factory _bulletProjectileFactory;
        private readonly PlayerInputState _playerInputState;
        private readonly Settings _settings;
        private readonly GameState _gameState;

        private bool _firingDisabled;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerFiringHandler(
            Player player,
            BulletProjectile.Factory bulletProjectileFactory,
            PlayerInputState playerInputState,
            Settings settings,
            GameState gameState)
        {
            _player = player;
            _bulletProjectileFactory = bulletProjectileFactory;
            _playerInputState = playerInputState;
            _settings = settings;
            _gameState = gameState;
        }

        public void Initialize()
        {
            FireProjectileAndEnforceCooldownDelay();
            
            Dispose();
        }
        
        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void FireProjectileAndEnforceCooldownDelay()
        {
            _playerInputState.IsFiring.Subscribe(hasFired =>
            {
                if (hasFired && !_player.IsDead && !_firingDisabled)
                {
                    FireProjectileBullets();
                    _firingDisabled = true;

                    Observable
                        .Timer(TimeSpan.FromSeconds(_settings.fireCooldown))
                        .Subscribe(_ => _firingDisabled = false)
                        .AddTo(_disposables);
                }
            }).AddTo(_disposables);
        }

        private void FireProjectileBullets()
        {
            var bulletProjectile = _bulletProjectileFactory.Create(
                _settings.projectileSpeed, _settings.projectileLifespan, BulletProjectileTypes.FromPlayer);

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
        }

        
    }
}
