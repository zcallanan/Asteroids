using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;
        private InputState _inputState;

        public Vector3 Position => _player.Position;

        public bool IsDead => _player.IsDead;

        public bool HyperspaceWasTriggered => _player.HyperspaceWasTriggered.Value;
        
        [Inject]
        public void Construct(
            Player player,
            InputState inputState)
        {
            _player = player;
            _inputState = inputState;
        }

        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);
        }
    }
}
