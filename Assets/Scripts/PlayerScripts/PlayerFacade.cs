using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;
        private PlayerInputState _playerInputState;

        public MeshCollider MeshCollider => _player.MeshCollider;
        public MeshRenderer MeshRenderer => _player.MeshRenderer;

        public Vector3 Facing => _player.Facing;

        public Vector3 Position => _player.Position;
        
        public ReactiveProperty<int> CurrentLives { get; private set; }
        
        [Inject]
        public void Construct(Player player, PlayerInputState playerInputState)
        {
            _player = player;
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _playerInputState.IsFiring = new ReactiveProperty<bool>(false);
            CurrentLives = new ReactiveProperty<int>(0);
        }

        private void Start()
        {
            _player.CurrentLives.Subscribe(lives => CurrentLives.Value = lives);
            
            if (gameObject.GetComponent<ObservableTriggerTrigger>() == null)
            {
                gameObject
                    .AddComponent<ObservableTriggerTrigger>()
                    .UpdateAsObservable()
                    .SampleFrame(60)
                    .Subscribe(_ => Debug.Log($"Observable added"));
            }
        }
    }
}
