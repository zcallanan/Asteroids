using System.Collections;
using Data;
using Models;
using Pools;
using UnityEngine;

namespace Controllers
{
    public class UfoController : MonoBehaviour
    {
        private Coroutine _currentSpawnCoroutine;
        private DifficultySettings _difficultySettings;
        
        private float _ufoSpawnTimerLower;
        private float _ufoSpawnTimerUpper;
        private int _spawnUfoOfSize;
        private int _levelToSpawnSmalls;
        
        private bool _ufoPresent;
        private bool _isGameOver;
        
        private Vector3 _maxBounds;
        private Vector3 _minBounds;
        private float _modifiedMinZ;
        private float _modifiedMaxZ;
        
        private Quaternion _currentRotation;
        private void Start()
        {
            if (_ufoPresent)
            {
                _currentSpawnCoroutine = StartCoroutine(RandomlySpawnUfoCoroutine());
            }
            else
            {
                enabled = false;
            }
            
            _difficultySettings = GameManager.sharedInstance.difficultySettings;
            
            _ufoPresent = _difficultySettings.ufoPresent;
            _ufoSpawnTimerLower = _difficultySettings.ufoSpawnTimerLower;
            _ufoSpawnTimerUpper = _difficultySettings.ufoSpawnTimerUpper;
            _levelToSpawnSmalls = _difficultySettings.levelToSpawnSmalls;
            
            DetermineUfoSpawnBounds();
            
            GameManager.sharedInstance.OnLevelStarted += HandleLevelChange;
            GameManager.sharedInstance.OnGameOver += HandleGameOver;
            GameManager.sharedInstance.OnScreenSizeChange += DetermineUfoSpawnBounds;
            
            
        }

        private void DetermineUfoSpawnBounds()
        {
            _maxBounds = BoundManager.sharedInstance.MaxBounds;
            _minBounds = BoundManager.sharedInstance.MinBounds;
            
            _modifiedMinZ = _minBounds.z - (_minBounds.z / 3);
            _modifiedMaxZ = _maxBounds.z - (_maxBounds.z / 3);
        }

        private void HandleGameOver()
        {
            _isGameOver = true;
            StopCoroutine(_currentSpawnCoroutine);
        }

        private void HandleLevelChange(int currentLevel)
        {
            if (currentLevel > _levelToSpawnSmalls && _spawnUfoOfSize == 0)
            {
                _spawnUfoOfSize = 1;
            }

            _ufoSpawnTimerLower--;
            _ufoSpawnTimerUpper--;
        }

        private void UfoSetup(Ufo pooledUfo)
        {
            pooledUfo.IsRecentlySpawned = true;
            var obj = pooledUfo.gameObject;
            obj.SetActive(true);
            obj.transform.localScale = pooledUfo.Scales[_spawnUfoOfSize] * Vector3.one;
            obj.name = $"{pooledUfo.Names[_spawnUfoOfSize]} UFO";
            pooledUfo.UfoSpawnsFromSide = Random.Range(0, 2);
            pooledUfo.UfoSize = _spawnUfoOfSize;
            var position = Vector3.zero;
            var rotation = Vector3.zero;
            if (pooledUfo.UfoSpawnsFromSide == 0)
            {
                position = new Vector3(_minBounds.x - 1, 1, Random.Range(_modifiedMinZ, _modifiedMaxZ));
                rotation = new Vector3(0, 90, 90);
            }
            else if (pooledUfo.UfoSpawnsFromSide == 1)
            {
                position = new Vector3(_maxBounds.x + 1, 1, Random.Range(_modifiedMinZ, _modifiedMaxZ));
                rotation = new Vector3(0, 270, 270);
            }

            obj.transform.position = position;
            _currentRotation.eulerAngles = rotation;
            obj.transform.rotation = _currentRotation;
            pooledUfo.DirectionToFace = obj.transform.forward;
        }

        private IEnumerator RandomlySpawnUfoCoroutine()
        {
            if (!_isGameOver)
            {
                var randomSpawnTimer = Random.Range(_ufoSpawnTimerLower, _ufoSpawnTimerUpper);
                yield return new WaitForSeconds(randomSpawnTimer);
                var pooledUfo = UfoPool.SharedInstance.GetPooledObject();
                UfoSetup(pooledUfo);
                _currentSpawnCoroutine = StartCoroutine(RandomlySpawnUfoCoroutine());
            }
        }
    }
}
