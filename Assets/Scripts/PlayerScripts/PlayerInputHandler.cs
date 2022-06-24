using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerInputHandler : ITickable
    {
        private readonly PlayerInputState _playerInputState;

        public PlayerInputHandler(PlayerInputState playerInputState)
        {
            _playerInputState = playerInputState;
        }

        public void Tick()
        {
            _playerInputState.IsFiring.Value = Input.GetButtonDown("Fire1");
            _playerInputState.IsHyperspaceActive.Value = Input.GetButtonDown("Fire2");
        
            _playerInputState.VerticalInput = Input.GetAxis("Vertical");
            _playerInputState.HorizontalInput = Input.GetAxis("Horizontal");
        }
    }
}
