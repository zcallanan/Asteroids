using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.UI
{
    public class ScoreUI : MonoBehaviour
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
            scoreText.text = _gameState.Score.Value.ToString();
            
            CheckIfScoreChanges();

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
