using System;
using System.Collections;
using Data;
using Models;
using UnityEngine;

namespace Controllers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameData gameSettings;
        
        public static GameManager sharedInstance;
        
        [NonSerialized] public DifficultySettings difficultySettings;

        public int Score { get; set; }
        public int CurrentLives { get; private set; }
        public int CurrentDifficulty { get; set; }
        
        public int TotalLargeAsteroidsCollided { get; private set; }
        public int TotalMediumAsteroidsCollided { get; private set; }
        public int TotalSmallAsteroidsCollided { get; private set; }
        public int TotalExpectedSmallAsteroidsInLevel { get; set; }
        public int CountActualSmallAsteroidsDestroyedInLevel { get; set; }
        
        public float UfoXTargetOffsetLower { get; private set; }
        public float UfoXTargetOffsetUpper{ get; private set; }
        public float UfoZTargetOffsetLower { get; private set; }
        public float UfoZTargetOffsetUpper { get; private set; }
        
        public int CurrentGameType { get; set; }
        public int CurrentLevel { get; set; }
        
        private Vector2 _previousScreenSize;
        private Vector2 _latestScreenSize;
        private int _largeAsteroidScore;
        private int _mediumAsteroidScore;
        private int _smallAsteroidScore;
        private int _largeUfoScore;
        private int _smallUfoScore;
        private int _otherPlayerScore;
        private int _previousScore;
        private int _getALifeEveryTenK = 10000;
        
        public delegate void LevelStarted(int level);
        public event LevelStarted OnLevelStarted;
        public delegate void AsteroidCollisionOccurred(Asteroid collidedAsteroid);
        public event AsteroidCollisionOccurred OnAsteroidCollisionOccurred;
        public delegate void UfoCollisionOccurred(Ufo collidedUfo);
        public event UfoCollisionOccurred OnUfoCollisionOccurred;
        public delegate void HyperspaceTriggered(Player player);
        public event HyperspaceTriggered OnHyperspaceTrigger;
        public delegate void HyperspaceEnded(Player player);
        public event HyperspaceEnded OnHyperspaceEnd;
        public delegate void PlayerAppliedThrust(bool thrustIsActive, Player player);
        public event PlayerAppliedThrust OnPlayerAppliedThrust;
        public delegate void PlayerDied(Player player);
        public event PlayerDied OnPlayerDied;
        public delegate void PlayerSpawn(Player player);
        public event PlayerSpawn OnPlayerSpawn;
        public delegate void UfoShouldFireProjectile(Ufo ufo);
        public event UfoShouldFireProjectile OnUfoReadyToFire;
        public delegate void ScreenSizeChange();
        public event ScreenSizeChange OnScreenSizeChange;
        public delegate void ScoreUpdate(int score);
        public event ScoreUpdate OnScoreUpdate;
        public delegate void LivesChange(int livesCount);
        public event LivesChange OnDisplayedLivesShouldChange;
        public delegate void GameOver();
        public event GameOver OnGameOver;
        
        private void Awake()
        {
            sharedInstance = this;
            // At initialization, set game settings. User selection will override these values
            CurrentLives = gameSettings.initialPlayerLives;
            CurrentDifficulty = gameSettings.defaultDifficulty;
            difficultySettings = gameSettings.difficulties[CurrentDifficulty];
            
            UfoXTargetOffsetLower = difficultySettings.ufoXTargetOffsetLower;
            UfoXTargetOffsetUpper = difficultySettings.ufoXTargetOffsetUpper;
            UfoZTargetOffsetLower = difficultySettings.ufoZTargetOffsetLower;
            UfoZTargetOffsetUpper = difficultySettings.ufoZTargetOffsetUpper;
            
            _largeAsteroidScore = gameSettings.scoreLargeAsteroid;
            _mediumAsteroidScore = gameSettings.scoreMediumAsteroid;
            _smallAsteroidScore = gameSettings.scoreSmallAsteroid;
            _largeUfoScore = gameSettings.scoreLargeUfo;
            _smallUfoScore = gameSettings.scoreSmallUfo;
            _otherPlayerScore = gameSettings.scoreOtherPlayer;
            
            _previousScreenSize = new Vector2(Screen.width, Screen.height);
        }

        private void Start()
        {
            Screen.SetResolution(800, 600, false);
            // TODO: CurrentLevel is set from an event when the game actually starts
            OnAsteroidCollisionOccurred += UpdateScoreUponAsteroidDeath;
            OnUfoCollisionOccurred += UpdateScoreUponUfoDeath;
            StartCoroutine(LevelStartDelayCoroutine());
        }

        private void Update()
        {
            _latestScreenSize = new Vector2(Screen.width, Screen.height);
            if (_latestScreenSize != _previousScreenSize)
            {
                Debug.Log("screen size changed");
                ScreenSizeChanged();
            }
            _previousScreenSize = _latestScreenSize;
        }

        public void UfoIsReadyToFire(Ufo ufo)
        {
            OnUfoReadyToFire?.Invoke(ufo);
        }

        public void PlayerIsApplyingThrust(bool thrustIsActive, Player player)
        {
            OnPlayerAppliedThrust?.Invoke(thrustIsActive, player);
        }

        public void HyperspaceIsEnding(Player player)
        {
            OnHyperspaceEnd?.Invoke(player);
        }

        public void HyperspaceWasTriggered(Player player)
        {
            OnHyperspaceTrigger?.Invoke(player);
        }
        
        public void AsteroidCollided(Asteroid collidedAsteroid)
        {
            switch (collidedAsteroid.AsteroidSize)
            {
                case 0:
                    TotalLargeAsteroidsCollided++;
                    break;
                case 1:
                    TotalMediumAsteroidsCollided++;
                    break;
                case 2:
                {
                    TotalSmallAsteroidsCollided++;
                    CountActualSmallAsteroidsDestroyedInLevel++;
                    if (CountActualSmallAsteroidsDestroyedInLevel == TotalExpectedSmallAsteroidsInLevel)
                    {
                        CurrentLevel++;
                        StartCoroutine(LevelStartDelayCoroutine());
                    }
                    break;
                }
            }
            OnAsteroidCollisionOccurred?.Invoke(collidedAsteroid);
        }

        public void UfoCollided(Ufo collidedUfo)
        {
            OnUfoCollisionOccurred?.Invoke(collidedUfo);
        }

        public void PlayerCollided(Player player)
        {
            CurrentLives--;
            if (CurrentLives < 0)
            {
                GameEnded();
            }
            else
            {
                OnDisplayedLivesShouldChange?.Invoke(CurrentLives);
                OnPlayerDied?.Invoke(player);
            }
        }

        public void PlayerSpawned(Player player)
        {
            OnPlayerSpawn?.Invoke(player);
        }

        private void GameEnded()
        {
            if (CurrentLives < 0)
            {
                OnGameOver?.Invoke();
            }
        }

        private void ScoreUpdated(int score)
        {
            OnScoreUpdate?.Invoke(score);
            if (_previousScore < _getALifeEveryTenK && Score >= _getALifeEveryTenK)
            {
                _getALifeEveryTenK += 10000;
                CurrentLives++;
                OnDisplayedLivesShouldChange?.Invoke(CurrentLives);
            }
        }

        private void ScreenSizeChanged()
        {
            OnScreenSizeChange?.Invoke();
        }

        private void LevelStart(int currentLevel)
        {
            CurrentLevel = currentLevel;
            OnLevelStarted?.Invoke(currentLevel);
        }

        private void UpdateScoreUponAsteroidDeath(Asteroid asteroid)
        {
            _previousScore = Score;
            if (asteroid.AsteroidSize == 0)
            {
                Score += _largeAsteroidScore;
            } else if (asteroid.AsteroidSize == 1)
            {
                Score += _mediumAsteroidScore;
            }
            else
            {
                Score += _smallAsteroidScore;
            }
            ScoreUpdated(Score);
        }

        private void UpdateScoreUponUfoDeath(Ufo ufo)
        {
            _previousScore = Score;
            if (ufo.UfoSize == 0)
            {
                Score += _smallUfoScore;
            }
            else if (ufo.UfoSize == 1)
            {
                Score += _largeUfoScore;
            }
            
            ScoreUpdated(Score);
        }

        private IEnumerator LevelStartDelayCoroutine()
        {
            if (CurrentLevel == 0)
            {
                yield return new WaitForSeconds(.5f);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
            }
            
            LevelStart(CurrentLevel);
        }
    }
}
