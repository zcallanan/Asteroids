// using System;
// using System.Collections;
// using Data;
// using UniRx;
// using UnityEngine;
//
// namespace Controllers
// {
//     public class GameManager : MonoBehaviour
//     {
//         [SerializeField] private GameData gameSettings;
//
//         public static GameManager sharedInstance;
//         
//         [NonSerialized] public DifficultySettings difficultySettings;
//         
//         public ReactiveProperty<int> Score { get; private set; }
//         public ReactiveProperty<int> CurrentLives { get; private set; }
//         public ReactiveProperty<bool> IsGameOver { get; private set; }
//         
//         public ReactiveProperty<int> TotalExpectedSmallAsteroidsInLevel { get; private set; }
//         public ReactiveProperty<int> CountActualSmallAsteroidsDestroyedInLevel { get; private set; }
//         
//         public ReactiveProperty<int> CurrentLevel { get; private set; }
//         public int CurrentDifficulty { get; set; }
//         public int CurrentGameType { get; set; }
//         
//         public int TotalLargeAsteroidsCollided { get; private set; }
//         public int TotalMediumAsteroidsCollided { get; private set; }
//         public int TotalSmallAsteroidsCollided { get; private set; }
//         
//         public Vector3 UfoOffsetMin { get; private set; }
//         public Vector3 UfoOffsetMax { get; private set; }
//
//         public ReactiveProperty<Vector2> LatestScreenSize { get; private set; }
//         
//         private int _getALifeEveryTenK = 10000;
//         private int _largeAsteroidScore;
//         private int _mediumAsteroidScore;
//         private int _smallAsteroidScore;
//         private int _largeUfoScore;
//         private int _smallUfoScore;
//         private int _otherPlayerScore;
//         private int _previousScore;
//
//         private void Awake()
//         {
//             sharedInstance = this;
//             
//             CurrentLives = new ReactiveProperty<int>(gameSettings.initialPlayerLives);
//             CurrentLevel = new ReactiveProperty<int>(0);
//             IsGameOver = new ReactiveProperty<bool>(false);
//             Score = new ReactiveProperty<int>(0);
//             
//             TotalExpectedSmallAsteroidsInLevel = new ReactiveProperty<int>(0);
//             CountActualSmallAsteroidsDestroyedInLevel = new ReactiveProperty<int>(0);
//             LatestScreenSize = new ReactiveProperty<Vector2>(new Vector2(Screen.width, Screen.height));
//             
//             CurrentDifficulty = gameSettings.defaultDifficulty;
//             difficultySettings = gameSettings.difficulties[CurrentDifficulty];
//
//             UfoOffsetMin = new Vector3(difficultySettings.ufoXTargetOffsetLower, 1, difficultySettings.ufoZTargetOffsetLower);
//             UfoOffsetMax = new Vector3(difficultySettings.ufoXTargetOffsetUpper, 1, difficultySettings.ufoZTargetOffsetUpper);
//
//             _largeAsteroidScore = gameSettings.scoreLargeAsteroid;
//             _mediumAsteroidScore = gameSettings.scoreMediumAsteroid;
//             _smallAsteroidScore = gameSettings.scoreSmallAsteroid;
//             _largeUfoScore = gameSettings.scoreLargeUfo;
//             _smallUfoScore = gameSettings.scoreSmallUfo;
//             _otherPlayerScore = gameSettings.scoreOtherPlayer;
//         }
//
//         private void Start()
//         {
//             Screen.SetResolution(800, 600, false);
//             // TODO: CurrentLevel is set from an event when the game actually starts
//             StartCoroutine(LevelStartDelayCoroutine(CurrentLevel.Value));
//         }
//
//         public void DecrementLivesAndCheckForGameOver()
//         {
//             CurrentLives.Value--;
//             if (CurrentLives.Value < 0)
//             {
//                 IsGameOver.Value = true;
//             }
//         }
//         
//         public void DetermineWhenToStartNewLevel(int asteroidSize)
//         {
//             switch (asteroidSize)
//             {
//                 case 0:
//                     TotalLargeAsteroidsCollided++;
//                     break;
//                 case 1:
//                     TotalMediumAsteroidsCollided++;
//                     break;
//                 case 2:
//                 {
//                     TotalSmallAsteroidsCollided++;
//                     CountActualSmallAsteroidsDestroyedInLevel.Value++;
//
//                     if (CountActualSmallAsteroidsDestroyedInLevel.Value == TotalExpectedSmallAsteroidsInLevel.Value)
//                     {
//                         StartCoroutine(LevelStartDelayCoroutine(CurrentLevel.Value + 1));
//                     }
//                     
//                     break;
//                 }
//             }
//         }
//         
//         public void UpdateScoreUponAsteroidDeath(int asteroidSize)
//         {
//             _previousScore = Score.Value;
//             if (asteroidSize == 0)
//             {
//                 Score.Value += _largeAsteroidScore;
//             } else if (asteroidSize == 1)
//             {
//                 Score.Value += _mediumAsteroidScore;
//             }
//             else
//             {
//                 Score.Value += _smallAsteroidScore;
//             }
//             CheckIfLivesShouldIncrease(Score.Value);
//         }
//         
//         public void UpdateScoreUponUfoDeath(int ufoSize)
//         {
//             _previousScore = Score.Value;
//             if (ufoSize == 0)
//             {
//                 Score.Value += _smallUfoScore;
//             }
//             else if (ufoSize == 1)
//             {
//                 Score.Value += _largeUfoScore;
//             }
//             
//             CheckIfLivesShouldIncrease(Score.Value);
//         }
//         
//         private void CheckIfLivesShouldIncrease(int score)
//         {
//             if (_previousScore < _getALifeEveryTenK && Score.Value >= _getALifeEveryTenK)
//             {
//                 _getALifeEveryTenK += 10000;
//                 CurrentLives.Value++;
//             }
//         }
//         
//         private IEnumerator LevelStartDelayCoroutine(int level)
//         {
//             if (level == 0)
//             {
//                 yield return new WaitForSeconds(1f);
//             }
//             else
//             {
//                 yield return new WaitForSeconds(1.5f);
//             }
//
//             CurrentLevel.Value = level;
//         }
//     }
// }
