using UniRx;
using UnityEngine;

namespace ProjectScripts
{
    public enum ObjectTypes
    {
        SmallAsteroid,
        MediumAsteroid,
        LargeAsteroid,
        SmallUfo,
        LargeUfo,
        OtherPlayer,
        Player
    }
    
    public class GameState
    {
        public ReactiveProperty<int> CurrentLevel { get; set; }
        public ReactiveProperty<int> Score { get; set; }
        
        public ReactiveProperty<bool> IsFiringEnabled { get; set; }
        public ReactiveProperty<bool> IsGameRunning { get; set; }
        public ReactiveProperty<bool> IsGameReset { get; set; }
        
        public ReactiveProperty<bool> IsGameOver { get; set; }

        public ReactiveProperty<bool> IsUfoSpawning { get; set; }
        
        public ReactiveProperty<int> GameDifficulty { get; set; }
        public ReactiveProperty<int> GameMode { get; set; }
        
        public ReactiveProperty<Sprite> PlayerLivesSprite { get; set; }
        
        public ReactiveProperty<Sprite> OtherPlayerLivesSprite { get; set; }
    }
}
