using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class GameLevelHandler : ITickable
    {
        public ReactiveProperty<int> CurrentLevel { get; set; }

        public int CountSmallAsteroidsDestroyedInLevel { get; set; }
        public int TotalExpectedSmallAsteroidsInLevel { get; set; }

        private readonly Settings _settings;
        
        private bool _isReadyToStartNewLevel;
        private float _whenLastSmallAsteroidWasKilled;

        public GameLevelHandler(Settings settings)
        {
            _settings = settings;
        }

        public void RegisterSmallDeathToDetermineNextLevel()
        {
            
            CountSmallAsteroidsDestroyedInLevel++;

            if (CountSmallAsteroidsDestroyedInLevel == TotalExpectedSmallAsteroidsInLevel)
            {
                // StartCoroutine(LevelStartDelayCoroutine(CurrentLevel.Value + 1));
                _whenLastSmallAsteroidWasKilled = Time.realtimeSinceStartup;
                
                _isReadyToStartNewLevel = true;
            }
            
        }

        public void Tick()
        {
            if (_isReadyToStartNewLevel &&
                Time.realtimeSinceStartup - _whenLastSmallAsteroidWasKilled >= _settings.levelStartDelay)
            {
                CurrentLevel.Value++;
                
                _isReadyToStartNewLevel = false;
            }
        }

        [Serializable]
        public class Settings
        {
            public float levelStartDelay;
        }
    }
}

