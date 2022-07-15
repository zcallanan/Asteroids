using ProjectScripts;
using UniRx;
using Zenject;

namespace AsteroidGame.Misc
{
    public class UfoSpawnInit : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UfoSpawnInit(
            GameState gameState)
        {
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            EnableUfoSpawnerTimedSpawn();
        }

        private void EnableUfoSpawnerTimedSpawn()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (isGameRunning)
                    {
                        _gameState.IsUfoSpawning.Value = true;
                    }
                    else
                    {
                        _disposables?.Clear();
                    }
                })
                .AddTo(_disposables);
        }
    }
}
