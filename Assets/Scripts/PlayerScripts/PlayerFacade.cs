using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;
        private PlayerInputState _playerInputState;
        
        [Inject]
        public void Construct(
            Player player,
            PlayerInputState playerInputState)
        {
            _player = player;
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);

            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _playerInputState.IsFiring = new ReactiveProperty<bool>(false);

            _playerInputState.IsApplyingThrust = new ReactiveProperty<bool>(false);
        }
    }
}
