using System.Collections;
using Models;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player playerPrefab;
        
        private Player _playerInstance;
        private readonly Vector3 _spawnPosition = new Vector3(0, 1.0f, 0);
        private Vector3 _previousPosition;
        private Vector3 _currentPosition;
        private Vector3 _positionPlayerIsFacing;
        
        private float _currentSpeed;
        private float _speedModifier;
        private float _accelerationSpeed;
        private float _decelerationSpeed;
        
        private float _ensureOnlyForwardInput;
        private float _horizontalInput;
        
        private bool _isOpaque;
        private bool _isTogglingTransparency;
        private bool _isHyperspaceInitiated;
        
        private float _alphaChange;
        private MeshRenderer _meshRenderer;
        private Material _material;
        private Color _color;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;

        private void Start()
        {
            _playerInstance = Instantiate(playerPrefab, _spawnPosition, playerPrefab.transform.rotation);
            var playerTransform = _playerInstance.transform;
            _previousPosition = playerTransform.position;
            
            _accelerationSpeed = (_playerInstance.MovementSpeed / 2) / _playerInstance.MovementOverNumberOfFrames;
            _decelerationSpeed = _playerInstance.MovementSpeed / _playerInstance.MovementOverNumberOfFrames;
            
            _ensureOnlyForwardInput = 0;
            _positionPlayerIsFacing = playerTransform.forward;
            _isOpaque = true;
            _alphaChange = _playerInstance.ShipAlphaValue;
            
            DetermineHyperspaceBounds();
            
            GameManager.sharedInstance.PlayerSpawned(_playerInstance);
            GameManager.sharedInstance.OnPlayerDied += HandlePlayerDeath;
            GameManager.sharedInstance.OnScreenSizeChange += DetermineHyperspaceBounds;
        }
        
        private void Update()
        {
            if (_playerInstance && _playerInstance.gameObject.activeSelf)
            {
                _isHyperspaceInitiated = InputController.sharedInstance.IsHyperspaceInitiated;
                
                HyperSpaceTriggered();
            }
        }
        
        private void FixedUpdate()
        {
            if (_playerInstance)
            {
                var verticalInput = InputController.sharedInstance.VerticalInput;
                _horizontalInput = InputController.sharedInstance.HorizontalInput;
                
                _currentPosition = _playerInstance.transform.position;

                if (verticalInput >= 0)
                {
                    _ensureOnlyForwardInput = verticalInput;
                }

                if (verticalInput > 0)
                {
                    GameManager.sharedInstance.PlayerIsApplyingThrust(true, _playerInstance);
                } else if (verticalInput == 0)
                {
                    GameManager.sharedInstance.PlayerIsApplyingThrust(false, _playerInstance);
                }
                
                CalculateMovementSpeedModifier();
                CalculateCurrentSpeed();
                MovePlayerShip();
                RotatePlayerShip();
                
                _previousPosition = _currentPosition;
            }
        }

        private void CalculateCurrentSpeed() {
            _currentPosition = _playerInstance.transform.position;
            _currentSpeed = Vector3.Distance(Abs(_previousPosition), Abs(_currentPosition)) * 100f;
        }

        private void CalculateMovementSpeedModifier()
        {
            if (_currentSpeed == 0)
            {
                _speedModifier = _accelerationSpeed;
                _positionPlayerIsFacing = _playerInstance.transform.forward; 
            }
            else if (_currentSpeed > 0 && _ensureOnlyForwardInput > 0 && _currentSpeed <= _playerInstance.MovementSpeed)
            {
                _speedModifier += _accelerationSpeed;
            } else if (_currentSpeed > 0 && _ensureOnlyForwardInput == 0 && _speedModifier >= 0)
            {
                var temp = _speedModifier - _decelerationSpeed;
                _speedModifier = (temp < 0) ? 0 : temp;
            }
        }

        private void MovePlayerShip()
        {
            if (_ensureOnlyForwardInput > 0 || _ensureOnlyForwardInput == 0 && _currentSpeed > 0 && _speedModifier != 0)
            {
                _playerInstance.transform.position += _positionPlayerIsFacing * (Time.fixedDeltaTime * _speedModifier);
            }
        }

        private void RotatePlayerShip()
        {
            _playerInstance.transform.Rotate(
                _playerInstance.RotationAngle * (_horizontalInput * Time.fixedDeltaTime * _playerInstance.RotationSpeed),
                Space.Self);

        }

        private Vector3 Abs(Vector3 posVector)
        {
            return new Vector3(Mathf.Abs(posVector.x), posVector.y, Mathf.Abs(posVector.z));
        }

        private void DetermineHyperspaceBounds()
        {
            _maxBounds = BoundManager.sharedInstance.MaxBounds;
            _minBounds = BoundManager.sharedInstance.MinBounds;
        }

        private void HyperSpaceTriggered()
        {
            if (_isHyperspaceInitiated && _playerInstance.gameObject.activeSelf)
            {
                _playerInstance.gameObject.SetActive(false);
                _playerInstance.transform.position = DetermineRandomHyperspacePosition();
                
                StartCoroutine(LeaveHyperspaceCoroutine());
                
                GameManager.sharedInstance.HyperspaceWasTriggered(_playerInstance);
            }
        }

        private IEnumerator LeaveHyperspaceCoroutine()
        {
            yield return new WaitForSeconds(_playerInstance.HyperspaceDuration);
            _playerInstance.gameObject.SetActive(true);
            _speedModifier = 0;
            
            GameManager.sharedInstance.HyperspaceIsEnding(_playerInstance);
        }

        private Vector3 DetermineRandomHyperspacePosition()
        {
            return new Vector3(Random.Range(_minBounds.x + 1, _maxBounds.x - 1), 1,
                Random.Range(_minBounds.z + 1, _maxBounds.z - 1));
        }

        private void HandlePlayerDeath(Player playerInstance)
        {
            if (GameManager.sharedInstance.CurrentLives >= 0)
            {
                StartCoroutine(PlayerRespawnCoroutine(playerInstance));
            }
        }

        private IEnumerator PlayerRespawnCoroutine(Player playerRespawnInstance)
        {
            _meshRenderer = playerRespawnInstance.PlayerMeshRenderer;
            _material = _meshRenderer.material;
            _color = _material.color;
            _color.a = _alphaChange;
            _isTogglingTransparency = true;
            
            TogglePlayerTransparency();
            StartCoroutine(TogglePlayerTransparencyCoroutine());
            
            yield return new WaitForSeconds(playerRespawnInstance.RespawnDelay);
            playerRespawnInstance.gameObject.SetActive(true);
            playerRespawnInstance.IsInvulnerable = true;
            playerRespawnInstance.PlayerMeshCollider.enabled = false;
            // playerRespawnInstance.gameObject.layer = 13;
            
            StartCoroutine(RemovePlayerRespawnInvulnerability(playerRespawnInstance));
            
            var playerTransform = playerRespawnInstance.transform;
            _positionPlayerIsFacing = Vector3.zero;
            playerTransform.position = _spawnPosition;
            playerTransform.rotation = Quaternion.identity;
            
            GameManager.sharedInstance.PlayerSpawned(playerRespawnInstance);
        }

        private void TogglePlayerTransparency()
        {
            MaterialMode.SetupBlendMode(_material, _isOpaque ? MaterialMode.BlendMode.Transparent : MaterialMode.BlendMode.Opaque);
            
            _material.color = _color;
            _meshRenderer.material = _material;
            _isOpaque = !_isOpaque;
        }

        private IEnumerator TogglePlayerTransparencyCoroutine()
        {
            yield return new WaitForSeconds(.25f);
            if (_isTogglingTransparency)
            {
                TogglePlayerTransparency();
                StartCoroutine(TogglePlayerTransparencyCoroutine());
            }
        }

        private IEnumerator RemovePlayerRespawnInvulnerability(Player playerRespawnInstance)
        {
            yield return new WaitForSeconds(playerRespawnInstance.InvulnerabilityTimer);
            playerRespawnInstance.IsInvulnerable = false;
            // playerRespawnInstance.gameObject.layer = 8;
            playerRespawnInstance.PlayerMeshCollider.enabled = true;
            _isTogglingTransparency = false;
        }
    }
}
