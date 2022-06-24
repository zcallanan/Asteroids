using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFiringHandler : IInitializable
    {
        private readonly Player _player;
        private readonly BulletProjectile.Factory _bulletProjectileFactory;
        private readonly PlayerInputState _playerInputState;
        private readonly Settings _settings;

        private float _lastFireTime;
        
        // TODO: clear on game over
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerFiringHandler(
            Player player,
            BulletProjectile.Factory bulletProjectileFactory,
            PlayerInputState playerInputState,
            Settings settings)
        {
            _player = player;
            _bulletProjectileFactory = bulletProjectileFactory;
            _playerInputState = playerInputState;
            _settings = settings;
        }

        public void Initialize()
        {
            _playerInputState.IsFiring.Subscribe(hasFired =>
            {
                if (!_player.IsDead && hasFired && Time.realtimeSinceStartup - _lastFireTime >= _settings.fireCooldown)
                {
                    _lastFireTime = Time.realtimeSinceStartup;
                    Fire();
                }
            }).AddTo(_disposables);
        }

        private void Fire()
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
