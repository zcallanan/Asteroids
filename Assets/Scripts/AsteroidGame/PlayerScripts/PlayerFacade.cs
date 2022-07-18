using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;
        private PlayerLifecycleHandler _playerLifecycleHandler;
        private GameState _gameState;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            Player player,
            PlayerLifecycleHandler playerLifecycleHandler,
            GameState gameState)
        {
            _player = player;
            _playerLifecycleHandler = playerLifecycleHandler;
            _gameState = gameState;
        }

        public Vector3 Position
        {
            get => _player.Position;
            set => _player.Position = value;
        }

        public MeshCollider MeshCollider { get; set; }

        public MeshRenderer MeshRenderer { get; set; }

        public Transform Transform { get; set; }
        
        public ReactiveProperty<int> CurrentLives { get; private set; }

        public ObjectTypes PlayerType => _player.PlayerType;
        
        public bool IsDead => _player.IsDead;

        public bool HyperspaceWasTriggered => _player.HyperspaceWasTriggered.Value;
        
        private bool CanCollide => _player.CanCollide;
        
        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);
            _player.JustRespawned = new ReactiveProperty<bool>(false);
            _player.CurrentLives = new ReactiveProperty<int>(2);
            CurrentLives = new ReactiveProperty<int>(2);
        }
        
        private void Start()
        {
            _player.CurrentLives
                .Subscribe(lives => CurrentLives.Value = lives)
                .AddTo(_disposables);

            DisposeIfGameNotRunning();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_player.CanCollide)
            {
                if (other.GetComponent<PlayerFacade>())
                {
                    var otherPlayerFacade = other.GetComponent<PlayerFacade>();
                    
                    if (otherPlayerFacade.CanCollide)
                    {
                        _playerLifecycleHandler.PlayerDeathEvents();
                    }
                } 
                else if (other.GetComponent<BulletProjectile>())
                {
                    var colliderObjectType = other.GetComponent<BulletProjectile>().OriginType;
                
                    if (colliderObjectType != (_player.PlayerType == ObjectTypes.Player
                            ? ObjectTypes.Player
                            : ObjectTypes.OtherPlayer))
                    {
                        _playerLifecycleHandler.PlayerDeathEvents();
                    }
                }
                else
                {
                    _playerLifecycleHandler.PlayerDeathEvents();
                }
            }
        }
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        public class Factory : PlaceholderFactory<ObjectTypes, PlayerFacade>
        {
        }
    }
}
