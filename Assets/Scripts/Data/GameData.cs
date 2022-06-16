using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu (fileName = "GameSettings", menuName = "Settings", order = 1)]
    public class GameData : ScriptableObject
    {
        public int initialPlayerLives;
        public int defaultDifficulty;
        public int scoreLargeAsteroid;
        public int scoreMediumAsteroid;
        public int scoreSmallAsteroid;
        public int scoreOtherPlayer;
        public int scoreLargeUfo;
        public int scoreSmallUfo;
        public List<DifficultySettings> difficulties = new List<DifficultySettings>();
        public List<GameTypes> gameTypes = new List<GameTypes>();
    }
    [System.Serializable]
    public struct DifficultySettings
    {
        public string name;
        public float asteroidLowerSpeed;
        public float asteroidUpperSpeed;
        public int initialLargeAsteroidCount;
        public int numberOfSmallAsteroidsPer;
        public int numberOfMediumAsteroidsPer;
        public bool ufoPresent;
        public float ufoXTargetOffsetLower;
        public float ufoXTargetOffsetUpper;
        public float ufoZTargetOffsetLower;
        public float ufoZTargetOffsetUpper; 
        public float ufoLowerSpeed;
        public float ufoUpperSpeed;
        public float ufoSpawnTimerLower;
        public float ufoSpawnTimerUpper;
        public float ufoFireTowardsPlayerFrequency;
        public int levelToSpawnSmalls;
    }

    [System.Serializable]
    public struct GameTypes
    {
        public string name;
        public int playerCount;
    }
}