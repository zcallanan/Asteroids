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

        public PlayerDirectionHandler(
            Player player,
            Settings settings,
            InputState inputState)
        {
            _player = player;
            _settings = settings;
            _inputState = inputState;
        }

        public void FixedTick()
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
