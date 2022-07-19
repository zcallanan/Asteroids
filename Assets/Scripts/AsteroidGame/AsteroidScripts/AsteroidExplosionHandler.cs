using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.AsteroidScripts
{
    public class AsteroidExplosionHandler : IInitializable
    {
        private readonly Asteroid _asteroid;
        private readonly Explosion.Factory _explosionFactory;
        private readonly GameState _gameState;

        private Explosion _explosion;
        
        private ParticleSystem.MainModule _expMain;

        private Color _startColor;

        private ObjectTypes _small;
        private ObjectTypes _medium;
        private ObjectTypes _large;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public AsteroidExplosionHandler(
            Asteroid asteroid,
            Explosion.Factory explosionFactory,
            GameState gameState)
        {
            _asteroid = asteroid;
            _explosionFactory = explosionFactory;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                _small = ObjectTypes.SmallAsteroid;
                _medium = ObjectTypes.MediumAsteroid;
                _large = ObjectTypes.LargeAsteroid;

                _startColor = new Color(0.5176471f, 0.5019608f, 0.4313726f, 1f);
            
                ExplodeOnTriggerEnter();

                DelayThenDespawnExplosion();

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

        private void CreateExplosion()
        {
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _asteroid.Position;
            
            var expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            _expMain = expParticleSystem.main;

            if (_asteroid.Size == _small)
            {
                _expMain.startSpeed = .5f;
                _expMain.startColor = _startColor;
            }
            else if (_asteroid.Size == _medium)
            {
                _expMain.startSpeed = .75f;
                _expMain.startColor = _startColor;
            }
            else if (_asteroid.Size == _large)
            {
                _expMain.startSpeed = 1f;
                _expMain.startColor = _startColor;
            }
            
            expParticleSystem.Clear();
            expParticleSystem.Play();
        }

        private void ExplodeOnTriggerEnter()
        {
            _asteroid
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => CreateExplosion())
                .AddTo(_disposables);
        }

        private void DelayThenDespawnExplosion()
        {
            _asteroid
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
