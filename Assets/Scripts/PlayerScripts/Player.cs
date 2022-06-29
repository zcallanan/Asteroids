using Misc;
using UniRx;
using UnityEngine;

namespace PlayerScripts
{
    public class Player
    {
        private readonly Transform _transform;
        private readonly PlayerFacade _playerFacade;
        private readonly BoundHandler _boundHandler;
        
        public Player(
            MeshRenderer meshRenderer,
            MeshCollider meshCollider,
            Transform transform,
            PlayerFacade playerFacade,
            BoundHandler boundHandler)
        {
            MeshRenderer = meshRenderer;
            MeshCollider = meshCollider;
            _transform = transform;
            _playerFacade = playerFacade;
            _boundHandler = boundHandler;
        }
        public MeshCollider MeshCollider { get; }

        public MeshRenderer MeshRenderer { get; }

        public Vector3 Facing
        {
            get => _transform.forward;
            set => _transform.forward = value;
        }

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = _boundHandler.EnforceBounds(value);
        }
        
        public Vector3 PreviousPosition { get; set; }

        public Transform Transform => _transform;
        
        public Vector3 Rotation
        {
            set => _transform.Rotate(value, Space.Self);
        }
        
        public float AdjustedSpeed { get; set; }

        public bool IsDead { get; set; }
        
        public ReactiveProperty<bool> JustRespawned { get; set; }

        public GameObject GameObj => _playerFacade.gameObject;
    }
}
