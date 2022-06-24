using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        PlayerInputState _playerInputState;

        [Inject]
        public void Construct(PlayerInputState playerInputState)
        {
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
        }

        private void Update()
        {
            _playerInputState.IsFiring = Input.GetButtonDown("Fire1");
            _playerInputState.IsHyperspaceActive.Value = Input.GetButtonDown("Fire2");
        
            _playerInputState.VerticalInput = Input.GetAxis("Vertical");
            _playerInputState.HorizontalInput = Input.GetAxis("Horizontal");
        }
    }
}
