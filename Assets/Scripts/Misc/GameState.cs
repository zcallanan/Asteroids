using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

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
    
    public class GameState : MonoBehaviour
    {
        public ReactiveProperty<int> CurrentLives { get; private set; }
        public ReactiveProperty<int> CurrentLevel { get; private set; }
        public ReactiveProperty<int> Score { get; private set; }
        public ReactiveProperty<bool> IsStartScreenInit { get; private set; }
        public ReactiveProperty<bool> IsGameRunning { get; private set; }

        public int GameDifficulty { get; set; }
        public int GameMode { get; set; }
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private void Awake()
        {
            // TODO set these from start screen scene
            GameDifficulty = 1; 
            GameMode = 0;
            
            CurrentLives = new ReactiveProperty<int>(2);
            CurrentLevel = new ReactiveProperty<int>(0);
            Score = new ReactiveProperty<int>(0);
            IsStartScreenInit = new ReactiveProperty<bool>(false);
            IsGameRunning = new ReactiveProperty<bool>(false);

        }
    }
}
