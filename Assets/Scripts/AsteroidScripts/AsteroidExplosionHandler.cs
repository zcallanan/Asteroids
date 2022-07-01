using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidScripts
{
    public class AsteroidExplosionHandler : IInitializable, IDisposable
    {
        private readonly Asteroid _asteroid;
        private readonly Explosion.Factory _explosionFactory;
        private readonly GameState _gameState;

        private Explosion _explosion;
        
        private ParticleSystem.MainModule _expMain;

        private Color _startColor;

        private Asteroid.AsteroidSizes _small;
        private Asteroid.AsteroidSizes _medium;
        private Asteroid.AsteroidSizes _large;
        
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
            _small = Asteroid.AsteroidSizes.SmallAsteroid;
            _medium = Asteroid.AsteroidSizes.MediumAsteroid;
            _large = Asteroid.AsteroidSizes.LargeAsteroid;

            _startColor = new Color(0.5176471f, 0.5019608f, 0.4313726f, 1f);
            
            ExplodeOnTriggerEnter();

            DelayThenDespawnExplosion();

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
                    Observable
                        .Timer(TimeSpan.FromSeconds(1))
                        .Subscribe(_ => _explosion.Dispose())
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);
        }
    }
}
