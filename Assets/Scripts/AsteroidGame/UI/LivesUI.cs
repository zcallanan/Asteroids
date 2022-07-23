using ProjectScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AsteroidGame.UI
{
    public class LivesUI : MonoBehaviour
    {
        [SerializeField] private ObjectTypes playerType;

        private GameState _gameState;

        private Image _image;
        
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
                _image = GetComponent<Image>();
            
                CheckForChangeToLivesSprite();

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

        private void CheckForChangeToLivesSprite()
        {
            var imageSource = playerType == ObjectTypes.Player
                ? _gameState.PlayerLivesSprite
                : _gameState.OtherPlayerLivesSprite;

            imageSource
                .Subscribe(sprite =>
                {
                    if (sprite == null)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        if (gameObject.activeSelf == false)
                        {
                            gameObject.SetActive(true);
                        }

                        _image.sprite = sprite;
                    }
                })
                .AddTo(_disposables);
        }
    }
}
