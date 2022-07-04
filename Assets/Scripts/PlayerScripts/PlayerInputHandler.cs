using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerInputHandler : ITickable
    {
        private readonly PlayerInputState _playerInputState;
        private readonly Player _player;

        public PlayerInputHandler(
            PlayerInputState playerInputState,
            Player player)
        {
            _playerInputState = playerInputState;
            _player = player;
        }

        public void Tick()
        {
            _playerInputState.IsFiring.Value = Input.GetButtonDown("Fire1");
            _playerInputState.IsHyperspaceActive.Value = Input.GetButtonDown("Fire2");

            _playerInputState.VerticalInput = Input.GetAxis("Vertical");
            _playerInputState.HorizontalInput = Input.GetAxis("Horizontal");
            
            _playerInputState.IsApplyingThrust.Value = _playerInputState.VerticalInput > 0 && !_player.IsDead &&
                                                       !_player.HyperspaceWasTriggered.Value;
        }
    }
}
