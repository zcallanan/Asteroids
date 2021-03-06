using System;
using ProjectScripts;
using UniRx;
using Zenject;

namespace AsteroidGame.Misc
{
    public class GameLevelHandler : IInitializable
    {

        private readonly GameState _gameState;
        private readonly Settings _settings;
        private readonly Difficulty.Settings _difficultySettings;

        private int _initLargeAsteroids;
        private int _smallPerMedium;
        
        private int _countSmallAsteroidsDestroyedInLevel;
        private int _totalExpectedSmallAsteroidsInLevel;

        private bool _isReadyToStartNewLevel;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public GameLevelHandler(
            Settings settings,
            Difficulty.Settings difficultySettings,
            GameState gameState)
        {
            _settings = settings;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
        }

        public void Initialize()
        {
            var gameDifficulty = _gameState.GameDifficulty;
            
            _initLargeAsteroids = _difficultySettings.difficulties[gameDifficulty.Value].initLargeAsteroids;
            _smallPerMedium = _difficultySettings.difficulties[gameDifficulty.Value].smallPerMedium;

            DetermineTotalSmallAsteroidsInThisLevel();

            DisposeIfGameNotRunning();
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

        private void DetermineTotalSmallAsteroidsInThisLevel()
        {
            _totalExpectedSmallAsteroidsInLevel =
                (_initLargeAsteroids + _gameState.CurrentLevel.Value) * (_smallPerMedium + _smallPerMedium);
        }
        
        public void RegisterSmallDeathToDetermineNextLevel()
        {
            _countSmallAsteroidsDestroyedInLevel++;

            if (_countSmallAsteroidsDestroyedInLevel == _totalExpectedSmallAsteroidsInLevel)
            {
                _isReadyToStartNewLevel = true;

                StartNewLevelAfterDelay();
            }
        }

        private void StartNewLevelAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.levelStartDelay))
                .Subscribe(_ => 
                { 
                    if (_isReadyToStartNewLevel)
                    {
                        _gameState.CurrentLevel.Value++;
                        _countSmallAsteroidsDestroyedInLevel = 0;

                        DetermineTotalSmallAsteroidsInThisLevel();

                        _isReadyToStartNewLevel = false;
                    } 
                })
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float levelStartDelay;
        }
    }
}

