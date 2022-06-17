using UnityEngine;

namespace Controllers
{
    public class InputController : MonoBehaviour
    {
        public static InputController sharedInstance;
        public float VerticalInput { get; private set; }
        public float HorizontalInput { get; private set; }
        public bool IsHyperspaceInitiated { get; private set; }
        public bool IsFiring { get; private set; }

        private void Awake()
        {
            sharedInstance = this;
        }

        private void Update()
        {
            VerticalInput = Input.GetAxis("Vertical");
            HorizontalInput = Input.GetAxis("Horizontal");
            IsFiring = Input.GetButtonDown("Fire1");
            IsHyperspaceInitiated = Input.GetButtonDown("Fire2");
        }
    }
}
