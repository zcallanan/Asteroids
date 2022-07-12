using Misc;
using UnityEngine;
using Zenject;

namespace UI
{
    public class SelectToStartUI : MonoBehaviour
    {
        private GameState _gameState;

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
        
        public void StartGame()
        {
            _gameState.IsGameRunning.Value = true;
        }
    }
}
