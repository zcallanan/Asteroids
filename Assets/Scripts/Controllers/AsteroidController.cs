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
        private int _initialLargeAsteroidCount;
        private int _smallPerMedium;
        private int _mediumPerLarge;
        private DifficultySettings _difficultySetting;
        private static float _outerMaxX;
        private static float _innerMaxX;
        private static float _outerMinX;
        private static float _innerMinX;
        private static float _outerMaxZ;
        private static float _innerMaxZ;
        private static float _outerMinZ;
        private static float _innerMinZ;

        private void Start()
        {
            _difficultySetting = GameManager.sharedInstance.DifficultySettings;
            _initialLargeAsteroidCount = _difficultySetting.initialLargeAsteroidCount;
            _smallPerMedium = _difficultySetting.numberOfSmallAsteroidsPer;
            _mediumPerLarge = _difficultySetting.numberOfMediumAsteroidsPer;
            DetermineAsteroidSpawnBoundValues();
            GameManager.sharedInstance.OnLevelStarted += HandleLevelStarted;
            GameManager.sharedInstance.OnAsteroidCollisionOccurred += HandleAsteroidCollision;
            GameManager.sharedInstance.OnGameOver += HandleGameOver;
            GameManager.sharedInstance.OnScreenSizeChange += DetermineAsteroidSpawnBoundValues;
        }

        private void DetermineAsteroidSpawnBoundValues()
        {
            _outerMaxX = BoundManager.sharedInstance.MaxX;
            _innerMaxX = _outerMaxX - 1;
            _outerMinX = BoundManager.sharedInstance.MinX;
            _innerMinX = _outerMinX + 1;
            _outerMaxZ = BoundManager.sharedInstance.MaxZ;
            _innerMaxZ = _outerMaxZ - 1;
            _outerMinZ = BoundManager.sharedInstance.MinZ;
            _innerMinZ = _outerMinZ + 1;
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
            if (asteroidCollided.asteroidSize + 1 < 3)
            {
                SetupAsteroidsFromPool(asteroidCollided.asteroidSize + 1, asteroidCollided);
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
                    pooledAsteroid.asteroidSize = asteroidOfSize;
                    pooledAsteroid.gameObject.transform.localScale = pooledAsteroid.scales[asteroidOfSize] * Vector3.one;
                    pooledAsteroid.gameObject.name = $"{pooledAsteroid.names[asteroidOfSize]} Asteroid {pooledAsteroid.tag}";
                    pooledAsteroid.gameObject.SetActive(true);
                    SetAsteroidPosition(pooledAsteroid, asteroidCollided);
                }
            }
        }
        
        private void SetAsteroidPosition(Asteroid asteroid, Asteroid biggerAsteroid = null)
        {
            asteroid.transform.position = biggerAsteroid == null ? DetermineLargeAsteroidsPosition() : biggerAsteroid.transform.position;
        }

        private Vector3 DetermineLargeAsteroidsPosition()
        {
            var side = Random.Range(0, 4);
            var positionResult = side switch
            {
                0 => new Vector3(Random.Range(_outerMinX, _outerMaxX), 1.0f, Random.Range(_innerMaxZ, _outerMaxZ)), // Top
                1 => new Vector3(Random.Range(_innerMaxX, _outerMaxX), 1.0f, Random.Range(_outerMinZ, _outerMaxZ)), // Right
                2 => new Vector3(Random.Range(_outerMinX, _outerMaxX), 1.0f, Random.Range(_innerMinZ, _outerMinZ)), // Bottom
                3 => new Vector3(Random.Range(_innerMinX, _outerMinX), 1.0f, Random.Range(_outerMinZ, _outerMaxZ)), // Left
                _ => throw new ArgumentOutOfRangeException(nameof(side), $"Not expected side value: {side}"),
            };
            return positionResult;
        }
    }
}
