using System;
using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.UI
{
    public class LivesUI : MonoBehaviour
    {
        [SerializeField] private Sprite[] lifeCountSprite;

        private GameState _gameState;

        private Image _imageComponent;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        private void Start()
        {
            _imageComponent = gameObject.GetComponent<Image>();
            
            CheckForChangeToCurrentLivesAfterDelay();

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

        private void CheckForChangeToCurrentLivesAfterDelay()
        {
            _gameState.CurrentLives
                .Throttle(TimeSpan.FromSeconds(.5))
                .Subscribe(ChangeNumberOfLivesSprite)
                .AddTo(_disposables);
        }

        private void ChangeNumberOfLivesSprite(int currentLives)
        {
            switch (currentLives)
            {
                case 0:
                {
                    _imageComponent.sprite = null;
                    _imageComponent.rectTransform.sizeDelta = Vector2.zero;
                    break;
                }
                case 1:
                {
                    _imageComponent.sprite = lifeCountSprite[0];
                    break;
                }
                case 2:
                {
                    _imageComponent.sprite = lifeCountSprite[1];
                    break;
                }
                case 3:
                {
                    _imageComponent.sprite = lifeCountSprite[2];
                    break;
                }
                case 4:
                {
                    _imageComponent.sprite = lifeCountSprite[3];
                    break;
                }
                case 5:
                {
                    _imageComponent.sprite = lifeCountSprite[4];
                    break;
                }
                case 6:
                {
                    _imageComponent.sprite = lifeCountSprite[5];
                    break;
                }
                case 7:
                {
                    _imageComponent.sprite = lifeCountSprite[6];
                    break;
                }
                case 8:
                {
                    _imageComponent.sprite = lifeCountSprite[7];
                    break;
                }
                case 9:
                {
                    _imageComponent.sprite = lifeCountSprite[8];
                    break;
                }
                case 10:
                {
                    _imageComponent.sprite = lifeCountSprite[9];
                    break;
                }
                default:
                {
                    _imageComponent.sprite = lifeCountSprite[10];
                    break;
                }
            }
        }
    }
}
