using System;
using System.Collections.Generic;

namespace AsteroidGame.Misc
{
    public class Difficulty
    {
        [Serializable]
        public class Settings
        {
            public List<Fields> difficulties = new List<Fields>();
        }
    
        [Serializable]
        public class Fields
        {
            public int smallPerMedium;
            public int mediumPerLarge;
            public int initLargeAsteroids;
            public float astMinSpeed;
            public float astMaxSpeed;
            
            public bool isUfoSpawnableInThisDifficulty;
            public int smallUfoLevelToSpawn;
            public float ufoMinSpeed;
            public float ufoMaxSpeed;
            public float ufoMinSpawnDelay;
            public float ufoMaxSpawnDelay;
            public float ufoMinFireDelay;
            public float ufoMaxFireDelay;

            public float ufoOffsetConstant;
            public float ufoOffsetMin;
            public float ufoOffsetMax;
        }
    }
}
