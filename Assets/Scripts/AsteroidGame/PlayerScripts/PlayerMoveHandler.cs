using System;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerMoveHandler : IInitializable, IFixedTickable
    {
        private readonly Player _player;
        private readonly InputState _inputState;
        private readonly Settings _settings;
        private readonly GameState _gameState;
        
        private Vector3 _currentPosition;
        private Vector3 _facing;
        private float _currentSpeed;
        private float _accelerationRate;
        private float _decelerationRate;
        
        private float _forwardInputValue;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerMoveHandler(
            Player player,
            InputState inputState,
            Settings settings,
            GameState gameState)
        {
            _player = player;
            _inputState = inputState;
            _settings = settings;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                CheckIfPlayersSpawned();

                DisposeIfGameNotRunning();
            }
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
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void CheckIfPlayersSpawned()
        {
            _gameState.ArePlayersSpawned
                .Subscribe(playersSpawned =>
                {
                    if (playersSpawned)
                    {
                        InitializeMoveHandler();
                    }
                })
                .AddTo(_disposables);
        }

        private void InitializeMoveHandler()
        {
            _currentPosition = _player.Position;
            _player.PreviousPosition = _player.Transform.position;
            _facing = _player.Facing;
            
            _accelerationRate = _settings.playerMoveSpeedConstant / 2 / _settings.movementModifier;
            _decelerationRate = _settings.playerMoveSpeedConstant / _settings.movementModifier;

            WatchForPlayerDeathOrHyperspace();
        }

        private void OnlyRegisterWhenPlayerInputsForwardMovement()
        {
            var verticalInput = _player.PlayerType == ObjectTypes.Player
                ? _inputState.VerticalInput.Value
                : _inputState.VerticalInput2.Value;
            
            if ( verticalInput >= 0 && !_player.IsDead &&
                 !_player.HyperspaceWasTriggered.Value)
            {
                _forwardInputValue = verticalInput;
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
                .AddTo(_disposables);
            
            _player.HyperspaceWasTriggered
                .Subscribe(ResetSpeedFollowingDeathOrHyperspace)
                .AddTo(_disposables);
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
