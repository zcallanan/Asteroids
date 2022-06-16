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
        private bool _ufoPresent;
        private float _ufoSpawnTimerLower;
        private float _ufoSpawnTimerUpper;
        private int _spawnUfoOfSize = 0;
        private int _levelToSpawnSmalls;
        private bool _isGameOver;
        private float _minX;
        private float _maxX;
        private float _minZ;
        private float _maxZ;
        private float _modifiedMinZ;
        private float _modifiedMaxZ;
        private Quaternion _currentRotation;
        private void Start()
        {
            _difficultySettings = GameManager.sharedInstance.DifficultySettings;
            _ufoPresent = _difficultySettings.ufoPresent;
            _ufoSpawnTimerLower = _difficultySettings.ufoSpawnTimerLower;
            _ufoSpawnTimerUpper = _difficultySettings.ufoSpawnTimerUpper;
            _levelToSpawnSmalls = _difficultySettings.levelToSpawnSmalls;
            DetermineUfoSpawnBounds();
            GameManager.sharedInstance.OnLevelStarted += HandleLevelChange;
            GameManager.sharedInstance.OnGameOver += HandleGameOver;
            GameManager.sharedInstance.OnScreenSizeChange += DetermineUfoSpawnBounds;
            
            if (_ufoPresent)
            {
                _currentSpawnCoroutine = StartCoroutine(RandomlySpawnUfoCoroutine());
            }
        }

        private void DetermineUfoSpawnBounds()
        {
            _minX = BoundManager.sharedInstance.MinX;
            _maxX = BoundManager.sharedInstance.MaxX;
            _minZ = BoundManager.sharedInstance.MinZ;
            _maxZ = BoundManager.sharedInstance.MaxZ;
            _modifiedMinZ = _minZ - (_minZ / 3);
            _modifiedMaxZ = _maxZ - (_maxZ / 3);
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
            obj.transform.localScale = pooledUfo.scales[_spawnUfoOfSize] * Vector3.one;
            obj.name = $"{pooledUfo.names[_spawnUfoOfSize]} UFO";
            pooledUfo.FromSide = Random.Range(0, 2);
            pooledUfo.UfoSize = _spawnUfoOfSize;
            var position = Vector3.zero;
            var rotation = Vector3.zero;
            if (pooledUfo.FromSide == 0)
            {
                position = new Vector3(_minX - 1, 1, Random.Range(_modifiedMinZ, _modifiedMaxZ));
                rotation = new Vector3(0, 90, 90);
            }
            else if (pooledUfo.FromSide == 1)
            {
                position = new Vector3(_maxX + 1, 1, Random.Range(_modifiedMinZ, _modifiedMaxZ));
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
                StartCoroutine(DisableRecentSpawningCoroutine(pooledUfo));
            }
        }

        private IEnumerator DisableRecentSpawningCoroutine(Ufo ufo)
        {
            yield return new WaitForSeconds(ufo.UfoEnforceBoundaryDelayLength);
            ufo.IsRecentlySpawned = false;
            ufo.UfoIsReadyToFireCoroutine = StartCoroutine(TriggerUfoToStartFiring(ufo));
        }

        private IEnumerator TriggerUfoToStartFiring(Ufo ufo)
        {
            if (ufo.isActiveAndEnabled)
            {
                GameManager.sharedInstance.UfoIsReadyToFire(ufo);
                yield return new WaitForSeconds(Random.Range(ufo.UfoProjectileCooldownLengthLower, ufo.UfoProjectileCooldownLengthUpper));
                ufo.UfoIsReadyToFireCoroutine = StartCoroutine(TriggerUfoToStartFiring(ufo));
            }
        }
    }
}
