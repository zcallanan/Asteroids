using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;

namespace AsteroidGame.PlayerScripts
{
    public class Player
    {
        private readonly PlayerFacade _playerFacade;
        private readonly BoundHandler _boundHandler;

        public Player(
            PlayerFacade playerFacade,
            BoundHandler boundHandler,
            ObjectTypes playerType)
        {
            _playerFacade = playerFacade;
            _boundHandler = boundHandler;
            PlayerType = playerType;
        }
        
        public ReactiveProperty<int> CurrentLives { get; set; }
        
        public ObjectTypes PlayerType { get; }

        public MeshCollider MeshCollider => _playerFacade.MeshCollider;

        public MeshRenderer MeshRenderer => _playerFacade.MeshRenderer;

        public Transform Transform => _playerFacade.Transform;

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

        public float AdjustedSpeed { get; set; }

        public bool IsDead { get; set; }
        
        public bool CanCollide { get; set; }

        public ReactiveProperty<bool> JustRespawned { get; set; }
        
        public ReactiveProperty<bool> HyperspaceWasTriggered { get; set; }

        public GameObject GameObj => _playerFacade.gameObject;

        public void SetRotation(Vector3 rot)
        {
            Transform.Rotate(rot, Space.Self);
        }
    }
}
