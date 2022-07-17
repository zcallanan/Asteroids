using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;

namespace AsteroidGame.PlayerScripts
{
    public class Player
    {
        private readonly PlayerFacade _playerFacade;
        private readonly OtherPlayerFacade _otherPlayerFacade;
        private readonly BoundHandler _boundHandler;

        public Player(
            MeshRenderer meshRenderer,
            MeshCollider meshCollider,
            Transform transform,
            PlayerFacade playerFacade,
            OtherPlayerFacade otherPlayerFacade,
            BoundHandler boundHandler,
            ObjectTypes playerType)
        {
            MeshRenderer = meshRenderer;
            MeshCollider = meshCollider;
            Transform = transform;
            _playerFacade = playerFacade;
            _otherPlayerFacade = otherPlayerFacade;
            _boundHandler = boundHandler;
            PlayerType = playerType;
        }
        
        public ObjectTypes PlayerType { get; }
        
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

        public float AdjustedSpeed { get; set; }

        public bool IsDead { get; set; }
        
        public bool CanCollide { get; set; }

        public ReactiveProperty<bool> JustRespawned { get; set; }
        
        public ReactiveProperty<bool> HyperspaceWasTriggered { get; set; }

        public GameObject GameObj
        {
            get
            {
                var result = PlayerType == ObjectTypes.Player ? _playerFacade.gameObject : _otherPlayerFacade.gameObject;

                return result;
            }
        }

        public void SetRotation(Vector3 rot)
        {
            Transform.Rotate(rot, Space.Self);
        }
    }
}
