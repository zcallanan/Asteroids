using System;
using System.Collections.Generic;
using UnityEngine;

namespace StartMenuScripts.StartMenu
{
    public class StartMenuData
    {
        [Serializable]
        public class Settings
        {
            [Range(0f, 5f)] public float titleAnimModifier;
            public List<string> difficultyLevels = new List<string>();
            
            public List<Vector2> positionsV2s = new List<Vector2>();
            public List<Vector2> widthHeights = new List<Vector2>();
        }
    }
}
