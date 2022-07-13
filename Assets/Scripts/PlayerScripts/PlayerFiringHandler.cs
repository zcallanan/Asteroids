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
        private readonly InputState _inputState;
        private readonly Settings _settings;

        private bool _firingDisabled;
        
        public PlayerFiringHandler(
            Player player,
            BulletProjectile.Factory bulletProjectileFactory,
            InputState inputState,
            Settings settings)
        {
            _player = player;
            _bulletProjectileFactory = bulletProjectileFactory;
            _inputState = inputState;
            _settings = settings;
        }

        public void Initialize()
        {
            FireProjectileAndEnforceCooldownDelay();
        }

        private void FireProjectileAndEnforceCooldownDelay()
        {
            _inputState.IsFiring.Subscribe(hasFired =>
            {
                if (hasFired && !_player.IsDead && !_firingDisabled)
                {
                    FireProjectileBullets();
                    _firingDisabled = true;

                    Observable
                        .Timer(TimeSpan.FromSeconds(_settings.fireCooldown))
                        .Subscribe(_ => _firingDisabled = false)
                        .AddTo(_player.GameObj);
                }
            }).AddTo(_player.GameObj);
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
        }

        
    }
}
