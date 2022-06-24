using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class GameState : MonoBehaviour
    {
        private GameLevelHandler _gameLevelHandler;
        
        [Inject]
        public void Construct(GameLevelHandler gameLevelHandler)
        {
            _gameLevelHandler = gameLevelHandler;
        }

        private void Awake()
        {
            _gameLevelHandler.CurrentLevel = new ReactiveProperty<int>(0);
        }
    }
}
