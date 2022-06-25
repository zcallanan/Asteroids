using Misc;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
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
            // GameManager.sharedInstance.IsGameOver.Subscribe(HandleGameOver).AddTo(_disposables);
            
            _gameState.Score
                .Subscribe(HandleScoreUpdate)
                .AddTo(_disposables);
            
            scoreText.text = "0";
        }

        private void HandleScoreUpdate(int score)
        {
            scoreText.text = score.ToString();
        }
        
        // private void HandleGameOver(bool isGameOver)
        // {
        //     if (isGameOver)
        //     {
        //         _disposables.Clear();
        //     }
        // }
    }
}
