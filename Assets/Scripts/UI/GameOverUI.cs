using System;
using System.Collections;
using Misc;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
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
                        _disposables.Clear();
                    })
                    .AddTo(_disposables);
            }
        }
    }
}
