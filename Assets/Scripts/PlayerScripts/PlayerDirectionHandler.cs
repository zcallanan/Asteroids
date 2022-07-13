using System;
using Misc;
using UnityEngine;
using Zenject;

namespace PlayerScripts
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
            var adjustedAngle = _settings.rotationAngle *
                             (_inputState.HorizontalInput.Value * Time.fixedDeltaTime *
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
