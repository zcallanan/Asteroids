using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidScripts
{
    public class AsteroidExplosionHandler : IInitializable
    {
        private readonly AsteroidFacade _asteroidFacade;
        private readonly Explosion.Factory _explosionFactory;

        private Explosion _explosion;
        
        private ParticleSystem _expParticleSystem;
        private ParticleSystem.MainModule _expMain;

        private AsteroidFacade.AsteroidSizes _small;
        private AsteroidFacade.AsteroidSizes _medium;
        private AsteroidFacade.AsteroidSizes _large;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AsteroidExplosionHandler(
            AsteroidFacade asteroidFacade,
            Explosion.Factory explosionFactory)
        {
            _asteroidFacade = asteroidFacade;
            _explosionFactory = explosionFactory;
        }
        
        public void Initialize()
        {
            _small = AsteroidFacade.AsteroidSizes.SmallAsteroid;
            _medium = AsteroidFacade.AsteroidSizes.MediumAsteroid;
            _large = AsteroidFacade.AsteroidSizes.LargeAsteroid;
            
            _asteroidFacade
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => CreateExplosion())
                .AddTo(_disposables);
            
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
        
        private void CreateExplosion()
        {
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _asteroidFacade.Position;
            
            _expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            _expMain = _expParticleSystem.main;

            if (_asteroidFacade.Size == _small)
            {
                _expMain.startSpeed = 1.0f;
            }
            else if (_asteroidFacade.Size == _medium)
            {
                _expMain.startSpeed = 3.0f;
            }
            else if (_asteroidFacade.Size == _large)
            {
                _expMain.startSpeed = 5.0f;
            }
            
            _expParticleSystem.Clear();
            _expParticleSystem.Play();
        }
    }
}
