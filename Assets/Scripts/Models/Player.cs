// using Controllers;
// using Data;
// using UniRx;
// using UnityEngine;
// using Vector3 = UnityEngine.Vector3;
//
// namespace Models
// {
//     public class Player : MonoBehaviour
//     {
//         [SerializeField] private PlayerData playerData;
//         
//         public ReactiveProperty<bool> IsApplyingThrust { get; private set; }
//         public ReactiveProperty<bool> IsHyperspaceActive { get; private set; }
//
//         public Vector3 PlayerFacing { get; private set; }
//         public float MovementSpeed { get; private set; }
//         public float MovementOverNumberOfFrames { get; private set; }
//         public float RotationSpeed { get; private set; }
//         public Vector3 RotationAngle { get; private set; }
//         
//         public float RespawnDelay { get; private set; }
//         public float InvulnerabilityTimer { get; private set; }
//         public float ShipAlphaValue { get; private set; }
//         public float HyperspaceDuration { get; private set; }
//         
//         public MeshCollider PlayerMeshCollider { get; private set; }
//         public MeshRenderer PlayerMeshRenderer { get; private set; }
//         
//         private Vector3 _playerPosition;
//         
//         private void Awake()
//         {
//             ProjectileController.sharedInstance.PlayerInstance.Value = this;
//             ParticleController.sharedInstance.PlayerInstance.Value = this;
//             
//             IsApplyingThrust = new ReactiveProperty<bool>(false);
//             IsHyperspaceActive = new ReactiveProperty<bool>(false);
//             
//             MovementSpeed = playerData.movementSpeed;
//             MovementOverNumberOfFrames = playerData.movementOverNumberOfFrames;
//             RotationSpeed = playerData.rotationSpeed;
//             RotationAngle = playerData.rotationAngle;
//             
//             RespawnDelay = playerData.respawnDelay;
//             InvulnerabilityTimer = playerData.invulnerabilityTimer;
//             ShipAlphaValue = playerData.shipAlphaValue;
//             HyperspaceDuration = playerData.hyperspaceDuration;
//             
//             PlayerMeshCollider = GetComponent<MeshCollider>();
//             PlayerMeshRenderer = GetComponent<MeshRenderer>();
//         }
//
//         private void Update()
//         {
//             // _playerPosition = BoundManager.sharedInstance.EnforceBounds(transform.position);
//             
//             var playerTransform = transform;
//             PlayerFacing = playerTransform.forward;
//             playerTransform.position = _playerPosition;
//         }
//
//         private void OnTriggerEnter(Collider other)
//         {
//             gameObject.SetActive(false);
//
//             ProjectileController.sharedInstance.PreventFiring();
//             ParticleController.sharedInstance.HandlePlayerExplosion(this);
//             GameManager.sharedInstance.DecrementLivesAndCheckForGameOver();
//         }
//     }
// }
