using System;
using System.Collections.Generic;
using Installers;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace AsteroidScripts
{
    public class AsteroidSpawner : IInitializable
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

            _gameState.CurrentLevel.Subscribe(level =>
            {
                Debug.Log($"The level is: {level}");
                for (int i = 0; i < _initLargeAsteroids; i++)
                {
                    var n = Random.Range(0, 4);
                    SpawnAsteroid(n, AsteroidFacade.AsteroidSizes.LargeAsteroid, Vector3.zero);
                }
            }).AddTo(_disposables);
        }
        
        public void SpawnAsteroid(int renderValue, AsteroidFacade.AsteroidSizes asteroidSize, Vector3 largerAsteroidPosition)
        {

            AsteroidFacade asteroidFacade = _asteroidFactory.Create(renderValue, asteroidSize);

            Vector3 tempPosition;
            
            if (asteroidSize == AsteroidFacade.AsteroidSizes.SmallAsteroid || asteroidSize == AsteroidFacade.AsteroidSizes.MediumAsteroid)
            {
                tempPosition = largerAsteroidPosition;
            }
            else
            {
                tempPosition = DetermineLargeAsteroidsPosition();
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

        private Vector3 DetermineLargeAsteroidsPosition()
         {
             var side = Random.Range(0, 4);
             var positionResult = side switch
             {
                 0 => new Vector3(Random.Range(_minBounds.x, _maxBounds.x), 1.0f, Random.Range(_innerMaxBounds.z, _maxBounds.z)), // Top
                 1 => new Vector3(Random.Range(_innerMaxBounds.x, _maxBounds.x), 1.0f, Random.Range(_minBounds.z, _maxBounds.z)), // Right
                 2 => new Vector3(Random.Range(_minBounds.x, _maxBounds.x), 1.0f, Random.Range(_innerMinBounds.z, _minBounds.z)), // Bottom
                 3 => new Vector3(Random.Range(_innerMinBounds.x, _minBounds.x), 1.0f, Random.Range(_minBounds.z, _maxBounds.z)), // Left
                 _ => throw new ArgumentOutOfRangeException(nameof(side), $"Not expected side value: {side}"),
             };
             return positionResult;
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
