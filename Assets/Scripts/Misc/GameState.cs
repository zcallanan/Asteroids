using UniRx;
using UnityEngine;

namespace Misc
{
    public class GameState : MonoBehaviour
    {
        public ReactiveProperty<int> CurrentLevel { get; private set; }
        public ReactiveProperty<int> Score { get; set; }
        
        public int GameDifficulty { get; set; }
        public int GameMode { get; set; }
        
        private void Awake()
        {
            // TODO set these from start screen scene
            GameDifficulty = 1; 
            GameMode = 0;
            
            CurrentLevel = new ReactiveProperty<int>(0);
            Score = new ReactiveProperty<int>(0);
        }
    }
}
