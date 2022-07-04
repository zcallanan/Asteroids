using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerMoveHandler : IInitializable, IFixedTickable
    {
        private readonly Player _player;
        private readonly PlayerInputState _playerInputState;
        private readonly Settings _settings;
        
        private Vector3 _currentPosition;
        private Vector3 _facing;
        private float _currentSpeed;
        private float _accelerationRate;
        private float _decelerationRate;
        
        private float _forwardInputValue;
        
        public PlayerMoveHandler(
            Player player,
            PlayerInputState playerInputState,
            Settings settings)
        {
            _player = player;
            _playerInputState = playerInputState;
            _settings = settings;
        }
        
        public void Initialize()
        {
            _currentPosition = new Vector3(0, 1f, 0);
            _player.PreviousPosition = _player.Transform.position;
            _facing = _player.Facing;
            
            _player.Position = _currentPosition;
            
            _accelerationRate = _settings.playerMoveSpeedConstant / 2 / _settings.movementModifier;
            _decelerationRate = _settings.playerMoveSpeedConstant / _settings.movementModifier;

            WatchForPlayerDeathOrHyperspace();
        }

        public void FixedTick()
        {
            _currentPosition = _player.Transform.position;

            OnlyRegisterWhenPlayerInputsForwardMovement();

            _currentSpeed = Vector3.Distance(Abs(_player.PreviousPosition), Abs(_currentPosition)) * 100f;
            
            CalculateAdjustedSpeed();
            
            _player.PreviousPosition = _currentPosition;

            MovePlayerShip();
        }

        private void OnlyRegisterWhenPlayerInputsForwardMovement()
        {
            if (_playerInputState.VerticalInput >= 0 && !_player.IsDead && !_player.HyperspaceWasTriggered.Value)
            {
                _forwardInputValue = _playerInputState.VerticalInput;
            }
        }

        private void CalculateAdjustedSpeed()
        {
            if (_currentSpeed == 0 && !_player.IsDead)
            {
                _player.AdjustedSpeed = _accelerationRate;
                _facing = _player.Facing; 
            }
            else if (_currentSpeed > 0 && _forwardInputValue > 0 &&
                     _currentSpeed <= _settings.playerMoveSpeedConstant)
            {
                _player.AdjustedSpeed += _accelerationRate;
            } else if (_currentSpeed > 0 && _forwardInputValue == 0 && _player.AdjustedSpeed >= 0)
            {
                var temp = _player.AdjustedSpeed - _decelerationRate;
                _player.AdjustedSpeed = temp < 0 ? 0 : temp;
            }
        }

        private void MovePlayerShip()
        {
            if (_forwardInputValue > 0 || _forwardInputValue == 0 && _currentSpeed > 0 && _player.AdjustedSpeed != 0)
            {
                _currentPosition += _facing * (Time.fixedDeltaTime * _player.AdjustedSpeed);
                _player.Position = _currentPosition;
            }
        }
        
        private void WatchForPlayerDeathOrHyperspace()
        {
            _player.JustRespawned
                .Subscribe(ResetSpeedFollowingDeathOrHyperspace)
                .AddTo(_player.GameObj);
            
            _player.HyperspaceWasTriggered
                .Subscribe(ResetSpeedFollowingDeathOrHyperspace)
                .AddTo(_player.GameObj);
        }
        
        private void ResetSpeedFollowingDeathOrHyperspace(bool shouldReset)
        {
            if (shouldReset)
            {
                _currentPosition = Vector3.up;
                _player.PreviousPosition = _currentPosition;
            }
        }
        
        private Vector3 Abs(Vector3 posVector)
        {
            return new Vector3(Mathf.Abs(posVector.x), posVector.y, Mathf.Abs(posVector.z));
        }

        [Serializable]
        public class Settings
        {
            public float playerMoveSpeedConstant;
            public int movementModifier;
        }
    }
}
