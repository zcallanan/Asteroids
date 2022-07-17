using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class OtherPlayerHandler : IInitializable
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public OtherPlayerHandler(
            GameState gameState,
            Player player)
        {
            _gameState = gameState;
            _player = player;
        }

        public void Initialize()
        {
            HidePlayerTwoOrEnableCollision();

            DisposeIfGameNotRunning();
        }
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void HidePlayerTwoOrEnableCollision()
        {
            _gameState.GameMode
                .Subscribe(gameMode =>
                {
                    if (gameMode == 0)
                    {
                        HidePlayerTwoInSinglePlayerMode();
                    }
                    else
                    {
                        _player.CanCollide = true;
                    }
                })
                .AddTo(_disposables);
        }

        private void HidePlayerTwoInSinglePlayerMode()
        {
            if (_player.PlayerType == ObjectTypes.Player)
            {
                // Dead players don't fire bullets
                _player.IsDead = true; 
                _player.GameObj.SetActive(false);
            }
            else
            {
                _player.CanCollide = true;
            }
        }
    }
}
