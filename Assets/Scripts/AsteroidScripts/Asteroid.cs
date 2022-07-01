using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace AsteroidScripts
{
    public class Asteroid : MonoBehaviour, IPoolable<int, Asteroid.AsteroidSizes, IMemoryPool>, IDisposable
    {
        private BoundHandler _boundHandler;
        private AsteroidData.Settings _asteroidData;
        private Difficulty.Settings _difficultySettings;
        private GameState _gameState;

        private IMemoryPool _pool;
        
        private Vector3 _randomDirection;
        private Vector3 _randomRotation;

        private float _asteroidSpeed;
        private float _asteroidRotSpeed;

        private MeshRenderer _meshRenderer;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            BoundHandler boundHandler,
            AsteroidData.Settings asteroidData,
            Difficulty.Settings difficultySettings,
            GameState gameState)
        {
            _boundHandler = boundHandler;
            _asteroidData = asteroidData;
            _difficultySettings = difficultySettings;
            _gameState = gameState;
        }

        private void Start()
        {
            var gameDifficulty = _gameState.GameDifficulty;
            
            var maxSpeed = _difficultySettings.difficulties[gameDifficulty].astMaxSpeed;
            var minSpeed = _difficultySettings.difficulties[gameDifficulty].astMinSpeed;

            var maxRotSpeed = _asteroidData.maxRotSpeed;
            var minRotSpeed = _asteroidData.minRotSpeed;
            
            _asteroidRotSpeed = Random.Range(minRotSpeed, maxRotSpeed);
            _asteroidSpeed = Random.Range(minSpeed, maxSpeed);

            _randomRotation = new Vector3(Random.value / 10, Random.value / 10, Random.value / 10);
            _randomDirection = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));

            Dispose();
        }

        private void Update()
        {
            // TODO: Fix the rate
            Rotation(_randomRotation * (Time.deltaTime * _asteroidRotSpeed));

            var position = transform.position;
            position += _randomDirection * (Time.deltaTime * _asteroidSpeed);
            Position = position;
        }

        private void OnTriggerEnter(Collider other)
        {
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

        public int RenderValue { get; private set; }

        public AsteroidSizes Size { get; private set; }

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
        
        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        public void MeshRendererMaterial(Material mat)
        {
            GetComponent<MeshRenderer>().material = mat;
        }

        public void MeshFilterMesh(Mesh mesh)
        {
            GetComponent<MeshFilter>().mesh = mesh;
        }

        public class Factory : PlaceholderFactory<int, AsteroidSizes, Asteroid>
        {
        }
        
        public enum AsteroidSizes
        {
            SmallAsteroid,
            MediumAsteroid,
            LargeAsteroid
        }
        
        private void Rotation(Vector3 rot)
        {
            transform.Rotate(rot, Space.Self);
        }
    }
}