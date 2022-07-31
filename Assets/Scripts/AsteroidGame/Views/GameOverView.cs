using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.Views
{
    public class GameOverView : BaseGameView
    {
        [SerializeField] private Text gameOverText;

        private ViewData.Settings _viewDataSettings;
        
        [Inject]
        public void Construct(
            ViewData.Settings viewDataSettings)
        { 
            _viewDataSettings = viewDataSettings;
        }
        
        private void Start()
        {
            CheckIfSpawned(gameState.IsGameOverViewSpawned);

            CheckForChange(gameState.IsGameOver);

            DisposeIfGameNotRunning();
        }

        protected override void SetUpView()
        {
            gameOverText.enabled = false;
            
            var rectTransform = GetComponent<RectTransform>();

            gameObject.name = "GameOverView";
            Vector3 pos = _viewDataSettings.gameOverPos;
            Vector3 widthHeight = _viewDataSettings.gameOverSizeDelta;
            
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
            if (val is true)
            {
                WhenGameIsOverDelayThenShowGameOver();
            }
        }

        private void WhenGameIsOverDelayThenShowGameOver()
        {
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    gameOverText.enabled = true;
                })
                .AddTo(disposables);
        }
        
        public class Factory : PlaceholderFactory<GameOverView>
        {
        }
    }
}