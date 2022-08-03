using System;
using System.Collections.Generic;
using AsteroidGame.Misc;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.ViewModels
{
    public class LivesViewModel : IInitializable
    {
        private readonly GameInstanceRegistry _instanceRegistry;
        private readonly GameState _gameState;
        private readonly Settings _settings;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LivesViewModel(
            GameInstanceRegistry instanceRegistry,
            GameState gameState,
            Settings settings)
        {
            _instanceRegistry = instanceRegistry;
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
            foreach (var playerFacade in _instanceRegistry.playerFacades)
            {
                UpdateSpriteOnLivesChange(playerFacade);
            }
        }

        private void UpdateSpriteOnLivesChange(PlayerFacade playerFacade)
        {
            playerFacade.CurrentLives
                .Subscribe(lives =>
                {
                    if (playerFacade.PlayerType == ObjectTypes.Player)
                    {
                        _gameState.PlayerLivesSprite.Value = SetSpriteValue(lives, _settings.playerLivesSprites);
                    } 
                    else if (playerFacade.PlayerType == ObjectTypes.OtherPlayer)
                    {
                        _gameState.OtherPlayerLivesSprite.Value =
                            SetSpriteValue(lives, _settings.otherPlayerLivesSprites);
                    }
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
