using System;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UniRx;
using Zenject;

namespace AsteroidGame.Misc
{
    public class GameOverHandler : IInitializable
    {
        private readonly GameState _gameState;
        private readonly InstanceRegistry _instanceRegistry;
        private readonly Settings _settings;

        private bool _isPlayerOutOfLives;
        private bool _isOtherPlayerOutOfLives;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public GameOverHandler(
            GameState gameState,
            InstanceRegistry instanceRegistry,
            Settings settings)
        {
            _gameState = gameState;
            _instanceRegistry = instanceRegistry;
            _settings = settings;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                CheckIfPlayersAreOutOfLives();
                
                ResetGameAfterGameOver();

                DisposeIfGameNotRunning();
            }
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
        
        private void CheckIfPlayersAreOutOfLives()
        {
            foreach (var playerFacade in _instanceRegistry.playerFacades)
            {
                CheckPlayerLives(playerFacade);
            }
        }

        private void CheckPlayerLives(PlayerFacade playerFacade)
        {
            playerFacade.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        if (_gameState.GameMode.Value == 0)
                        {
                            _gameState.IsGameOver.Value = true;
                        }
                        else
                        {
                            if (playerFacade.PlayerType == ObjectTypes.Player)
                            {
                                _isPlayerOutOfLives = true;
                            }
                            else if (playerFacade.PlayerType == ObjectTypes.OtherPlayer)
                            {
                                _isOtherPlayerOutOfLives = true;
                            }
                            
                            SetGameOverIfBothPlayersAreOutOfLives();
                        }
                    }
                })
                .AddTo(_disposables);
        }

        private void SetGameOverIfBothPlayersAreOutOfLives()
        {
            if (_isPlayerOutOfLives && _isOtherPlayerOutOfLives)
            {
                _gameState.IsGameOver.Value = true;
            }
        }

        private void ResetGameAfterGameOver()
        {
            _gameState.IsGameOver
                .Subscribe(isGameOver =>
                {
                    if (isGameOver)
                    {
                        DelayBeforeGameReset();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void DelayBeforeGameReset()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.delayBeforeGameReset))
                .Subscribe(_ =>
                {
                    _gameState.IsGameRunning.Value = false;
                    _gameState.IsGameReset.Value = true;
                })
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float delayBeforeGameReset;
        }
        
    }
}
