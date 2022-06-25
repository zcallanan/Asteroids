using System;

namespace Misc
{
    public enum ScoreTypes
    {
        SmallAsteroid,
        MediumAsteroid,
        LargeAsteroid,
        SmallUfo,
        LargeUfo,
        OtherPlayer
    }
    
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

        public void UpdateScore(ScoreTypes scoreTypes)
        {
            switch (scoreTypes)
            {
                case ScoreTypes.SmallAsteroid:
                    _gameState.Score.Value += _settings.smallAstVal;
                    break;
                case ScoreTypes.MediumAsteroid:
                    _gameState.Score.Value += _settings.mediumAstVal;
                    break;
                case ScoreTypes.LargeAsteroid:
                    _gameState.Score.Value += _settings.largeAstVal;
                    break;
                case ScoreTypes.SmallUfo:
                    _gameState.Score.Value += _settings.smallUfoVal;
                    break;
                case ScoreTypes.LargeUfo:
                    _gameState.Score.Value += _settings.largeUfoVal;
                    break;
                case ScoreTypes.OtherPlayer:
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