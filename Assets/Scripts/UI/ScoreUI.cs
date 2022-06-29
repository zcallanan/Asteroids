using System;
using Misc;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ScoreUI : MonoBehaviour, IDisposable
    {
        [SerializeField] private Text scoreText;

        private GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
        
        private void Start()
        {
            scoreText.text = "0";
            
            CheckIfScoreChanges();
            
            Dispose();
        }
        
        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void CheckIfScoreChanges()
        {
            _gameState.Score
                .Subscribe(HandleScoreUpdate)
                .AddTo(_disposables);
        }

        private void HandleScoreUpdate(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
