using UniRx;

namespace Misc
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
        public ReactiveProperty<int> CurrentLives { get; set; }
        public ReactiveProperty<int> CurrentLevel { get; set; }
        public ReactiveProperty<int> Score { get; set; }
        
        public ReactiveProperty<bool> IsGameRunning { get; set; }
        public ReactiveProperty<bool> IsGameReset { get; set; }
        
        
        
        public ReactiveProperty<int> GameDifficulty { get; set; }
        public  ReactiveProperty<int> GameMode { get; set; }
    }
}
