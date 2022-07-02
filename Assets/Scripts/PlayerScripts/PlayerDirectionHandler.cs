using System;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerDirectionHandler : IFixedTickable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        private readonly PlayerInputState _playerInputState;

        public PlayerDirectionHandler(
            Player player,
            Settings settings,
            PlayerInputState playerInputState)
        {
            _player = player;
            _settings = settings;
            _playerInputState = playerInputState;
        }

        public void FixedTick()
        {
            var adjustedAngle = _settings.rotationAngle *
                             (_playerInputState.HorizontalInput * Time.fixedDeltaTime *
                              _settings.rotationSpeed);
            
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
