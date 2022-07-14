using System;
using AsteroidGame.Misc;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace AsteroidGame.UfoScripts
{
    public class UfoFiringHandler : IInitializable
    {
        private readonly Ufo _ufo;
        private readonly BulletProjectile.Factory _bulletProjectileFactory;
        private readonly Settings _settings;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;
        private readonly PlayerFacade _playerFacade;
        
        private BulletProjectile _bulletProjectile;
        
        private float _ufoBulletProjectileSpeed;
        private float _ufoBulletProjectileLifespan;

        private float _ufoMinFireDelay;
        private float _ufoMaxFireDelay;
        private float _ufoOffsetConstant;
        private float _ufoOffsetMin;
        private float _ufoOffsetMax;
        
        private IDisposable _fireDelayTimer;

        public UfoFiringHandler(
            Ufo ufo,
            BulletProjectile.Factory bulletProjectileFactory,
            Settings settings,
            Difficulty.Settings difficultySettings,
            GameState gameState,
            PlayerFacade playerFacade)
        {
            _ufo = ufo;
            _bulletProjectileFactory = bulletProjectileFactory;
            _settings = settings;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
            _playerFacade = playerFacade;
        }
        
        public void Initialize()
        {
            _ufoBulletProjectileSpeed = _settings.ufoProjSpeed;
            _ufoBulletProjectileLifespan = _settings.ufoProjLifespan;

            var difficulties = _difficultySettings.difficulties[_gameState.GameDifficulty.Value];
            
            _ufoMinFireDelay = difficulties.ufoMinFireDelay;
            _ufoMaxFireDelay = difficulties.ufoMaxFireDelay;
            
            _ufoOffsetConstant = difficulties.ufoOffsetConstant;
            _ufoOffsetMin = difficulties.ufoOffsetMin;
            _ufoOffsetMax = difficulties.ufoOffsetMax;
            
            CheckIfUfoIsDead();
        }

        private void FireDelay(bool isInitial = false)
        {
            float delayTime = Random.Range(_ufoMinFireDelay, _ufoMaxFireDelay);
            
            if (isInitial)
            {
                delayTime = Random.Range(1.75f, 2.25f);
            }
            
            _fireDelayTimer = Observable
                .Timer(TimeSpan.FromSeconds(delayTime))
                .Subscribe(_ => FireProjectileBullets())
                .AddTo(_ufo.gameObject);
        }

        private void FireProjectileBullets()
        {
            _bulletProjectile =
                _bulletProjectileFactory.Create(_ufoBulletProjectileSpeed, _ufoBulletProjectileLifespan, _ufo.Size);

            var transform = _bulletProjectile.transform;
            transform.position = _ufo.transform.position;
            transform.forward = DetermineUfoProjectileFacing();
            transform.localScale = .15f * Vector3.one;

            _bulletProjectile.name = "Ufo Projectile";
            
            FireDelay();
        }

        private Vector3 DetermineUfoProjectileFacing()
        {
            var xTarget = _playerFacade.Position.x + _ufoOffsetConstant + Random.Range(_ufoOffsetMin, _ufoOffsetMax);
            var zTarget =_playerFacade.Position.z + _ufoOffsetConstant + Random.Range(_ufoOffsetMin, _ufoOffsetMax);
            
            xTarget = Random.Range(0, 1f) > .51 ? xTarget : -xTarget;
            zTarget = Random.Range(0, 1f) > .51 ? zTarget : -zTarget;

            return new Vector3(xTarget, 1, zTarget);
        }
        
        private void CheckIfUfoIsDead()
        {
            _ufo.IsDead
                .Subscribe(isDead =>
                {
                    if (isDead)
                    {
                        _fireDelayTimer.Dispose();
                    }
                    else
                    {
                        FireDelay(true);
                    }
                })
                .AddTo(_ufo.gameObject);
        }

        [Serializable]
        public class Settings
        {
            public float ufoProjSpeed;
            public float ufoProjLifespan;
        }
    }
}
