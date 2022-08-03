using ProjectScripts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.Views
{
    public class ScoreView : BaseGameView
    {
        [SerializeField] private Text scoreText;
        
        private ObjectTypes _playerType;
        private GameViewData.Settings _gameViewDataSettings;
        
        [Inject]
        public void Construct(
            [InjectOptional] ObjectTypes playerType,
            GameViewData.Settings gameViewDataSettings)
        {
            _playerType = playerType;
            _gameViewDataSettings = gameViewDataSettings;
        }
        
        private void Start()
        {
            CheckIfSpawned(gameState.AreScoreViewsSpawned);
            
            var scoreSource = _playerType == ObjectTypes.Player
                ? gameState.PlayerScoreText
                : gameState.OtherPlayerScoreText;
            
            CheckForChange(scoreSource);

            ToggleDispose(false);
        }

        protected override void SetUpView()
        {
            var rectTransform = GetComponent<RectTransform>();
            Vector3 pos;
            Vector3 widthHeight;

            if (_playerType == ObjectTypes.OtherPlayer)
            {
                gameObject.name = "P2ScoreView";
                pos = _gameViewDataSettings.p2ScoreViewPos;
                widthHeight = _gameViewDataSettings.p2ScoreSizeDelta;
                scoreText.GetComponent<Text>().color = _gameViewDataSettings.p2Color;
            }
            else
            {
                gameObject.name = "P1ScoreView";
                pos = _gameViewDataSettings.p1ScoreViewPos;
                widthHeight = _gameViewDataSettings.p1ScoreSizeDelta;
            }

            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.pivot = Vector2.zero;
            
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.Euler(0,0,0);
            
            rectTransform.sizeDelta = new Vector2(widthHeight.x, widthHeight.y);
            rectTransform.localPosition = new Vector3(pos.x, pos.y, transform.position.z);
        }

        protected override void UpdateVal<T>(T val)
        {
            scoreText.text = val as string;
        }

        public class Factory : PlaceholderFactory<ObjectTypes, ScoreView>
        {
        }
    }
}
