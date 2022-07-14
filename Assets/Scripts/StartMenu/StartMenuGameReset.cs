using Misc;
using UniRx;
using Zenject;

namespace StartMenu
{
    public class StartMenuGameReset : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public StartMenuGameReset(
            GameState gameState)
        {
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            AfterGameOverResetToInitialValues();

            DisposeOnGameRunning();
        }
        
        private void DisposeOnGameRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void AfterGameOverResetToInitialValues()
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
