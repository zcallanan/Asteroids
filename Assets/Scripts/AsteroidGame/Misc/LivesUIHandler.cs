using System;
using System.Collections.Generic;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Misc
{
    public class LivesUIHandler : IInitializable
    {
        private readonly PlayerFacade _playerFacade;
        private readonly OtherPlayerFacade _otherPlayerFacade;
        private readonly GameState _gameState;
        private readonly Settings _settings;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LivesUIHandler(
            PlayerFacade playerFacade,
            OtherPlayerFacade otherPlayerFacade,
            GameState gameState,
            Settings settings)
        {
            _playerFacade = playerFacade;
            _otherPlayerFacade = otherPlayerFacade;
            _gameState = gameState;
            _settings = settings;
        }

        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                SetInitialSprites();

                UpdateSprites();

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

        private void SetInitialSprites()
        {
            _gameState.PlayerLivesSprite.Value = _settings.playerLivesSprites[1];

            if (_gameState.GameMode.Value != 0)
            {
                _gameState.OtherPlayerLivesSprite.Value = _settings.otherPlayerLivesSprites[1];
            }
        }

        private void UpdateSprites()
        {
            UpdateSpriteOnPlayerLivesChange();

            if (_gameState.GameMode.Value != 0)
            {
                UpdateSpriteOnOtherPlayerLivesChange();
            }
        }

        private void UpdateSpriteOnPlayerLivesChange()
        {
            _playerFacade.CurrentLives
                .Subscribe(lives =>
                {
                    _gameState.PlayerLivesSprite.Value = SetSpriteValue(lives, _settings.playerLivesSprites);
                })
                .AddTo(_disposables);
        }

        private void UpdateSpriteOnOtherPlayerLivesChange()
        {
            _otherPlayerFacade.CurrentLives
                .Subscribe(lives =>
                {
                    _gameState.OtherPlayerLivesSprite.Value =
                        SetSpriteValue(lives, _settings.otherPlayerLivesSprites);
                })
                .AddTo(_disposables);
        }

        private static Sprite SetSpriteValue(int lives, IReadOnlyList<Sprite> spriteList)
        {
            Sprite result = null;
            
            if (lives > 10)
            {
                result = spriteList[10];
            }
            else if (lives > 0)
            {
                result = spriteList[lives - 1];
            }

            return result;
        }

        [Serializable]
        public class Settings
        {
            public List<Sprite> playerLivesSprites = new List<Sprite>();
            public List<Sprite> otherPlayerLivesSprites = new List<Sprite>();
        }
    }
}
