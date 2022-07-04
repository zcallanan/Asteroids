using System;

namespace Misc
{
    public class ScoreHandler
    {
        private readonly Settings _settings;
        private readonly GameState _gameState;

        public ScoreHandler(
            Settings settings,
            GameState gameState)
        {
            _settings = settings;
            _gameState = gameState;
        }

        public void UpdateScore(ObjectTypes scoreTypes)
        {
            switch (scoreTypes)
            {
                case ObjectTypes.SmallAsteroid:
                    _gameState.Score.Value += _settings.smallAstVal;
                    break;
                case ObjectTypes.MediumAsteroid:
                    _gameState.Score.Value += _settings.mediumAstVal;
                    break;
                case ObjectTypes.LargeAsteroid:
                    _gameState.Score.Value += _settings.largeAstVal;
                    break;
                case ObjectTypes.SmallUfo:
                    _gameState.Score.Value += _settings.smallUfoVal;
                    break;
                case ObjectTypes.LargeUfo:
                    _gameState.Score.Value += _settings.largeUfoVal;
                    break;
                case ObjectTypes.OtherPlayer:
                    _gameState.Score.Value += _settings.otherPlayerVal;
                    break;
                default:
                    _gameState.Score.Value += 0;
                    break;
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