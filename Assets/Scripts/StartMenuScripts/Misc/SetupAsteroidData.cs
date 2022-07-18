using ProjectScripts;
using UniRx;
using Zenject;

namespace StartMenuScripts.Misc
{
    public class SetupAsteroidData : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public SetupAsteroidData(
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
                        _gameState.CurrentLevel.Value = 0;
                        _gameState.PlayerScoreText.Value = "0";
                        _gameState.OtherPlayerScoreText.Value = "0";
                        
                        _gameState.IsFiringEnabled.Value = false;
                        _gameState.IsGameReset.Value = false;
                        _gameState.IsUfoSpawning.Value = false;
                        _gameState.IsGameOver.Value = false;
                    }
                })
                .AddTo(_disposables);
        }
    }
}
