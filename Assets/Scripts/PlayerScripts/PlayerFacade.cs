using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;
        private GameState _gameState;
        private PlayerInputState _playerInputState;

        public MeshCollider MeshCollider => _player.MeshCollider;
        public MeshRenderer MeshRenderer => _player.MeshRenderer;

        public Vector3 Facing => _player.Facing;

        public Vector3 Position => _player.Position;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        
        [Inject]
        public void Construct(
            Player player,
            GameState gameState,
            PlayerInputState playerInputState)
        {
            _player = player;
            _gameState = gameState;
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _playerInputState.IsFiring = new ReactiveProperty<bool>(false);
        }

        private void Start()
        {
            if (gameObject.GetComponent<ObservableTriggerTrigger>() == null)
            {
                gameObject
                    .AddComponent<ObservableTriggerTrigger>()
                    .UpdateAsObservable()
                    .SampleFrame(60)
                    .Subscribe(_ => Debug.Log($"Observable added"))
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
    }
}
