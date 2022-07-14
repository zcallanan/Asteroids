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
        
        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
        
        private void Start()
        {
            scoreText.text = "0";
            
            CheckIfScoreChanges();
        }

        private void CheckIfScoreChanges()
        {
            _gameState.Score
                .Subscribe(HandleScoreUpdate)
                .AddTo(this);
        }

        private void HandleScoreUpdate(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
