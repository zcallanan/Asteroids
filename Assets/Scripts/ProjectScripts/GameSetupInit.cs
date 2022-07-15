using UniRx;
using UnityEngine;
using Zenject;

namespace ProjectScripts
{
    public class GameSetupInit : MonoBehaviour
    {
        private GameState _gameState;
        private InputState _inputState;
        
        [Inject]
        private void Construct(
            GameState gameState,
            InputState inputState)
        {
            _gameState = gameState;
            _inputState = inputState;
        }
    
        private void Awake()
        {
            _gameState.GameDifficulty = new ReactiveProperty<int>(1);
            _gameState.GameMode = new ReactiveProperty<int>(0);
            
            _gameState.CurrentLives = new ReactiveProperty<int>(2);
            _gameState.CurrentLevel = new ReactiveProperty<int>(0);
            _gameState.Score = new ReactiveProperty<int>(0);

            _gameState.IsFiringEnabled = new ReactiveProperty<bool>(false);
            _gameState.IsGameRunning = new ReactiveProperty<bool>(false);
            _gameState.IsGameReset = new ReactiveProperty<bool>(false);

            _inputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _inputState.IsFiring = new ReactiveProperty<bool>(false);

            _inputState.IsApplyingThrust = new ReactiveProperty<bool>(false);
            _inputState.VerticalInput = new ReactiveProperty<float>(0);
            _inputState.HorizontalInput = new ReactiveProperty<float>(0);
        }
    }
}
