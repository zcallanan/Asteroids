using System;
using Installers;
using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class GameLevelHandler : IInitializable, ITickable
    {

        private readonly GameState _gameState;
        private readonly Settings _settings;
        private readonly Difficulty.Settings _difficultySettings;

        private int _gameDifficulty;
        private int _initLargeAsteroids;
        private int _smallPerMedium;
        
        private int _countSmallAsteroidsDestroyedInLevel;
        private int _totalExpectedSmallAsteroidsInLevel;

        private bool _isReadyToStartNewLevel;
        private float _whenLastSmallAsteroidWasKilled;

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
            _gameDifficulty = _gameState.GameDifficulty;
            _initLargeAsteroids = _difficultySettings.difficulties[_gameDifficulty].initLargeAsteroids;
            _smallPerMedium = _difficultySettings.difficulties[_gameDifficulty].smallPerMedium;
        }

        public void Tick()
        {
            if (_isReadyToStartNewLevel &&
                Time.realtimeSinceStartup - _whenLastSmallAsteroidWasKilled >= _settings.levelStartDelay)
            {
                _gameState.CurrentLevel.Value++;
                _countSmallAsteroidsDestroyedInLevel = 0;

                _totalExpectedSmallAsteroidsInLevel =
                    (_initLargeAsteroids + _gameState.CurrentLevel.Value) * (_smallPerMedium + _smallPerMedium);
                
                _isReadyToStartNewLevel = false;
            }
        }
        
        public void RegisterSmallDeathToDetermineNextLevel()
        {
            _countSmallAsteroidsDestroyedInLevel++;

            if (_countSmallAsteroidsDestroyedInLevel == _totalExpectedSmallAsteroidsInLevel)
            {
                _whenLastSmallAsteroidWasKilled = Time.realtimeSinceStartup;
                
                _isReadyToStartNewLevel = true;
            }
        }

        [Serializable]
        public class Settings
        {
            public float levelStartDelay;
        }
    }
}

