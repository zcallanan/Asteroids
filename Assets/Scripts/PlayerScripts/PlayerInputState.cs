using UniRx;
using Zenject;

namespace PlayerScripts
{
    public class PlayerInputState
    {
        public ReactiveProperty<bool> IsHyperspaceActive { get; set; }
        public ReactiveProperty<bool> IsFiring { get; set; }
    
        public float VerticalInput { get; set; }

        public float HorizontalInput { get; set; }
    }
}
