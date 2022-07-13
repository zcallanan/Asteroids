using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class GameLifecycleHandler : MonoBehaviour
    {
        private GameState _gameState;
        private InputState _inputState;
        
        // TODO: Dispose of these
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
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
            // TODO set these from start screen scene
            _gameState.GameDifficulty = new ReactiveProperty<int>(1);
            _gameState.GameMode = new ReactiveProperty<int>(0);
            
            _gameState.CurrentLives = new ReactiveProperty<int>(2);
            _gameState.CurrentLevel = new ReactiveProperty<int>(0);
            _gameState.Score = new ReactiveProperty<int>(0);
            
            _gameState.IsGameRunning = new ReactiveProperty<bool>(false);
            _gameState.IsGameReset = new ReactiveProperty<bool>(false);

            _inputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _inputState.IsFiring = new ReactiveProperty<bool>(false);

            _inputState.IsApplyingThrust = new ReactiveProperty<bool>(false);
            _inputState.VerticalInput = new ReactiveProperty<float>(0);
            _inputState.HorizontalInput = new ReactiveProperty<float>(0);
        }

        private void Start()
        {
            AfterGameOverResetInitialValues();
        }

        private void AfterGameOverResetInitialValues()
        {
            _gameState.IsGameReset
                .Subscribe(isGameReset =>
                {
                    if (isGameReset)
                    {
                        _gameState.CurrentLives.Value = 2;
                        _gameState.CurrentLevel.Value = 0;
                        _gameState.Score.Value = 0;
                        
                        _gameState.IsGameReset.Value = false;
                    }
                })
                .AddTo(_disposables);
        }
    }
}
