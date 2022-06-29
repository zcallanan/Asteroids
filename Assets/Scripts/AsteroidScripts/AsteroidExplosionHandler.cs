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
        private readonly AsteroidFacade _asteroidFacade;
        private readonly Explosion.Factory _explosionFactory;
        private readonly GameState _gameState;

        private Explosion _explosion;
        
        private ParticleSystem _expParticleSystem;
        private ParticleSystem.MainModule _expMain;

        private Color _startColor;

        private AsteroidFacade.AsteroidSizes _small;
        private AsteroidFacade.AsteroidSizes _medium;
        private AsteroidFacade.AsteroidSizes _large;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AsteroidExplosionHandler(
            AsteroidFacade asteroidFacade,
            Explosion.Factory explosionFactory,
            GameState gameState)
        {
            _asteroidFacade = asteroidFacade;
            _explosionFactory = explosionFactory;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            _small = AsteroidFacade.AsteroidSizes.SmallAsteroid;
            _medium = AsteroidFacade.AsteroidSizes.MediumAsteroid;
            _large = AsteroidFacade.AsteroidSizes.LargeAsteroid;

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

            _explosion.transform.position = _asteroidFacade.Position;
            
            _expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            _expMain = _expParticleSystem.main;

            if (_asteroidFacade.Size == _small)
            {
                _expMain.startSpeed = .5f;
                _expMain.startColor = _startColor;
            }
            else if (_asteroidFacade.Size == _medium)
            {
                _expMain.startSpeed = .75f;
                _expMain.startColor = _startColor;
            }
            else if (_asteroidFacade.Size == _large)
            {
                _expMain.startSpeed = 1f;
                _expMain.startColor = _startColor;
            }
            
            _expParticleSystem.Clear();
            _expParticleSystem.Play();
        }

        private void ExplodeOnTriggerEnter()
        {
            _asteroidFacade
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => CreateExplosion())
                .AddTo(_disposables);
        }

        private void DelayThenDespawnExplosion()
        {
            _asteroidFacade
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
