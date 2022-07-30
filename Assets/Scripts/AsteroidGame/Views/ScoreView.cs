using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.Views
{
    public class ScoreView : AbstractView
    {
        [SerializeField] private Text scoreText;
        
        private ObjectTypes _playerType;
        private ViewData.Settings _viewDataSettings;
        
        [Inject]
        public void Construct(
            [InjectOptional] ObjectTypes playerType,
            ViewData.Settings viewDataSettings)
        {
            _playerType = playerType;
            _viewDataSettings = viewDataSettings;
        }
        
        private void Start()
        {
            CheckIfSpawned(gameState.AreScoreViewsSpawned, _playerType);
            
            var scoreSource = _playerType == ObjectTypes.Player
                ? gameState.PlayerScoreText
                : gameState.OtherPlayerScoreText;
            
            CheckForChange(scoreSource);

            DisposeIfGameNotRunning();
        }

        protected override void SetUp()
        {
            var rectTransform = GetComponent<RectTransform>();
            Vector3 pos;
            Vector3 widthHeight;

            if (_playerType == ObjectTypes.OtherPlayer)
            {
                gameObject.name = "P2ScoreView";
                pos = _viewDataSettings.p2ScoreViewPos;
                widthHeight = _viewDataSettings.p2ScoreSizeDelta;
                scoreText.GetComponent<Text>().color = _viewDataSettings.p2Color;
            }
            else
            {
                gameObject.name = "P1ScoreView";
                pos = _viewDataSettings.p1ScoreViewPos;
                widthHeight = _viewDataSettings.p1ScoreSizeDelta;
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
