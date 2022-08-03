using ProjectScripts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.Views
{
    public class LivesView : BaseGameView
    {
        [SerializeField] private Image image;

        private ObjectTypes _playerType;
        private ViewData.Settings _viewDataSettings;
        
        [Inject]
        public void Construct(
            ViewData.Settings viewDataSettings,
            [InjectOptional] ObjectTypes playerType)
        {
            _viewDataSettings = viewDataSettings;
            _playerType = playerType;
        }

        private void Start()
        {
            if (_playerType == ObjectTypes.Player || _playerType == ObjectTypes.OtherPlayer)
            {
                CheckIfSpawned(gameState.AreLivesViewsSpawned);
            }
            
            var imageSource = _playerType == ObjectTypes.Player
                ? gameState.PlayerLivesSprite
                : gameState.OtherPlayerLivesSprite;
            
            CheckForChange(imageSource);

            DisposeIfGameNotRunning();
        }

        protected override void SetUpView()
        {
            var rectTransform = GetComponent<RectTransform>();
            Vector3 pos;
            Vector3 widthHeight;

            if (_playerType == ObjectTypes.OtherPlayer)
            {
                gameObject.name = "P2LivesView";
                pos = _viewDataSettings.p2LivesViewPos;
                widthHeight = _viewDataSettings.p2LivesSizeDelta;
            }
            else
            {
                gameObject.name = "P1LivesView";
                pos = _viewDataSettings.p1LivesViewPos;
                widthHeight = _viewDataSettings.p1LivesSizeDelta;
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
            if (val == null && image)
            {
                image.gameObject.SetActive(false);
            }
            else if (image)
            {
                if (!image.gameObject.activeSelf)
                {
                    image.gameObject.SetActive(true);
                }

                image.sprite = val as Sprite;
            }
        }

        public class Factory : PlaceholderFactory<ObjectTypes, LivesView>
        {
        }
    }
}
