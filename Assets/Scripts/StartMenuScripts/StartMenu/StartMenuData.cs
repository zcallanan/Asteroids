using System;
using UnityEngine;

namespace StartMenuScripts.StartMenu
{
    public class StartMenuData
    {
        [Serializable]
        public class Settings
        {
            [Range(0f, 1f)] public float titleAnimModifier;
        }
    }
}
