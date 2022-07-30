using System;
using UnityEngine;

namespace AsteroidGame.Misc
{
    public class ViewData
    {
        [Serializable]
        public class Settings
        {
            public Vector2 p1LivesViewPos;
            public Vector2 p1LivesSizeDelta;
            public Vector2 p2LivesViewPos;
            public Vector2 p2LivesSizeDelta;
            
            public Vector2 p1ScoreViewPos;
            public Vector2 p1ScoreSizeDelta;
            public Vector2 p2ScoreViewPos;
            public Vector2 p2ScoreSizeDelta;

            public Color p2Color;
        }
    }
}
