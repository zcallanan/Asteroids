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
        
        public ReactiveProperty<int> CurrentLives { get; private set; }
        
        public ReactiveProperty<int> Score { get; private set; }

        public Vector3 Position
        {
            get => _player.Position;
            set => _player.Position = value;
        }

        public MeshCollider MeshCollider { get; set; }

        public MeshRenderer MeshRenderer { get; set; }

        public Transform Transform { get; set; }

        public ObjectTypes PlayerType => _player.PlayerType;
        
        public bool IsDead => _player.IsDead;

        public bool HyperspaceWasTriggered => _player.HyperspaceWasTriggered.Value;
        
        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);
            _player.JustRespawned = new ReactiveProperty<bool>(false);
            _player.CurrentLives = new ReactiveProperty<int>(2);
            _player.Score = new ReactiveProperty<int>(0);
            
            CurrentLives = new ReactiveProperty<int>(2);
            Score = new ReactiveProperty<int>(0);
        }
        
        private void Start()
        {
            _player.CurrentLives
                .Subscribe(lives => CurrentLives.Value = lives)
                .AddTo(_disposables);
            
            _player.Score
                .Subscribe(score => Score.Value = score)
                .AddTo(_disposables);

            DisposeIfGameNotRunning();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerFacade>() && _gameState.GameMode.Value != 2)
            {
                _playerLifecycleHandler.PlayerDeathEvents();
            } 
            else if (other.GetComponent<BulletProjectile>())
            {
                var originType = other.GetComponent<BulletProjectile>().OriginType;

                if (originType != BulletFromPlayer() && !IsBulletFromTeammate(originType))
                {
                    _playerLifecycleHandler.PlayerDeathEvents();
                }
            }
            else
            {
                _playerLifecycleHandler.PlayerDeathEvents();
            }
        }

        private bool IsBulletFromTeammate(ObjectTypes originType)
        {
            return _gameState.GameMode.Value == 2 && originType == ObjectTypes.Player ||
                   originType == ObjectTypes.OtherPlayer;
        }

        private ObjectTypes BulletFromPlayer()
        {
            return _player.PlayerType == ObjectTypes.Player
                ? ObjectTypes.Player
                : ObjectTypes.OtherPlayer;
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
