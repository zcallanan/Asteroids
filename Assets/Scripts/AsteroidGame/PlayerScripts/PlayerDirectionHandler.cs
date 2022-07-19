using System;
using ProjectScripts;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerDirectionHandler : IFixedTickable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        private readonly InputState _inputState;
        private readonly GameState _gameState;

        public PlayerDirectionHandler(
            Player player,
            Settings settings,
            InputState inputState,
            GameState gameState)
        {
            _player = player;
            _settings = settings;
            _inputState = inputState;
            _gameState = gameState;
        }

        public void FixedTick()
        {
            if (_gameState.IsGameRunning.Value)
            {
                RotatePlayer();
            }
        }

        private void RotatePlayer()
        {
            var horizontalInput = _player.PlayerType == ObjectTypes.Player
                ? _inputState.HorizontalInput.Value
                : _inputState.HorizontalInput2.Value;
            
            var adjustedAngle = _settings.rotationAngle *
                                (horizontalInput * Time.fixedDeltaTime * _settings.rotationSpeed);
            
            _player.SetRotation(adjustedAngle);
        }
        
        [Serializable]
        public class Settings
        {
            public Vector3 rotationAngle;
            public float rotationSpeed;
        }
    }
}
