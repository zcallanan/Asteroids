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
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
        
        private void Start()
        {
            if (_gameState.IsGameRunning.Value)
            {
                HideOtherPlayerScoreIfSinglePlayerMode();
                
                CheckIfScoreTextChanges();

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
        
        private void HideOtherPlayerScoreIfSinglePlayerMode()
        {
            if (_gameState.GameMode.Value == 0 && playerType == ObjectTypes.OtherPlayer)
            {
                scoreText.enabled = false;
            }
        }

        private void CheckIfScoreTextChanges()
        {
            var scoreSource = playerType == ObjectTypes.Player
                ? _gameState.PlayerScoreText
                : _gameState.OtherPlayerScoreText;
            
            scoreSource
                .Subscribe(scoreValueText => scoreText.text = scoreValueText)
                .AddTo(_disposables);
        }
    }
}
