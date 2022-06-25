using UniRx;
using UnityEngine;

namespace Misc
{
    public class GameState : MonoBehaviour
    {
        public ReactiveProperty<int> CurrentLevel { get; private set; }
        
        public int GameDifficulty { get; set; }

        private void Awake()
        {
            GameDifficulty = 1; // TODO set from start screen scene
            
            CurrentLevel = new ReactiveProperty<int>(0);
        }
    }
}
