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
        private readonly PlayerFacade _playerFacade;
        private readonly OtherPlayerFacade _otherPlayerFacade;
        private readonly Settings _settings;

        private bool _isPlayerOutOfLives;
        private bool _isOtherPlayerOutOfLives;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public GameOverHandler(
            GameState gameState,
            PlayerFacade playerFacade,
            OtherPlayerFacade otherPlayerFacade,
            Settings settings)
        {
            _gameState = gameState;
            _playerFacade = playerFacade;
            _otherPlayerFacade = otherPlayerFacade;
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
            _gameState.GameMode
                .Subscribe(gameMode =>
                {
                    CheckPlayerLives();

                    if (gameMode != 0 )
                    {
                        CheckOtherPlayerLives();
                    }
                })
                .AddTo(_disposables);
        }

        private void CheckPlayerLives()
        {
            _playerFacade.CurrentLives
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
                            _isPlayerOutOfLives = true;
                            SetGameOverIfBothPlayersAreOutOfLives();
                        }
                    }
                })
                .AddTo(_disposables);
        }
        
        private void CheckOtherPlayerLives()
        {
            _otherPlayerFacade.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _isOtherPlayerOutOfLives = true;
                        SetGameOverIfBothPlayersAreOutOfLives();
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
