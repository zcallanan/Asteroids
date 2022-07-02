using Misc;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace AsteroidScripts
{
    public class AsteroidCollisionHandler : IInitializable
    {
        private readonly Asteroid _asteroid;
        private readonly ScoreHandler _scoreHandler;
        private readonly AsteroidSpawner _asteroidSpawner;
        private readonly GameLevelHandler _gameLevelHandler;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;

        private Asteroid.AsteroidSizes _large;
        private Asteroid.AsteroidSizes _medium;
        private Asteroid.AsteroidSizes _small;
        
        private int _mediumPerLarge;
        private int _smallPerMedium;
        
        public AsteroidCollisionHandler(
            Asteroid asteroid,
            ScoreHandler scoreHandler,
            AsteroidSpawner asteroidSpawner,
            GameLevelHandler gameLevelHandler,
            Difficulty.Settings difficultySettings,
            GameState gameState)
        {
            _asteroid = asteroid;
            _scoreHandler = scoreHandler;
            _asteroidSpawner = asteroidSpawner;
            _gameLevelHandler = gameLevelHandler;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            var gameDifficulty = _gameState.GameDifficulty;
            _mediumPerLarge = _difficultySettings.difficulties[gameDifficulty].mediumPerLarge;
            _smallPerMedium = _difficultySettings.difficulties[gameDifficulty].smallPerMedium;

            _large = Asteroid.AsteroidSizes.LargeAsteroid;
            _medium = Asteroid.AsteroidSizes.MediumAsteroid;
            _small = Asteroid.AsteroidSizes.SmallAsteroid;

            HandleCollisionOnTriggerEnter();
        }

        private void HandleCollision()
        {
            if (_asteroid.Size == _small)
            {
                _scoreHandler.UpdateScore(ObjectTypes.SmallAsteroid);
                
                _gameLevelHandler.RegisterSmallDeathToDetermineNextLevel();
            }
            else if (_asteroid.Size == _medium)
            {
                for (int i = 0; i < _smallPerMedium; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(_asteroid.RenderValue, _small, _asteroid.transform.position);
                }

                _scoreHandler.UpdateScore(ObjectTypes.MediumAsteroid);
            }
            else if (_asteroid.Size == _large)
            {
                for (int i = 0; i < _mediumPerLarge; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(_asteroid.RenderValue, _medium, _asteroid.transform.position);
                }

                _scoreHandler.UpdateScore(ObjectTypes.LargeAsteroid);
            }
        }
        
        private void HandleCollisionOnTriggerEnter()
        {
            _asteroid
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => HandleCollision())
                .AddTo(_asteroid.gameObject);
        }
    }
}
