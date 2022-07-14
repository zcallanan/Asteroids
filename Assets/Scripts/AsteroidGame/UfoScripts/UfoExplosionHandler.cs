using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.UfoScripts
{
    public class UfoExplosionHandler : IInitializable
    {
        private readonly Ufo _ufo;
        private readonly Explosion.Factory _explosionFactory;
        private readonly GameState _gameState;

        private Collider _collider;
        private Explosion _explosion;
        private Color _startColor;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UfoExplosionHandler(
            Ufo ufo,
            Explosion.Factory explosionFactory,
            GameState gameState)
        {
            _ufo = ufo;
            _explosionFactory = explosionFactory;
            _gameState = gameState;
        }

        public void Initialize()
        {
            _startColor = new Color(0, 0.674f, 1f, 1f);
            
            ExplodeOnTriggerEnter();

            DelayThenDespawnExplosion();

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
        
        private void CreateExplosion()
        {
            if (IsExplodingFromFiredBullets())
            {
                return;
            }
            
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _ufo.transform.position;
            
            var expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            var expMain = expParticleSystem.main;

            if (_ufo.Size == ObjectTypes.SmallUfo)
            {
                expMain.startSpeed = 0.55f;
            }
            else if (_ufo.Size == ObjectTypes.LargeUfo)
            {
                expMain.startSpeed = 0.85f;
            }
            
            expMain.startColor = _startColor;

            expParticleSystem.Clear();
            expParticleSystem.Play();
        }

        private bool IsExplodingFromFiredBullets()
        {
            if (_collider.GetComponent<BulletProjectile>())
            {
                var originType = _collider.GetComponent<BulletProjectile>().OriginType;

                if (originType == ObjectTypes.LargeUfo || originType == ObjectTypes.SmallUfo)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExplodeOnTriggerEnter()
        {
            _ufo
                .OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    _collider = collider;
                    
                    CreateExplosion();
                })
                .AddTo(_disposables);
        }
        
        private void DelayThenDespawnExplosion()
        {
            _ufo
                .OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    ExplosionDespawnDelay();
                })
                .AddTo(_disposables);
        }

        private void ExplosionDespawnDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(1))
                .Subscribe(_ => _explosion.Dispose())
                .AddTo(_disposables);
        }
    }
}
