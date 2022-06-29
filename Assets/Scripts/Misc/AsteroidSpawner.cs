using System;
using System.Collections.Generic;
using AsteroidScripts;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Misc
{
    public class AsteroidSpawner : IInitializable, IDisposable
    {
        private readonly Settings _settings;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;
        private readonly AsteroidFacade.Factory _asteroidFactory;
        private readonly BoundHandler _boundHandler;

        private int _gameDifficulty;
        private int _initLargeAsteroids;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;
        private Vector3 _innerMaxBounds;
        private Vector3 _innerMinBounds;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AsteroidSpawner(
            Settings settings,
            Difficulty.Settings difficultySettings,
            GameState gameState,
            AsteroidFacade.Factory asteroidFactory,
            BoundHandler boundHandler)
        { 
            _settings = settings;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
            _asteroidFactory = asteroidFactory;
            _boundHandler = boundHandler;
        }
        
        public void Initialize()
        {
            _gameDifficulty = _gameState.GameDifficulty;
            _initLargeAsteroids = _difficultySettings.difficulties[_gameDifficulty].initLargeAsteroids;
            
            DetermineAsteroidSpawnBoundValues();

            UpdateAsteroidSpawnBounds();

            SpawnAsteroidsWithinBoundsAtTheStartOfALevel();
            
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
        
        public void SpawnAsteroid(int renderValue, AsteroidFacade.AsteroidSizes asteroidSize, Vector3 largerAsteroidPosition)
        {
            var asteroidFacade = _asteroidFactory.Create(renderValue, asteroidSize);

            Vector3 tempPosition;
            
            if (asteroidSize == AsteroidFacade.AsteroidSizes.SmallAsteroid ||
                asteroidSize == AsteroidFacade.AsteroidSizes.MediumAsteroid)
            {
                tempPosition = largerAsteroidPosition;
            }
            else
            {
                tempPosition = AsteroidSpawnLocation.DetermineLargeAsteroidsPosition(_minBounds, _maxBounds,
                    _innerMinBounds, _innerMaxBounds);
            }
            
            asteroidFacade.Position = tempPosition;
            asteroidFacade.name = $"{asteroidFacade.Size} {asteroidFacade.RenderValue}";

            RenderAsteroid(asteroidFacade);
            ScaleAsteroid(asteroidFacade);
        }

        private void RenderAsteroid(AsteroidFacade asteroidFacade)
        {
            asteroidFacade.MeshFilterMesh = _settings.meshFilterMesh[asteroidFacade.RenderValue];
            asteroidFacade.MeshRendererMaterial = _settings.meshRendererMaterials[asteroidFacade.RenderValue];
        }

        private void ScaleAsteroid(AsteroidFacade asteroidFacade)
        {
            if (asteroidFacade.Size == AsteroidFacade.AsteroidSizes.SmallAsteroid)
            {
                asteroidFacade.Scale = _settings.smallScale * Vector3.one;
            }
            else if (asteroidFacade.Size == AsteroidFacade.AsteroidSizes.MediumAsteroid)
            {
                asteroidFacade.Scale = _settings.mediumScale * Vector3.one;
            }
            else if (asteroidFacade.Size == AsteroidFacade.AsteroidSizes.LargeAsteroid)
            {
                asteroidFacade.Scale = _settings.largeScale * Vector3.one;
            }
        }
        
        private void DetermineAsteroidSpawnBoundValues()
         {
             _innerMaxBounds = new Vector3(_maxBounds.x - 1, 1, _maxBounds.z - 1);
             _innerMinBounds = new Vector3(_minBounds.x + 1, 1, _minBounds.z + 1);
         }

        private void UpdateAsteroidSpawnBounds()
        {
            _boundHandler.MaxBounds.Subscribe(maxGameBounds =>
            {
                _maxBounds = maxGameBounds;
                DetermineAsteroidSpawnBoundValues();
            }).AddTo(_disposables);
            
            _boundHandler.MinBounds.Subscribe(minGameBounds =>
            {
                _minBounds = minGameBounds;
                DetermineAsteroidSpawnBoundValues();
            }).AddTo(_disposables);
        }
        
        private void SpawnAsteroidsWithinBoundsAtTheStartOfALevel()
        {
            _gameState.CurrentLevel.Subscribe(level =>
            {
                for (int i = 0; i < _initLargeAsteroids + _gameState.CurrentLevel.Value; i++)
                {
                    var renderValue = Random.Range(0, 4);
                    SpawnAsteroid(renderValue, AsteroidFacade.AsteroidSizes.LargeAsteroid, Vector3.zero);
                }
            }).AddTo(_disposables);
        }
        
        [Serializable]
        public class Settings
        {
            public float largeScale;
            public float mediumScale;
            public float smallScale;
            public List<Material> meshRendererMaterials = new List<Material>();
            public List<Mesh> meshFilterMesh = new List<Mesh>();
        }
    }
}
