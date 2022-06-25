using System;
using Installers;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Asteroid
{
    public class AsteroidSpawner : IInitializable
    {
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;
        private readonly AsteroidFacade.Factory _asteroidFactory;
        private readonly BoundHandler _boundHandler;

        private int _gameDifficulty;
        private float _minSpeed;
        private float _maxSpeed;
        private int _initLargeAsteroids;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;
        private Vector3 _innerMaxBounds;
        private Vector3 _innerMinBounds;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AsteroidSpawner(
            Difficulty.Settings difficultySettings,
            GameState gameState,
            AsteroidFacade.Factory asteroidFactory,
            BoundHandler boundHandler)
        {
            _difficultySettings = difficultySettings;
            _gameState = gameState;
            _asteroidFactory = asteroidFactory;
            _boundHandler = boundHandler;
        }
        
        public void Initialize()
        {
            _gameDifficulty = _gameState.GameDifficulty;
            _minSpeed = _difficultySettings.difficulties[_gameDifficulty].astMinSpeed;
            _maxSpeed = _difficultySettings.difficulties[_gameDifficulty].astMaxSpeed;
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
                for (int i = 0; i < _initLargeAsteroids; i++)
                {
                    SpawnAsteroid(AsteroidSizes.LargeAsteroid, Vector3.zero);
                }
            }).AddTo(_disposables);
        }
        
        public void SpawnAsteroid(AsteroidSizes asteroidSize, Vector3 largerAsteroidPosition)
        {
            float speed = Random.Range(_minSpeed, _maxSpeed);

            var asteroidFacade = _asteroidFactory.Create(speed, asteroidSize);
            
            Vector3 tempPosition;
            
            if (asteroidSize == AsteroidSizes.SmallAsteroid || asteroidSize == AsteroidSizes.MediumAsteroid)
            {
                tempPosition = largerAsteroidPosition;
            }
            else
            {
                tempPosition = DetermineLargeAsteroidsPosition();
            }
            
            asteroidFacade.Position = tempPosition;
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
            
        }
    }
}
