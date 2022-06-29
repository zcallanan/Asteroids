using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace AsteroidScripts
{
    public class AsteroidFacade : MonoBehaviour, IPoolable<int, AsteroidFacade.AsteroidSizes, IMemoryPool>
    {
        private BoundHandler _boundHandler;
        private AsteroidData.Settings _asteroidData;
        private Difficulty.Settings _difficultySettings;
        private GameState _gameState;

        private IMemoryPool _pool;

        private int _gameDifficulty;

        private Vector3 _randomDirection;
        private Vector3 _randomRotation;

        private float _maxSpeed;
        private float _minSpeed;
        private float _maxRotSpeed;
        private float _minRotSpeed;

        private float _asteroidSpeed;
        private float _asteroidRotSpeed;
        
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
            _gameDifficulty = _gameState.GameDifficulty;

            _maxSpeed = _difficultySettings.difficulties[_gameDifficulty].astMaxSpeed;
            _minSpeed = _difficultySettings.difficulties[_gameDifficulty].astMinSpeed;

            _maxRotSpeed = _asteroidData.maxRotSpeed;
            _minRotSpeed = _asteroidData.minRotSpeed;

            _asteroidRotSpeed = Random.Range(_minRotSpeed, _maxRotSpeed);
            _asteroidSpeed = Random.Range(_minSpeed, _maxSpeed);

            _randomRotation = new Vector3(Random.value / 10, Random.value / 10, Random.value / 10);
            _randomDirection = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));

            if (gameObject.GetComponent<ObservableTriggerTrigger>() == null)
            {
                gameObject
                    .AddComponent<ObservableTriggerTrigger>()
                    .UpdateAsObservable()
                    .SampleFrame(60)
                    .Subscribe(_ => Debug.Log($"Collision trigger added"))
                    .AddTo(_disposables);
            }

            if (gameObject.GetComponent<ObservableEnableTrigger>() == null)
            {
                gameObject
                    .AddComponent<ObservableEnableTrigger>()
                    .UpdateAsObservable()
                    .SampleFrame(60)
                    .Subscribe(_ => Debug.Log("Enable trigger added"))
                    .AddTo(_disposables);
            }
        }

        private void Update()
        {
            // TODO: Fix the rate
            Rotation = _randomRotation * (Time.deltaTime * _asteroidRotSpeed);

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

        public Material MeshRendererMaterial
        {
            set => GetComponent<MeshRenderer>().material = value;
        }

        public Mesh MeshFilterMesh
        {
            set => GetComponent<MeshFilter>().mesh = value;
        }

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