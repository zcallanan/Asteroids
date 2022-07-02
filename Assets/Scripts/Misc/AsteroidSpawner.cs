using System;
using System.Collections.Generic;
using AsteroidScripts;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Misc
{
    public class AsteroidSpawner : IInitializable
    {
        private readonly Settings _settings;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;
        private readonly Asteroid.Factory _asteroidFactory;
        private readonly BoundHandler _boundHandler;

        private int _initLargeAsteroids;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;
        private Vector3 _innerMaxBounds;
        private Vector3 _innerMinBounds;
        
        public AsteroidSpawner(
            Settings settings,
            Difficulty.Settings difficultySettings,
            GameState gameState,
            Asteroid.Factory asteroidFactory,
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
            var gameDifficulty = _gameState.GameDifficulty;
            _initLargeAsteroids = _difficultySettings.difficulties[gameDifficulty].initLargeAsteroids;
            
            DetermineAsteroidSpawnBoundValues();

            UpdateAsteroidSpawnBounds();

            SpawnAsteroidsWithinBoundsAtTheStartOfALevel();
        }

        public void SpawnAsteroid(int renderValue, Asteroid.AsteroidSizes asteroidSize, Vector3 largerAsteroidPosition)
        {
            var asteroid = _asteroidFactory.Create(renderValue, asteroidSize);

            Vector3 tempPosition;
            
            if (asteroidSize == Asteroid.AsteroidSizes.SmallAsteroid ||
                asteroidSize == Asteroid.AsteroidSizes.MediumAsteroid)
            {
                tempPosition = largerAsteroidPosition;
            }
            else
            {
                tempPosition = AsteroidSpawnLocation.DetermineLargeAsteroidsPosition(_minBounds, _maxBounds,
                    _innerMinBounds, _innerMaxBounds);
            }
            
            asteroid.Position = tempPosition;
            asteroid.name = $"{asteroid.Size} {asteroid.RenderValue}";

            RenderAsteroid(asteroid);
            ScaleAsteroid(asteroid);
        }

        private void RenderAsteroid(Asteroid asteroid)
        {
            asteroid.SetMeshFilterMesh(_settings.meshFilterMesh[asteroid.RenderValue]);
            asteroid.SetMeshRendererMaterial(_settings.meshRendererMaterials[asteroid.RenderValue]);
        }

        private void ScaleAsteroid(Asteroid asteroid)
        {
            if (asteroid.Size == Asteroid.AsteroidSizes.SmallAsteroid)
            {
                asteroid.SetScale(_settings.smallScale * Vector3.one);
            }
            else if (asteroid.Size == Asteroid.AsteroidSizes.MediumAsteroid)
            {
                asteroid.SetScale(_settings.mediumScale * Vector3.one);
            }
            else if (asteroid.Size == Asteroid.AsteroidSizes.LargeAsteroid)
            {
                asteroid.SetScale(_settings.largeScale * Vector3.one);
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
            }).AddTo(_gameState.gameObject);
            
            _boundHandler.MinBounds.Subscribe(minGameBounds =>
            {
                _minBounds = minGameBounds;
                DetermineAsteroidSpawnBoundValues();
            }).AddTo(_gameState.gameObject);
        }
        
        private void SpawnAsteroidsWithinBoundsAtTheStartOfALevel()
        {
            _gameState.CurrentLevel.Subscribe(level =>
            {
                for (int i = 0; i < _initLargeAsteroids + _gameState.CurrentLevel.Value; i++)
                {
                    var renderValue = Random.Range(0, 4);
                    SpawnAsteroid(renderValue, Asteroid.AsteroidSizes.LargeAsteroid, Vector3.zero);
                }
            }).AddTo(_gameState.gameObject);
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
