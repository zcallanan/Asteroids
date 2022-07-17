using System;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.UI
{
    public class GameOverUI : MonoBehaviour
    {
        private Text _gameOverText;

        private GameState _gameState;
        private PlayerFacade _playerFacade;
        private OtherPlayerFacade _otherPlayerFacade;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            GameState gameState,
            PlayerFacade playerFacade,
            OtherPlayerFacade otherPlayerFacade)
        {
            _gameState = gameState;
            _playerFacade = playerFacade;
            _otherPlayerFacade = otherPlayerFacade;
        }

        private void Start()
        {
            _gameOverText = GetComponent<Text>();
            _gameOverText.enabled = false;

            CheckForGameOver();

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

        private void CheckForGameOver()
        {
            _gameState.IsGameOver
                .Subscribe(isGameOver =>
                {
                    if (isGameOver)
                    {
                        WhenGameIsOverDelayThenShowGameOver();
                    }
                })
                .AddTo(_disposables);
        }

        private void WhenGameIsOverDelayThenShowGameOver()
        {
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    _gameOverText.enabled = true;
                })
                .AddTo(_disposables);
        }
    }
}