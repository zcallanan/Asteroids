using System;
using System.Collections.Generic;

namespace Misc
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
        }
    }
}
