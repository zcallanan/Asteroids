using System;
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

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            GameState gameState)
        {
            _gameState = gameState;
        }

        private void Start()
        {
            _gameOverText = GetComponent<Text>();
            _gameOverText.enabled = false;

            CheckForChangeToCurrentLives();

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

        private void CheckForChangeToCurrentLives()
        {
            _gameState.CurrentLives
                .Subscribe(WhenLivesAreBelowZeroDelayThenShowGameOver)
                .AddTo(_disposables);
        }

        private void WhenLivesAreBelowZeroDelayThenShowGameOver(int lives)
        {
            if (lives < 0)
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
}