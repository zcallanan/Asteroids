using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.Views
{
    public class LivesView : MonoBehaviour
    {
        [SerializeField] private Image image;

        private ObjectTypes _playerType;
        
        private GameState _gameState;
        private ViewData.Settings _viewDataSettings;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            GameState gameState,
            ViewData.Settings viewDataSettings,
            [InjectOptional] ObjectTypes playerType)
        {
            _gameState = gameState;
            _viewDataSettings = viewDataSettings;
            _playerType = playerType;
        }

        private void Start()
        {
            CheckIfSpawned();
            
            CheckForChangeToLivesSprite();

            DisposeIfGameNotRunning();
        }

        private void CheckIfSpawned()
        {
            _gameState.AreLivesViewsSpawned
                .Subscribe(areLivesViewsSpawned =>
                    {
                        if (areLivesViewsSpawned &&
                            (_playerType == ObjectTypes.Player || _playerType == ObjectTypes.OtherPlayer))
                        {
                            SetupImage();
                        }
                    })
                    .AddTo(_disposables);
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
        
        private void SetupImage()
        {
            var rectTransform = GetComponent<RectTransform>();
            Vector3 pos;
            Vector3 widthHeight;

            if (_playerType == ObjectTypes.OtherPlayer)
            {
                pos = _viewDataSettings.otherPlayerLivesViewPos;
                widthHeight = _viewDataSettings.otherPlayerSizeDelta;
            }
            else
            {
                pos = _viewDataSettings.playerLivesViewPos;
                widthHeight = _viewDataSettings.playerSizeDelta;
            }

            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.pivot = Vector2.zero;
            
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.Euler(0,0,0);
            
            rectTransform.sizeDelta = new Vector2(widthHeight.x, widthHeight.y);
            rectTransform.localPosition = new Vector3(pos.x, pos.y, transform.position.z);
        }

        private void CheckForChangeToLivesSprite()
        {
            var imageSource = _playerType == ObjectTypes.Player
                ? _gameState.PlayerLivesSprite
                : _gameState.OtherPlayerLivesSprite;

            imageSource
                .Subscribe(sprite =>
                {
                    if (sprite == null && image)
                    {
                        image.gameObject.SetActive(false);
                    }
                    else if (image)
                    {
                        if (!image.gameObject.activeSelf)
                        {
                            image.gameObject.SetActive(true);
                        }

                        image.sprite = sprite;
                    }
                })
                .AddTo(_disposables);
        }
        
        public class Factory : PlaceholderFactory<ObjectTypes, LivesView>
        {
        }
    }
}
