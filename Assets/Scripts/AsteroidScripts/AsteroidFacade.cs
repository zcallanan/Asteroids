using System;
using System.Numerics;
using Installers;
using Misc;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace AsteroidScripts
{
    public class AsteroidFacade : MonoBehaviour, IPoolable<int, AsteroidFacade.AsteroidSizes, IMemoryPool>
    {
        private GameLevelHandler _gameLevelHandler;
        private ScoreHandler _scoreHandler;
        private BoundHandler _boundHandler;
        private AsteroidSpawner _asteroidSpawner;
        private AsteroidData.Settings _asteroidData;
        private Difficulty.Settings _difficultySettings;
        private GameState _gameState;

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

        private float _asteroidSpeed;
        private float _asteroidRotSpeed;

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

            _asteroidRotSpeed = Random.Range(_minRotSpeed, _maxRotSpeed);
            _asteroidSpeed = Random.Range(_minSpeed, _maxSpeed);

            _randomRotation = new Vector3(Random.value / 10, Random.value / 10, Random.value / 10);
            _randomDirection = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
        }

        private void Update()
        {
            Rotation = _randomRotation * (Time.deltaTime * _asteroidRotSpeed);

            var position = transform.position;
            position += _randomDirection * (Time.deltaTime * _asteroidSpeed);
            Position = position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Size == AsteroidSizes.SmallAsteroid)
            {
                _scoreHandler.UpdateScore(ScoreTypes.SmallAsteroid);
            }
            else if (Size == AsteroidSizes.MediumAsteroid)
            {
                for (int i = 0; i < _smallPerMedium; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(RenderValue, AsteroidSizes.SmallAsteroid, transform.position);
                }

                _scoreHandler.UpdateScore(ScoreTypes.MediumAsteroid);
            }
            else if (Size == AsteroidSizes.LargeAsteroid)
            {
                for (int i = 0; i < _mediumPerLarge; i++)
                {
                    _asteroidSpawner.SpawnAsteroid(RenderValue, AsteroidSizes.MediumAsteroid, transform.position);
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

        public Vector3 Scale
        {
            set => transform.localScale = value;
        }

        public int RenderValue { get; set; }

        public Material MeshRendererMaterial
        {
            set => GetComponent<MeshRenderer>().material = value;
        }

        public Mesh MeshFilterMesh
        {
            set => GetComponent<MeshFilter>().mesh = value;
        }

        public AsteroidSizes Size { get; set; }

        public void OnSpawned(int renderValue, AsteroidSizes type, IMemoryPool pool)
        {
            RenderValue = renderValue;
            Size = type;
            _pool = pool;
        }
        
        public void OnDespawned()
        {
            _pool = null;
        }

        public class Factory : PlaceholderFactory<int, AsteroidSizes, AsteroidFacade>
        {
        }
        
        public enum AsteroidSizes
        {
            SmallAsteroid,
            MediumAsteroid,
            LargeAsteroid
        }
        
        private Vector3 Rotation
        {
            set => transform.Rotate(value);
        }
    }
}