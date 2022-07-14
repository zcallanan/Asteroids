using UnityEngine;
using Zenject;

namespace ProjectScripts
{
    public class InputHandler : ITickable
    {
        private readonly InputState _inputState;
        
        public InputHandler(
            InputState inputState)
        {
            _inputState = inputState;
        }

        public void Tick()
        {
            _inputState.IsFiring.Value = Input.GetButtonDown("Fire1");
            _inputState.IsHyperspaceActive.Value = Input.GetButtonDown("Fire2");

            _inputState.VerticalInput.Value = Input.GetAxis("Vertical");
            _inputState.HorizontalInput.Value = Input.GetAxis("Horizontal");
        }
    }
}
