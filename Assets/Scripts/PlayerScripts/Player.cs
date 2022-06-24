using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class Player
    {
        private readonly MeshFilter _meshFilter;
        private readonly Rigidbody _rigidBody;
        private readonly Transform _transform;
        
        public Player(
            MeshRenderer meshRenderer,
            MeshFilter meshFilter,
            MeshCollider meshCollider,
            Rigidbody rigidBody,
            Transform transform
            )
        {
            MeshRenderer = meshRenderer;
            _meshFilter = meshFilter;
            MeshCollider = meshCollider;
            _rigidBody = rigidBody;
            _transform = transform;
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
            set => _transform.position = value;
        }
        
        public Vector3 PreviousPosition { get; set; }

        public Transform Transform => _transform;
        
        public Vector3 Rotation
        {
            set => _transform.Rotate(value, Space.Self);
        }

        public float AdjustedSpeed { get; set; }

        public int CurrentLives { get; set; }
        
        public ReactiveProperty<bool> JustRespawned { get; set; }
    }
}
