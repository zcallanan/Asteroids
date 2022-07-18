using AsteroidGame.Misc;
using AsteroidGame.PlayerScripts;
using AsteroidGame.UfoScripts;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.AsteroidScripts
{
    public class AsteroidCollisionHandler : IInitializable
    {
        private readonly Asteroid _asteroid;
        private readonly ScoreHandler _scoreHandler;
        private readonly AsteroidSpawner _asteroidSpawner;
        private readonly GameLevelHandler _gameLevelHandler;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;

        private ObjectTypes _large;
        private ObjectTypes _medium;
        private ObjectTypes _small;
        
        private int _mediumPerLarge;
        private int _smallPerMedium;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public AsteroidCollisionHandler(
            Asteroid asteroid,
            ScoreHandler scoreHandler,
            AsteroidSpawner asteroidSpawner,
            GameLevelHandler gameLevelHandler,
            Difficulty.Settings difficultySettings,
            GameState gameState)
        {
            _asteroid = asteroid;
            _scoreHandler = scoreHandler;
            _asteroidSpawner = asteroidSpawner;
            _gameLevelHandler = gameLevelHandler;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                var gameDifficulty = _gameState.GameDifficulty;
                _mediumPerLarge = _difficultySettings.difficulties[gameDifficulty.Value].mediumPerLarge;
                _smallPerMedium = _difficultySettings.difficulties[gameDifficulty.Value].smallPerMedium;

                _large = ObjectTypes.LargeAsteroid;
                _medium = ObjectTypes.MediumAsteroid;
                _small = ObjectTypes.SmallAsteroid;

                HandleCollisionOnTriggerEnter();

                DisposeIfGameNotRunning();
            }
        }
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void HandleCollision(Collider collider)
        {
            if (_asteroid.Size == _small)
            {
                _gameLevelHandler.RegisterSmallDeathToDetermineNextLevel();
            }
            else if (_asteroid.Size == _medium)
            {
                for (int i = 0; i < _smallPerMedium; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(_asteroid.RenderValue, _small, _asteroid.transform.position);
                }
            }
            else if (_asteroid.Size == _large)
            {
                for (int i = 0; i < _mediumPerLarge; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(_asteroid.RenderValue, _medium, _asteroid.transform.position);
                }
            }
            
            _scoreHandler.UpdateScore(_asteroid.Size, collider);
        }

        private void HandleCollisionOnTriggerEnter()
        {
            _asteroid
                .OnTriggerEnterAsObservable()
                .Subscribe(HandleCollision)
                .AddTo(_disposables);
        }
    }
}
