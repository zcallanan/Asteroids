using System;
using Misc;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerMoveHandler : IInitializable, IFixedTickable
    {
        private readonly Player _player;
        private readonly PlayerInputState _playerInputState;
        private readonly Settings _settings;
        private readonly BoundManager _boundManager;
        
        private Vector3 _currentPosition;
        private Vector3 _previousPosition;
        private Vector3 _facing;
        private float _currentSpeed;
        private float _accelerationRate;
        private float _decelerationRate;
        
        private float _forwardInputValue;

        public PlayerMoveHandler(
            Player player,
            PlayerInputState playerInputState,
            Settings settings,
            BoundManager boundManager)
        {
            _player = player;
            _playerInputState = playerInputState;
            _settings = settings;
            _boundManager = boundManager;
        }
        
        public void Initialize()
        {
            _currentPosition = new Vector3(0, 1f, 0);
            _player.PreviousPosition = _player.Transform.position;
            _facing = _player.Facing;
            
            _player.Position = _currentPosition;
            
            _accelerationRate = _settings.playerMoveSpeedConstant / 2 / _settings.movementModifier;
            _decelerationRate = _settings.playerMoveSpeedConstant / _settings.movementModifier;
        }
        
        public void FixedTick()
        {
            _currentPosition = _player.Transform.position;

            if (_playerInputState.VerticalInput >= 0)
            {
                _forwardInputValue = _playerInputState.VerticalInput;
            }
            
            _currentSpeed = Vector3.Distance(Abs(_player.PreviousPosition), Abs(_currentPosition)) * 100f;
            
            CalculateAdjustedSpeed();
            
            _player.PreviousPosition = _currentPosition;

            MovePlayerShip();
        }

        private void CalculateAdjustedSpeed()
        {
            if (_currentSpeed == 0)
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
                _player.Position = _boundManager.EnforceBounds(_currentPosition);
            }
        }
        
        private Vector3 Abs(Vector3 posVector)
        {
            return new Vector3(Mathf.Abs(posVector.x), posVector.y, Mathf.Abs(posVector.z));
        }
        
        private static float Distance(Vector3 a, Vector3 b)
        {
            Debug.Log(a.x);
            Debug.Log(b.x);
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            Debug.Log($"num1: {num1}, num2: {num2}, num3: {num3}");
            return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2 + (double) num3 * (double) num3);
        }
        
        [Serializable]
        public class Settings
        {
            public float playerMoveSpeedConstant;
            public int movementModifier;
        }
    }
}
