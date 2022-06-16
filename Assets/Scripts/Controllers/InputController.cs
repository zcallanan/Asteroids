using UnityEngine;

namespace Controllers
{
    public class InputController : MonoBehaviour
    {
        public static InputController SharedInstance;
        public float VerticalInput { get; private set; }
        public float HorizontalInput { get; private set; }
        public bool IsHyperspaceInitiated { get; private set; }
        public bool IsFiring { get; private set; }
        void Awake()
        {
            SharedInstance = this;
        }

        void Update()
        {
            VerticalInput = Input.GetAxis("Vertical");
            HorizontalInput = Input.GetAxis("Horizontal");
            IsFiring = Input.GetButtonDown("Fire1");
            IsHyperspaceInitiated = Input.GetButtonDown("Fire2");
        }
    }
}
