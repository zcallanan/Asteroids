using Misc;
using UniRx;
using UnityEngine;

namespace PlayerScripts
{
    public class Player
    {
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
            Transform = transform;
            _playerFacade = playerFacade;
            _boundHandler = boundHandler;
        }
        public MeshCollider MeshCollider { get; }

        public MeshRenderer MeshRenderer { get; }

        public Vector3 Facing
        {
            get => Transform.forward;
            set => Transform.forward = value;
        }

        public Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = _boundHandler.EnforceBounds(value);
        }
        
        public Vector3 PreviousPosition { get; set; }
        
        public Transform Transform { get; }

        public Vector3 Rotation
        {
            set => Transform.Rotate(value, Space.Self);
        }
        
        public float AdjustedSpeed { get; set; }

        public bool IsDead { get; set; }
        
        public ReactiveProperty<bool> JustRespawned { get; set; }
        
        public ReactiveProperty<bool> HyperspaceWasTriggered { get; set; }

        public GameObject GameObj => _playerFacade.gameObject;
    }
}
