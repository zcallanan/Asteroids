using System;
using UnityEngine;

namespace AsteroidGame.Misc
{
    public class PlayerData
    {
        [Serializable]
        public class Settings
        {
            public Material playerMat;
            public Material otherPlayerMat;
            public float respawnDelay;
        }
    }
}
