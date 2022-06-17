using System;
using Data;
using Models;
using Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class AsteroidController : MonoBehaviour
    {
        private Vector3 _maxBounds;
        private Vector3 _minBounds;
        private Vector3 _innerMaxBounds;
        private Vector3 _innerMinBounds;

        private int _initialLargeAsteroidCount;
        private int _smallPerMedium;
        private int _mediumPerLarge;
        private DifficultySettings _difficultySetting;

        private void Start()
        {
            _difficultySetting = GameManager.sharedInstance.difficultySettings;
            _initialLargeAsteroidCount = _difficultySetting.initialLargeAsteroidCount;
            _smallPerMedium = _difficultySetting.numberOfSmallAsteroidsPer;
            _mediumPerLarge = _difficultySetting.numberOfMediumAsteroidsPer;
            
            DetermineAsteroidSpawnBoundValues();
            
            GameManager.sharedInstance.OnLevelStarted += HandleLevelStarted;
            GameManager.sharedInstance.OnAsteroidCollisionOccurred += HandleAsteroidCollision;
            GameManager.sharedInstance.OnGameOver += HandleGameOver;
            GameManager.sharedInstance.OnScreenSizeChange += DetermineAsteroidSpawnBoundValues;
        }

        private void HandleGameOver()
        {
            GameManager.sharedInstance.OnLevelStarted -= HandleLevelStarted;
            GameManager.sharedInstance.OnAsteroidCollisionOccurred -= HandleAsteroidCollision;
            GameManager.sharedInstance.OnGameOver -= HandleGameOver;
        }

        private void HandleLevelStarted(int level)
        {
            GameManager.sharedInstance.TotalExpectedSmallAsteroidsInLevel = (_initialLargeAsteroidCount + level) *
                                                             (_smallPerMedium +
                                                              _smallPerMedium);
            GameManager.sharedInstance.CountActualSmallAsteroidsDestroyedInLevel = 0;
            
            SetupAsteroidsFromPool(0, null, level);
        }

        private void HandleAsteroidCollision(Asteroid asteroidCollided)
        {
            if (asteroidCollided.AsteroidSize + 1 < 3)
            {
                SetupAsteroidsFromPool(asteroidCollided.AsteroidSize + 1, asteroidCollided);
            }
        }

        private void SetupAsteroidsFromPool(int asteroidOfSize, Asteroid asteroidCollided = null, int level = -1)
        {
            var asteroidCount = 0;
            string asteroidTag = null;
            
            if (asteroidCollided)
            {
                asteroidTag = asteroidCollided.tag;
            }
            
            if (asteroidOfSize == 0)
            {
                asteroidCount = level + _initialLargeAsteroidCount;
            }
            else if (asteroidOfSize == 1)
            {
                asteroidCount = _mediumPerLarge;
            }
            else if (asteroidOfSize == 2)
            {
                asteroidCount = _smallPerMedium;
            }
            
            for (var i = 0; i < asteroidCount ; i++)
            {
                Asteroid pooledAsteroid = AsteroidPool.SharedInstance.GetPooledObject(asteroidTag);
                
                if (pooledAsteroid != null)
                {
                    pooledAsteroid.AsteroidSize = asteroidOfSize;
                    pooledAsteroid.gameObject.transform.localScale = pooledAsteroid.Scales[asteroidOfSize] * Vector3.one;
                    pooledAsteroid.gameObject.name = $"{pooledAsteroid.Names[asteroidOfSize]} Asteroid {pooledAsteroid.tag}";
                    pooledAsteroid.gameObject.SetActive(true);
                    
                    SetAsteroidPosition(pooledAsteroid, asteroidCollided);
                }
            }
        }
        
        private void SetAsteroidPosition(Asteroid asteroid, Asteroid biggerAsteroid = null)
        {
            asteroid.transform.position = biggerAsteroid == null ? DetermineLargeAsteroidsPosition() : biggerAsteroid.transform.position;
        }
        
        private void DetermineAsteroidSpawnBoundValues()
        {
            _maxBounds = BoundManager.sharedInstance.MaxBounds;
            _minBounds = BoundManager.sharedInstance.MinBounds;

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
    }
}
