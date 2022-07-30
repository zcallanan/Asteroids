using System;
using UnityEngine;

namespace AsteroidGame.Misc
{
    public class ViewData
    {
        [Serializable]
        public class Settings
        {
            public Vector2 playerLivesViewPos;
            public Vector2 playerSizeDelta;
            public Vector2 otherPlayerLivesViewPos;
            public Vector2 otherPlayerSizeDelta;
        }
    }
}
