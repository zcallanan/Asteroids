using System;
using System.Collections.Generic;
using System.Linq;
using AsteroidGame.AsteroidScripts;
using AsteroidGame.PlayerScripts;
using AsteroidGame.UfoScripts;
using ProjectScripts;
using UnityEngine;

namespace AsteroidGame.Misc
{
    public class ScoreHandler
    {
        private readonly Settings _settings;
        private readonly GameState _gameState;
        private readonly InstanceRegistry _instanceRegistry;

        public ScoreHandler(
            Settings settings,
            GameState gameState,
            InstanceRegistry instanceRegistry)
        {
            _settings = settings;
            _gameState = gameState;
            _instanceRegistry = instanceRegistry;
        }

        public void UpdateScore(ObjectTypes whatDied, Collider collider)
        {
            var scoreRecipients = new List<ObjectTypes>();
            
            if (collider.GetComponent<BulletProjectile>())
            {
                var component = collider.GetComponent<BulletProjectile>();
                
                if (component.OriginType != ObjectTypes.SmallUfo || component.OriginType != ObjectTypes.LargeUfo)
                {
                    scoreRecipients.Add(component.OriginType);
                }
            }
            else if (collider.GetComponent<PlayerFacade>())
            {
                scoreRecipients.Add(collider.GetComponent<PlayerFacade>().PlayerType);
            }
            else if (collider.GetComponent<Ufo>() || collider.GetComponent<Asteroid>())
            {
                scoreRecipients.Add(ObjectTypes.Player);
                
                if (_gameState.GameMode.Value != 0)
                {
                    scoreRecipients.Add(ObjectTypes.OtherPlayer);
                }
            }
            
            foreach (var playerFacade in _instanceRegistry.playerFacades.Where(pFacade => scoreRecipients.Contains(pFacade.PlayerType)))
            {
                AddObjectValueToScore(whatDied, playerFacade);
            }
        }

        private void AddObjectValueToScore(ObjectTypes scoreTypes, PlayerFacade playerFacade)
        {
            switch (scoreTypes)
            {
                case ObjectTypes.SmallAsteroid:
                    playerFacade.Score.Value += _settings.smallAstVal;
                    break;
                case ObjectTypes.MediumAsteroid:
                    playerFacade.Score.Value += _settings.mediumAstVal;
                    break;
                case ObjectTypes.LargeAsteroid:
                    playerFacade.Score.Value += _settings.largeAstVal;
                    break;
                case ObjectTypes.SmallUfo:
                    playerFacade.Score.Value += _settings.smallUfoVal;
                    break;
                case ObjectTypes.LargeUfo:
                    playerFacade.Score.Value += _settings.largeUfoVal;
                    break;
                case ObjectTypes.OtherPlayer:
                    playerFacade.Score.Value += _settings.otherPlayerVal;
                    break;
                case ObjectTypes.Player:
                    playerFacade.Score.Value += _settings.playerVal;
                    break;
                default:
                    playerFacade.Score.Value += 0;
                    break;
            }

            SetScoreStringValues(playerFacade);
        }

        private void SetScoreStringValues(PlayerFacade playerFacade)
        {
            if (playerFacade.PlayerType == ObjectTypes.Player)
            {
                _gameState.PlayerScoreText.Value = playerFacade.Score.Value.ToString();
            }
            else if (playerFacade.PlayerType == ObjectTypes.OtherPlayer)
            {
                _gameState.OtherPlayerScoreText.Value = playerFacade.Score.Value.ToString();
            }
        }

        [Serializable]
        public class Settings
        {
            public int smallAstVal;
            public int mediumAstVal;
            public int largeAstVal;
            public int smallUfoVal;
            public int largeUfoVal;
            public int otherPlayerVal;
            public int playerVal;
        }
    }
}