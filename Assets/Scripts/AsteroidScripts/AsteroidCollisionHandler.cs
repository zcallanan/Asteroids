using Misc;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace AsteroidScripts
{
    public class AsteroidCollisionHandler : IInitializable
    {
        private readonly AsteroidFacade _asteroidFacade;
        private readonly ScoreHandler _scoreHandler;
        private readonly AsteroidSpawner _asteroidSpawner;
        private readonly GameLevelHandler _gameLevelHandler;
        private readonly Difficulty.Settings _difficultySettings;
        private readonly GameState _gameState;

        private AsteroidFacade.AsteroidSizes _large;
        private AsteroidFacade.AsteroidSizes _medium;
        private AsteroidFacade.AsteroidSizes _small;
        
        private int _gameDifficulty;
        private int _mediumPerLarge;
        private int _smallPerMedium;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AsteroidCollisionHandler(
            AsteroidFacade asteroidFacade,
            ScoreHandler scoreHandler,
            AsteroidSpawner asteroidSpawner,
            GameLevelHandler gameLevelHandler,
            Difficulty.Settings difficultySettings,
            GameState gameState)
        {
            _asteroidFacade = asteroidFacade;
            _scoreHandler = scoreHandler;
            _asteroidSpawner = asteroidSpawner;
            _gameLevelHandler = gameLevelHandler;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            _gameDifficulty = _gameState.GameDifficulty;
            _mediumPerLarge = _difficultySettings.difficulties[_gameDifficulty].mediumPerLarge;
            _smallPerMedium = _difficultySettings.difficulties[_gameDifficulty].smallPerMedium;

            _large = AsteroidFacade.AsteroidSizes.LargeAsteroid;
            _medium = AsteroidFacade.AsteroidSizes.MediumAsteroid;
            _small = AsteroidFacade.AsteroidSizes.SmallAsteroid;

            _asteroidFacade
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => HandleCollision())
                .AddTo(_disposables);
        }
        
        private void HandleCollision()
        {
            if (_asteroidFacade.Size == _small)
            {
                _scoreHandler.UpdateScore(ScoreTypes.SmallAsteroid);
                
                _gameLevelHandler.RegisterSmallDeathToDetermineNextLevel();
            }
            else if (_asteroidFacade.Size == _medium)
            {
                for (int i = 0; i < _smallPerMedium; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(_asteroidFacade.RenderValue, _small, _asteroidFacade.transform.position);
                }

                _scoreHandler.UpdateScore(ScoreTypes.MediumAsteroid);
            }
            else if (_asteroidFacade.Size == _large)
            {
                for (int i = 0; i < _mediumPerLarge; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(_asteroidFacade.RenderValue, _medium, _asteroidFacade.transform.position);
                }

                _scoreHandler.UpdateScore(ScoreTypes.LargeAsteroid);
            }
        }
    }
}
