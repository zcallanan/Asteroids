// using System;
// using Data;
// using Misc;
// using Models;
// using Pools;
// using UniRx;
// using UniRx.Triggers;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace Controllers
// {
//     public class AsteroidController : MonoBehaviour
//     {
//         public static AsteroidController sharedInstance;
//         private Vector3 _maxBounds;
//         private Vector3 _minBounds;
//         private Vector3 _innerMaxBounds;
//         private Vector3 _innerMinBounds;
//
//         private int _initialLargeAsteroidCount;
//         private int _smallPerMedium;
//         private int _mediumPerLarge;
//         private DifficultySettings _difficultySetting;
//         
//         private readonly CompositeDisposable _disposables = new CompositeDisposable();
//
//         private void Start()
//         {
//             sharedInstance = this;
//             
//             _difficultySetting = GameManager.sharedInstance.difficultySettings;
//             _initialLargeAsteroidCount = _difficultySetting.initialLargeAsteroidCount;
//             _smallPerMedium = _difficultySetting.numberOfSmallAsteroidsPer;
//             _mediumPerLarge = _difficultySetting.numberOfMediumAsteroidsPer;
//             
//             DetermineAsteroidSpawnBoundValues();
//
//             GameManager.sharedInstance.IsGameOver
//                 .Subscribe(HandleGameOver)
//                 .AddTo(_disposables);
//             
//             GameManager.sharedInstance.CurrentLevel
//                 .Throttle(TimeSpan.FromSeconds(.5))
//                 .Subscribe(HandleLevelStarted)
//                 .AddTo(_disposables);
//             
//             GameManager.sharedInstance.LatestScreenSize
//                 .Subscribe(unit => DetermineAsteroidSpawnBoundValues())
//                 .AddTo(_disposables);
//         }
//
//         public void SetupAsteroidsFromPool(int asteroidOfSize, Models.Asteroid asteroidCollided = null, int level = -1)
//         {
//             var asteroidCount = 0;
//             string asteroidTag = null;
//             
//             if (asteroidCollided)
//             {
//                 asteroidTag = asteroidCollided.tag;
//             }
//             
//             if (asteroidOfSize == 0)
//             {
//                 asteroidCount = level + _initialLargeAsteroidCount;
//             }
//             else if (asteroidOfSize == 1)
//             {
//                 asteroidCount = _mediumPerLarge;
//             }
//             else if (asteroidOfSize == 2)
//             {
//                 asteroidCount = _smallPerMedium;
//             }
//             
//             for (var i = 0; i < asteroidCount ; i++)
//             {
//                 Models.Asteroid pooledAsteroid = AsteroidPool.SharedInstance.GetPooledObject(asteroidTag);
//                 
//                 if (pooledAsteroid != null)
//                 {
//                     pooledAsteroid.AsteroidSize = asteroidOfSize;
//                     pooledAsteroid.gameObject.transform.localScale = pooledAsteroid.Scales[asteroidOfSize] * Vector3.one;
//                     pooledAsteroid.gameObject.name = $"{pooledAsteroid.Names[asteroidOfSize]} Asteroid {pooledAsteroid.tag}";
//                     pooledAsteroid.gameObject.SetActive(true);
//                     
//                     SetAsteroidPosition(pooledAsteroid, asteroidCollided);
//                 }
//             }
//         }
//         
//         private void HandleGameOver(bool isGameOver)
//         {
//             if (isGameOver)
//             {
//                 _disposables.Clear();
//             }
//         }
//
//         private void HandleLevelStarted(int level)
//         {
//             GameManager.sharedInstance.TotalExpectedSmallAsteroidsInLevel.Value =
//                 (_initialLargeAsteroidCount + level) * (_smallPerMedium + _smallPerMedium);
//             GameManager.sharedInstance.CountActualSmallAsteroidsDestroyedInLevel.Value = 0;
//             
//             SetupAsteroidsFromPool(0, null, level);
//         }
//         
//         private void SetAsteroidPosition(Models.Asteroid asteroid, Models.Asteroid biggerAsteroid = null)
//         {
//             asteroid.transform.position = biggerAsteroid == null ? DetermineLargeAsteroidsPosition() : biggerAsteroid.transform.position;
//         }
//         
//         private void DetermineAsteroidSpawnBoundValues()
//         {
//             // _maxBounds = BoundManager.sharedInstance.MaxBounds;
//             // _minBounds = BoundManager.sharedInstance.MinBounds;
//
//             _innerMaxBounds = new Vector3(_maxBounds.x - 1, 1, _maxBounds.z - 1);
//             _innerMinBounds = new Vector3(_minBounds.x + 1, 1, _minBounds.z + 1);
//         }
//
//         private Vector3 DetermineLargeAsteroidsPosition()
//         {
//             var side = Random.Range(0, 4);
//             var positionResult = side switch
//             {
//                 0 => new Vector3(Random.Range(_minBounds.x, _maxBounds.x), 1.0f, Random.Range(_innerMaxBounds.z, _maxBounds.z)), // Top
//                 1 => new Vector3(Random.Range(_innerMaxBounds.x, _maxBounds.x), 1.0f, Random.Range(_minBounds.z, _maxBounds.z)), // Right
//                 2 => new Vector3(Random.Range(_minBounds.x, _maxBounds.x), 1.0f, Random.Range(_innerMinBounds.z, _minBounds.z)), // Bottom
//                 3 => new Vector3(Random.Range(_innerMinBounds.x, _minBounds.x), 1.0f, Random.Range(_minBounds.z, _maxBounds.z)), // Left
//                 _ => throw new ArgumentOutOfRangeException(nameof(side), $"Not expected side value: {side}"),
//             };
//             return positionResult;
//         }
//     }
// }
