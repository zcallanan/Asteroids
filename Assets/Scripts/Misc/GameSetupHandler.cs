using UniRx;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class GameSetupHandler : MonoBehaviour
    {
        private GameState _gameState;
        
        // TODO: Dispose of these
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        private void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
    
        private void Awake()
        {
            // TODO set these from start screen scene
            _gameState.GameDifficulty = 1; 
            _gameState.GameMode = 0;
            
            _gameState.CurrentLives = new ReactiveProperty<int>(2);
            _gameState.CurrentLevel = new ReactiveProperty<int>(0);
            _gameState.Score = new ReactiveProperty<int>(0);
            
            _gameState.IsStartScreenInit = new ReactiveProperty<bool>(false);
            _gameState.IsGameRunning = new ReactiveProperty<bool>(false);
            _gameState.IsGameReset = new ReactiveProperty<bool>(false);
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
