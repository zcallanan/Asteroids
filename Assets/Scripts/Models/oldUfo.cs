// using System.Collections;
// using System.Collections.Generic;
// using Controllers;
// using Data;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace Models
// {
//     public class Ufo : MonoBehaviour
//     {
//         [SerializeField] private UfoData ufoData;
//         
//         public bool IsRecentlySpawned { get; set; }
//         public int UfoSpawnsFromSide { get; set; }
//         public Vector3 DirectionToFace { get; set; }
//         public int UfoSize { get; set; }
//         public List<float> Scales { get; private set; }
//         public List<string> Names { get; private set; }
//         public MeshCollider UfoMeshCollider { get; private set; }
//         
//         private DifficultySettings _difficultySettings;
//         private Ufo _ufo;
//
//         private float _ufoLowerSpeed;
//         private float _ufoUpperSpeed;
//         private float _ufoProjectileCooldownLengthLower;
//         private float _ufoProjectileCooldownLengthUpper;
//         private float _ufoEnforceBoundaryDelayLength;
//         
//         private Coroutine _ufoIsReadyToFireCoroutine;
//         private Coroutine _ufoDisableRecentSpawningCoroutine;
//         
//         private void Awake()
//         {
//             _difficultySettings = GameManager.sharedInstance.difficultySettings;
//             _ufo = this;
//             
//             _ufoLowerSpeed = _difficultySettings.ufoLowerSpeed;
//             _ufoUpperSpeed = _difficultySettings.ufoUpperSpeed;
//
//             Scales = ufoData.scales;
//             Names = ufoData.names;
//
//             _ufoProjectileCooldownLengthLower = ufoData.ufoProjectileCooldownLengthLower;
//             _ufoProjectileCooldownLengthUpper = ufoData.ufoProjectileCooldownLengthUpper;
//             _ufoEnforceBoundaryDelayLength = ufoData.ufoEnforceBoundaryDelayLength;
//
//             UfoMeshCollider = GetComponent<MeshCollider>();
//         }
//
//         private void Update()
//         {
//             var position = transform.position;
//             position +=
//                 DirectionToFace * (Time.deltaTime * Random.Range(_ufoLowerSpeed, _ufoUpperSpeed));
//             
//             if (!IsRecentlySpawned)
//             {
//                 // position = BoundManager.sharedInstance.EnforceBounds(position);
//             }
//             
//             transform.position = position;
//         }
//         
//         private void OnEnable()
//         {
//             _ufoDisableRecentSpawningCoroutine = StartCoroutine(DisableRecentSpawningCoroutine());
//         }
//
//         private void OnDisable()
//         {
//             if (_ufoDisableRecentSpawningCoroutine != null)
//             {
//                 StopCoroutine(_ufoDisableRecentSpawningCoroutine);
//             }
//             
//             if (_ufoIsReadyToFireCoroutine != null)
//             {
//                 StopCoroutine(_ufoIsReadyToFireCoroutine);
//             }
//         }
//
//         // private void OnTriggerEnter(Collider other)
//         // {
//         //     gameObject.SetActive(false);
//         //             
//         //     ParticleController.sharedInstance.HandleUfoExplosion(this);
//         //     ProjectileController.sharedInstance.RemoveDeadUfoFromActiveList(this);
//         //     GameManager.sharedInstance.UpdateScoreUponUfoDeath(UfoSize);
//         // }
//
//         private IEnumerator DisableRecentSpawningCoroutine()
//         {
//             yield return new WaitForSeconds(_ufoEnforceBoundaryDelayLength);
//             IsRecentlySpawned = false;
//             _ufoIsReadyToFireCoroutine = StartCoroutine(TriggerUfoToStartFiring());
//             
//             // ProjectileController.sharedInstance.SpawnUfoProjectile(this);
//         }
//         
//         private IEnumerator TriggerUfoToStartFiring()
//         {
//             if (isActiveAndEnabled)
//             {
//                 yield return new WaitForSeconds(Random.Range(_ufoProjectileCooldownLengthLower, _ufoProjectileCooldownLengthUpper));
//                 // ProjectileController.sharedInstance.SpawnUfoProjectile(this);
//                 
//                 _ufoIsReadyToFireCoroutine = StartCoroutine(TriggerUfoToStartFiring());
//             }
//         }
//     }
// }
