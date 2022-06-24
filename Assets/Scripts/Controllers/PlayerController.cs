// using System.Collections;
// using Models;
// using UniRx;
// using UniRx.Triggers;
// using UnityEngine;
//
// namespace Controllers
// {
//     public class PlayerController : MonoBehaviour
//     {
//         [SerializeField] private Player playerPrefab;
//         [SerializeField] private Material transparentMaterial;
//         
//         private Player _playerInstance;
//         private readonly Vector3 _spawnPosition = new Vector3(0, 1.0f, 0);
//         private Vector3 _previousPosition;
//         private Vector3 _currentPosition;
//         private Vector3 _positionPlayerIsFacing;
//         
//         private float _currentSpeed;
//         private float _speedModifier;
//         private float _accelerationSpeed;
//         private float _decelerationSpeed;
//         
//         private float _ensureOnlyForwardInput;
//         private float _horizontalInput;
//         
//         private bool _isTogglingTransparency;
//         private bool _isHyperspaceInitiated;
//         
//         private MeshRenderer _meshRenderer;
//         private Material _material;
//
//         private Vector3 _maxBounds;
//         private Vector3 _minBounds;
//         
//         private readonly CompositeDisposable _disposables = new CompositeDisposable();
//
//         private void Start()
//         {
//             // _playerInstance = Instantiate(playerPrefab, _spawnPosition, playerPrefab.transform.rotation);
//             
//             var playerTransform = _playerInstance.transform;
//             _previousPosition = playerTransform.position;
//             
//             _accelerationSpeed = (_playerInstance.MovementSpeed / 2) / _playerInstance.MovementOverNumberOfFrames;
//             _decelerationSpeed = _playerInstance.MovementSpeed / _playerInstance.MovementOverNumberOfFrames;
//             
//             // _ensureOnlyForwardInput = 0;
//             _positionPlayerIsFacing = playerTransform.forward;
//
//             // DetermineHyperspaceBounds();
//             
//             GameManager.sharedInstance.IsGameOver
//                 .Subscribe(HandleGameOver)
//                 .AddTo(_disposables);
//             
//             // GameManager.sharedInstance.LatestScreenSize
//             //     .Subscribe(unit => DetermineHyperspaceBounds())
//             //     .AddTo(_disposables);
//             //
//             _playerInstance.gameObject
//                 .OnTriggerEnterAsObservable()
//                 .Subscribe(y => HandlePlayerDeath(_playerInstance))
//                 .AddTo(_disposables);
//         }
//         
//         private void Update()
//         {
//             if (_playerInstance && _playerInstance.gameObject.activeSelf)
//             {
//                 _isHyperspaceInitiated = InputController.sharedInstance.IsHyperspaceInitiated;
//                 
//                 // HyperSpaceTriggered();
//             }
//         }
//         
//         private void FixedUpdate()
//         {
//             if (_playerInstance)
//             {
//                 // var verticalInput = InputController.sharedInstance.VerticalInput;
//                 // _horizontalInput = InputController.sharedInstance.HorizontalInput;
//                 //
//                 // _currentPosition = _playerInstance.transform.position;
//                 //
//                 // if (verticalInput >= 0)
//                 // {
//                 //     _ensureOnlyForwardInput = verticalInput;
//                 // }
//                 //
//                 // if (verticalInput > 0)
//                 // {
//                 //     _playerInstance.IsApplyingThrust.Value = true;
//                 // } else if (verticalInput == 0)
//                 // {
//                 //     _playerInstance.IsApplyingThrust.Value = false;
//                 // }
//                 //
//                 // CalculateMovementSpeedModifier();
//                 // CalculateCurrentSpeed();
//                 // MovePlayerShip();
//                 // // RotatePlayerShip();
//                 //
//                 // _previousPosition = _currentPosition;
//             }
//         }
//
//         private void HandleGameOver(bool isGameOver)
//         {
//             if (isGameOver)
//             {
//                 _disposables.Clear();
//             }
//         }
//         
//         private void HandlePlayerDeath(Player playerInstance)
//         {
//             if (GameManager.sharedInstance.CurrentLives.Value >= 0)
//             {
//                 StartCoroutine(PlayerRespawnCoroutine(playerInstance));
//             }
//         }
//
//         // private void CalculateCurrentSpeed() {
//         //     _currentPosition = _playerInstance.transform.position;
//         //     _currentSpeed = Vector3.Distance(Abs(_previousPosition), Abs(_currentPosition)) * 100f;
//         // }
//         //
//         // private void CalculateMovementSpeedModifier()
//         // {
//         //     if (_currentSpeed == 0)
//         //     {
//         //         _speedModifier = _accelerationSpeed;
//         //         _positionPlayerIsFacing = _playerInstance.transform.forward; 
//         //     }
//         //     else if (_currentSpeed > 0 && _ensureOnlyForwardInput > 0 && _currentSpeed <= _playerInstance.MovementSpeed)
//         //     {
//         //         _speedModifier += _accelerationSpeed;
//         //     } else if (_currentSpeed > 0 && _ensureOnlyForwardInput == 0 && _speedModifier >= 0)
//         //     {
//         //         var temp = _speedModifier - _decelerationSpeed;
//         //         _speedModifier = (temp < 0) ? 0 : temp;
//         //     }
//         // }
//         //
//         // private void MovePlayerShip()
//         // {
//         //     if (_ensureOnlyForwardInput > 0 || _ensureOnlyForwardInput == 0 && _currentSpeed > 0 && _speedModifier != 0)
//         //     {
//         //         _playerInstance.transform.position += _positionPlayerIsFacing * (Time.fixedDeltaTime * _speedModifier);
//         //     }
//         // }
//         //
//         // // private void RotatePlayerShip()
//         // // {
//         // //     _playerInstance.transform.Rotate(
//         // //         _playerInstance.RotationAngle * (_horizontalInput * Time.fixedDeltaTime * _playerInstance.RotationSpeed),
//         // //         Space.Self);
//         // //
//         // // }
//         //
//         // private Vector3 Abs(Vector3 posVector)
//         // {
//         //     return new Vector3(Mathf.Abs(posVector.x), posVector.y, Mathf.Abs(posVector.z));
//         // }
//
//         // private void DetermineHyperspaceBounds()
//         // {
//         //     _maxBounds = BoundManager.sharedInstance.MaxBounds;
//         //     _minBounds = BoundManager.sharedInstance.MinBounds;
//         // }
//         //
//         // private void HyperSpaceTriggered()
//         // {
//         //     if (_isHyperspaceInitiated && _playerInstance.gameObject.activeSelf)
//         //     {
//         //         _playerInstance.gameObject.SetActive(false);
//         //         _playerInstance.transform.position = DetermineRandomHyperspacePosition();
//         //         
//         //         StartCoroutine(LeaveHyperspaceCoroutine());
//         //
//         //         _playerInstance.IsHyperspaceActive.Value = true;
//         //     }
//         // }
//         //
//         // private IEnumerator LeaveHyperspaceCoroutine()
//         // {
//         //     yield return new WaitForSeconds(_playerInstance.HyperspaceDuration);
//         //     _playerInstance.gameObject.SetActive(true);
//         //     _speedModifier = 0;
//         //     
//         //     _playerInstance.IsHyperspaceActive.Value = false;
//         // }
//         //
//         // private Vector3 DetermineRandomHyperspacePosition()
//         // {
//         //     return new Vector3(Random.Range(_minBounds.x + 1, _maxBounds.x - 1), 1,
//         //         Random.Range(_minBounds.z + 1, _maxBounds.z - 1));
//         // }
//
//         private IEnumerator PlayerRespawnCoroutine(Player playerRespawnInstance)
//         {
//             _meshRenderer = playerRespawnInstance.PlayerMeshRenderer;
//             _material = _meshRenderer.material;
//             
//             _isTogglingTransparency = true;
//             
//             TogglePlayerTransparency();
//             StartCoroutine(TogglePlayerTransparencyCoroutine());
//             
//             yield return new WaitForSeconds(playerRespawnInstance.RespawnDelay);
//             playerRespawnInstance.gameObject.SetActive(true);
//             playerRespawnInstance.PlayerMeshCollider.enabled = false;
//             
//             StartCoroutine(RemovePlayerRespawnInvulnerability(playerRespawnInstance));
//             
//             var playerTransform = playerRespawnInstance.transform;
//             _positionPlayerIsFacing = Vector3.zero;
//             playerTransform.position = _spawnPosition;
//             playerTransform.rotation = Quaternion.identity;
//             
//             // GameManager.sharedInstance.PlayerSpawned(playerRespawnInstance);
//         }
//
//         private void TogglePlayerTransparency()
//         {
//             _meshRenderer.material = _meshRenderer.material == _material ? transparentMaterial : _material;
//         }
//
//         private IEnumerator TogglePlayerTransparencyCoroutine()
//         {
//             yield return new WaitForSeconds(.25f);
//             if (_isTogglingTransparency)
//             {
//                 TogglePlayerTransparency();
//                 StartCoroutine(TogglePlayerTransparencyCoroutine());
//             }
//         }
//
//         private IEnumerator RemovePlayerRespawnInvulnerability(Player playerRespawnInstance)
//         {
//             yield return new WaitForSeconds(playerRespawnInstance.InvulnerabilityTimer);
//             playerRespawnInstance.PlayerMeshCollider.enabled = true;
//             _isTogglingTransparency = false;
//             _meshRenderer.material = _material;
//         }
//     }
// }
