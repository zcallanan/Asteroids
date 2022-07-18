using System;
using AsteroidGame.PlayerScripts;
using ProjectScripts;

namespace AsteroidGame.Misc
{
    public class ScoreHandler
    {
        private readonly Settings _settings;
        private readonly GameState _gameState;
        private readonly PlayerRegistry _playerRegistry;

        public ScoreHandler(
            Settings settings,
            GameState gameState,
            PlayerRegistry playerRegistry)
        {
            _settings = settings;
            _gameState = gameState;
            _playerRegistry = playerRegistry;
        }

        public void UpdateScore(ObjectTypes scoreTypes)
        {
            foreach (var playerFacade in _playerRegistry.playerFacades)
            {
                AddObjectValueToScore(scoreTypes, playerFacade);
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
        }
    }
}