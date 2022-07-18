using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private ObjectTypes playerType;

        [SerializeField] private Text scoreText;

        private GameState _gameState;

        private ReactiveProperty<string> _scoreSource;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
        
        private void Start()
        {
            CheckIfScoreTextChanges();

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

        private void CheckIfScoreTextChanges()
        {
            _scoreSource = playerType == ObjectTypes.Player
                ? _gameState.PlayerScoreText
                : _gameState.OtherPlayerScoreText;
            
            _scoreSource
                .Subscribe(scoreValueText => scoreText.text = scoreValueText)
                .AddTo(_disposables);
        }
    }
}
