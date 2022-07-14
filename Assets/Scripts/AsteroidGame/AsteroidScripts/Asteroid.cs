using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace AsteroidGame.AsteroidScripts
{
    public class Asteroid : MonoBehaviour, IPoolable<int, ObjectTypes, IMemoryPool>, IDisposable
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
        
        public void Dispose()
        {
            _pool.Despawn(this);
        }
        
        public class Factory : PlaceholderFactory<int, ObjectTypes, Asteroid>
        {
        }

        private void Start()
        {
            var gameDifficulty = _gameState.GameDifficulty;
            
            var maxSpeed = _difficultySettings.difficulties[gameDifficulty.Value].astMaxSpeed;
            var minSpeed = _difficultySettings.difficulties[gameDifficulty.Value].astMinSpeed;

            var maxRotSpeed = _asteroidData.maxRotSpeed;
            var minRotSpeed = _asteroidData.minRotSpeed;
            
            _asteroidRotSpeed = Random.Range(minRotSpeed, maxRotSpeed);
            _asteroidSpeed = Random.Range(minSpeed, maxSpeed);

            _randomRotation = new Vector3(Random.value / 10, Random.value / 10, Random.value / 10);
            _randomDirection = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
        }

        private void Update()
        {
            // TODO: Fix the rate
            SetRotation(_randomRotation * (Time.deltaTime * _asteroidRotSpeed));

            var position = transform.position;
            position += _randomDirection * (Time.deltaTime * _asteroidSpeed);
            Position = position;
        }

        private void OnTriggerEnter(Collider other)
        {
            Dispose();
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = _boundHandler.EnforceBounds(value);
        }

        public int RenderValue { get; private set; }

        public ObjectTypes Size { get; private set; }

        public void OnSpawned(int renderValue, ObjectTypes size, IMemoryPool pool)
        {
            RenderValue = renderValue;
            Size = size;
            _pool = pool;
        }
        
        public void OnDespawned()
        {
            _pool = null;
        }

        public void SetMeshRendererMaterial(Material mat)
        {
            GetComponent<MeshRenderer>().material = mat;
        }

        public void SetMeshFilterMesh(Mesh mesh)
        {
            GetComponent<MeshFilter>().mesh = mesh;
        }
        
        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        
        private void SetRotation(Vector3 rot)
        {
            transform.Rotate(rot, Space.Self);
        }
    }
}