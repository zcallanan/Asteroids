using System;
using Installers;
using Misc;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Asteroid
{
    public enum AsteroidSizes
    {
        SmallAsteroid,
        MediumAsteroid,
        LargeAsteroid
    }
    
    public class AsteroidFacade : MonoBehaviour, IPoolable<float, AsteroidSizes, IMemoryPool>
    {
        private GameLevelHandler _gameLevelHandler;
        private ScoreHandler _scoreHandler;
        private BoundHandler _boundHandler;
        private AsteroidSpawner _asteroidSpawner;
        private AsteroidData.Settings _asteroidData;
        private Difficulty.Settings _difficultySettings;
        private GameState _gameState;

        private AsteroidSizes _type;
        private float _speed;
        private IMemoryPool _pool;

        private int _gameDifficulty;
        private int _mediumPerLarge;
        private int _smallPerMedium;

        private Vector3 _randomDirection;
        private Vector3 _randomRotation;

        private float _maxSpeed;
        private float _minSpeed;
        private float _maxRotSpeed;
        private float _minRotSpeed;
        
        [Inject]
        public void Construct(
            GameLevelHandler gameLevelHandler,
            ScoreHandler scoreHandler,
            BoundHandler boundHandler,
            AsteroidData.Settings asteroidData,
            AsteroidSpawner asteroidSpawner,
            Difficulty.Settings difficultySettings,
            GameState gameState)
        {
            _gameLevelHandler = gameLevelHandler;
            _scoreHandler = scoreHandler;
            _boundHandler = boundHandler;
            _asteroidData = asteroidData;
            _asteroidSpawner = asteroidSpawner;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
        }

        private void Start()
        {
            _gameDifficulty = _gameState.GameDifficulty;
            _mediumPerLarge = _difficultySettings.difficulties[_gameDifficulty].mediumPerLarge;
            _smallPerMedium = _difficultySettings.difficulties[_gameDifficulty].smallPerMedium;

            _maxSpeed = _difficultySettings.difficulties[_gameDifficulty].astMaxSpeed;
            _minSpeed = _difficultySettings.difficulties[_gameDifficulty].astMinSpeed;
            
            _maxRotSpeed = _asteroidData.maxRotSpeed;
            _minRotSpeed = _asteroidData.minRotSpeed;
            
            _randomRotation = new Vector3(Random.value/10, Random.value/10, Random.value/10);
            _randomDirection = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
        }

        private void Update()
        {
            var asteroidTransform = transform;
            asteroidTransform.Rotate(_randomRotation * (Time.deltaTime * Random.Range(_minRotSpeed, _maxRotSpeed)));
             
             var position = asteroidTransform.position;
             position += _randomDirection * (Time.deltaTime * Random.Range(_minSpeed, _maxSpeed));
             Position = position;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_type == AsteroidSizes.SmallAsteroid)
            {
                _scoreHandler.UpdateScore(ScoreTypes.SmallAsteroid);
            }
            else if (_type == AsteroidSizes.MediumAsteroid)
            {
                for (int i = 0; i < _smallPerMedium; i++)  
                {
                    _asteroidSpawner.SpawnAsteroid(AsteroidSizes.SmallAsteroid, transform.position);
                }
                
                _scoreHandler.UpdateScore(ScoreTypes.MediumAsteroid);
            }
            else if (_type == AsteroidSizes.LargeAsteroid)
            {
                for (int i = 0; i < _mediumPerLarge; i++)  
                {
                    _asteroidSpawner.SpawnAsteroid(AsteroidSizes.MediumAsteroid, transform.position);
                }
                
                _scoreHandler.UpdateScore(ScoreTypes.LargeAsteroid);
                
                _gameLevelHandler.RegisterSmallDeathToDetermineNextLevel();
            }
            
            _pool.Despawn(this);
            
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = _boundHandler.EnforceBounds(value);
        }

        public void OnSpawned(float speed, AsteroidSizes type, IMemoryPool pool)
        {
            _speed = speed;
            _type = type;
            _pool = pool;
        }
        
        public void OnDespawned()
        {
            _pool = null;
        }

        public class Factory : PlaceholderFactory<float, AsteroidSizes, AsteroidFacade>
        {
        }
    }
}
