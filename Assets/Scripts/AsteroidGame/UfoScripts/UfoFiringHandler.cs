using System;
using System.Collections.Generic;
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
        private readonly InstanceRegistry _instanceRegistry;
        
        private float _ufoBulletProjectileSpeed;
        private float _ufoBulletProjectileLifespan;

        private float _ufoMinFireDelay;
        private float _ufoMaxFireDelay;
        private float _ufoOffsetConstant;
        private float _ufoOffsetMin;
        private float _ufoOffsetMax;
        
        private IDisposable _fireDelayTimer;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UfoFiringHandler(
            Ufo ufo,
            BulletProjectile.Factory bulletProjectileFactory,
            Settings settings,
            Difficulty.Settings difficultySettings,
            GameState gameState,
            InstanceRegistry instanceRegistry)
        {
            _ufo = ufo;
            _bulletProjectileFactory = bulletProjectileFactory;
            _settings = settings;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
            _instanceRegistry = instanceRegistry;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
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
                .AddTo(_disposables);
        }

        private void FireProjectileBullets()
        {
            var bulletProjectile =
                _bulletProjectileFactory.Create(_ufoBulletProjectileSpeed, _ufoBulletProjectileLifespan, _ufo.Size);

            var transform = bulletProjectile.transform;
            transform.position = _ufo.transform.position;
            transform.forward = DetermineUfoProjectileFacing();
            transform.localScale = .15f * Vector3.one;

            bulletProjectile.name = "Ufo Projectile";
            
            FireDelay();
        }

        private Vector3 DetermineUfoProjectileFacing()
        {
            Vector3 result = new Vector3(Random.Range(-2.5f, 2.5f),1, Random.Range(-2.5f, 2.5f));
            
            var targetList = new List<PlayerFacade>();
            
            foreach (var playerFacade in _instanceRegistry.playerFacades)
            {
                if (!playerFacade.IsDead && !playerFacade.HyperspaceWasTriggered)
                {
                    targetList.Add(playerFacade);
                }
            }

            if (targetList.Count > 0)
            {
                var target = targetList[Random.Range(0, targetList.Count)];
                
                var xTarget = target.Position.x + _ufoOffsetConstant + Random.Range(_ufoOffsetMin, _ufoOffsetMax);
                var zTarget = target.Position.z + _ufoOffsetConstant + Random.Range(_ufoOffsetMin, _ufoOffsetMax);
            
                xTarget = Random.Range(0, 1f) > .51 ? xTarget : -xTarget;
                zTarget = Random.Range(0, 1f) > .51 ? zTarget : -zTarget;

                result = new Vector3(xTarget, 1, zTarget);
            }

            return result;
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
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float ufoProjSpeed;
            public float ufoProjLifespan;
        }
    }
}
