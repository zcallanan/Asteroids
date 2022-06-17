using System.Numerics;
using Controllers;
using Data;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Models
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        
        public Vector3 PlayerFacing { get; private set; }
        public float MovementSpeed { get; private set; }
        public float MovementOverNumberOfFrames { get; private set; }
        public float RotationSpeed { get; private set; }
        public Vector3 RotationAngle { get; private set; }
        public float RespawnDelay { get; private set; }
        public float InvulnerabilityTimer { get; private set; }
        public bool IsInvulnerable { get; set; }
        public float ShipAlphaValue { get; private set; }
        public float HyperspaceDuration { get; private set; }
        public MeshCollider PlayerMeshCollider { get; private set; }
        public MeshRenderer PlayerMeshRenderer { get; private set; }
        
        private Vector3 _playerPosition;

        private void Awake()
        {
            MovementSpeed = playerData.movementSpeed;
            MovementOverNumberOfFrames = playerData.movementOverNumberOfFrames;
            RotationSpeed = playerData.rotationSpeed;
            RotationAngle = playerData.rotationAngle;
            RespawnDelay = playerData.respawnDelay;
            InvulnerabilityTimer = playerData.invulnerabilityTimer;
            IsInvulnerable = playerData.isInvulnerable;
            ShipAlphaValue = playerData.shipAlphaValue;
            HyperspaceDuration = playerData.hyperspaceDuration;
            PlayerMeshCollider = GetComponent<MeshCollider>();
            PlayerMeshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            _playerPosition = BoundManager.sharedInstance.EnforceBounds(transform.position);
            var playerTransform = transform;
            PlayerFacing = playerTransform.forward;
            playerTransform.position = _playerPosition;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var playerCollider = gameObject.GetComponent<Player>();
            if (!playerCollider.IsInvulnerable)
            {
                GameManager.sharedInstance.PlayerCollided(playerCollider);
                if (other.gameObject.GetComponent<Asteroid>())
                {
                    GameManager.sharedInstance.AsteroidCollided(other.gameObject.GetComponent<Asteroid>());
                }
                other.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}
