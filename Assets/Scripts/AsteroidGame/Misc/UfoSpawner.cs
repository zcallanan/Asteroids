using System;
using AsteroidGame.UfoScripts;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace AsteroidGame.Misc
{
    public class UfoSpawner : IInitializable
    {
        private readonly Ufo.Factory _ufoFactory;
        private readonly GameState _gameState;
        private readonly Settings _settings;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly BoundHandler _boundHandler;
        
        private int _smallUfoLevelToSpawn;
        private float _ufoMinSpawnDelay;
        private float _ufoMaxSpawnDelay;

        private Vector3 _minBounds;
        private Vector3 _maxBounds;

        private float _modifiedMinZ;
        private float _modifiedMaxZ;

        private Quaternion _currentRotation;

        private IDisposable _ufoSpawnTimer;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UfoSpawner(
            Ufo.Factory ufoFactory,
            GameState gameState,
            Settings settings,
            Difficulty.Settings difficultySettings,
            BoundHandler boundHandler)
        {
            _ufoFactory = ufoFactory;
            _gameState = gameState;
            _settings = settings;
            _difficultySettings = difficultySettings;
            _boundHandler = boundHandler;
        }

        public void Initialize()
        {
            UpdateUfoSpawnBounds();
            
            OnlySpawnUfoDuringTheGame();

            DisposeIfGameNotRunning();
        }

        private void OnlySpawnUfoDuringTheGame()
        {
            _gameState.IsUfoSpawning
                .Subscribe(isUfoSpawning =>
                {
                    if (isUfoSpawning)
                    {
                        var difficulties = _difficultySettings.difficulties[_gameState.GameDifficulty.Value];
            
                        var isUfoSpawnableInThisDifficulty = difficulties.isUfoSpawnableInThisDifficulty;

                        if (!isUfoSpawnableInThisDifficulty)
                        {
                            return;
                        }
            
                        _smallUfoLevelToSpawn = difficulties.smallUfoLevelToSpawn;

                        _ufoMinSpawnDelay = difficulties.ufoMinSpawnDelay;
                        _ufoMaxSpawnDelay = difficulties.ufoMaxSpawnDelay;
                        
                        DelayForATimeThenSpawnUfo(ObjectTypes.LargeUfo);

                        TrackLevelChangeToSwitchToSmallUfo();
                    }
                })
                .AddTo(_disposables);
        }

        private void TrackLevelChangeToSwitchToSmallUfo()
        {
            _gameState.CurrentLevel
                .Subscribe(SpawnSmallUfoAtLevel)
                .AddTo(_disposables);
        }

        private void UpdateUfoSpawnBounds()
        {
            _boundHandler.MinBounds
                .Subscribe(min =>
                {
                    _minBounds = min;
                    _modifiedMinZ = _minBounds.z - (_minBounds.z / 4);
                })
                .AddTo(_disposables);
            
            _boundHandler.MaxBounds
                .Subscribe(max =>
                {
                    _maxBounds = max;
                    _modifiedMaxZ = _maxBounds.z - (_maxBounds.z / 4);
                })
                .AddTo(_disposables);
        }

        private void SpawnSmallUfoAtLevel(int level)
        {
            if (level == _smallUfoLevelToSpawn)
            {
                _ufoSpawnTimer?.Dispose();
                DelayForATimeThenSpawnUfo(ObjectTypes.SmallUfo);
            }
        }

        private void DelayForATimeThenSpawnUfo(ObjectTypes size)
        {
            _ufoSpawnTimer = Observable
                .Timer(TimeSpan.FromSeconds(Random.Range(_ufoMinSpawnDelay, _ufoMaxSpawnDelay)))
                .Subscribe(_ =>
                {
                    if (_gameState.IsGameRunning.Value)
                    {
                        SpawnUfo(size);
                    }
                })
                .AddTo(_disposables);
        }

        private void SpawnUfo(ObjectTypes size)
        {
            var ufo = _ufoFactory.Create(size);

            ufo.Size = size;
            ufo.IsRecentlySpawned = true;
            ufo.IsDead.Value = false;
            
            ScaleUfo(ufo);
            PositionUfo(ufo);
            
            ToggleUfoRecentlySpawnedOnceItsInBounds(ufo);
            
            DelayForATimeThenSpawnUfo(size);
        }

        private void ScaleUfo(Ufo ufo)
        {
            if (ufo.Size == ObjectTypes.LargeUfo)
            {
                ufo.transform.localScale = _settings.largeScale * Vector3.one;
            }
            else if (ufo.Size == ObjectTypes.SmallUfo)
            {
                ufo.transform.localScale = _settings.smallScale * Vector3.one;
            }
        }

        private void PositionUfo(Ufo ufo)
        {
            var ufoSpawnsOnLeft = Random.value >= 0.5;
            
            Vector3 position;
            Vector3 rotation;
            
            if (ufoSpawnsOnLeft)
            {
                position = new Vector3(_minBounds.x - 1, 1, Random.Range(_modifiedMinZ, _modifiedMaxZ));
                rotation = new Vector3(0, 90, 90);

                ufo.name = $"{ufo.Size} from Left";
            }
            else
            {
                position = new Vector3(_maxBounds.x + 1, 1, Random.Range(_modifiedMinZ, _modifiedMaxZ)); 
                rotation = new Vector3(0, 270, 270);
                 
                ufo.name = $"{ufo.Size} from Right";
            }

            var transform = ufo.transform;
            
            transform.position = position;
            _currentRotation.eulerAngles = rotation;

            transform.rotation = _currentRotation;
            ufo.Facing = transform.forward;
        }

        private void ToggleUfoRecentlySpawnedOnceItsInBounds(Ufo ufo)
        {
            Observable.Timer(TimeSpan.FromSeconds(1))
                .Subscribe(_ => ufo.IsRecentlySpawned = false)
                .AddTo(_disposables);
        }

        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables?.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float largeScale;
            public float smallScale;
        }
    }
}
