using UniRx;
using Zenject;

namespace PlayerScripts
{
    public class PlayerInputState
    {
        // public bool IsHyperspaceActive { get; set; }
        public ReactiveProperty<bool> IsHyperspaceActive { get; set; }
        public bool IsFiring { get; set; }
    
        public float VerticalInput { get; set; }

        public float HorizontalInput { get; set; }

        // public void Initialize()
        // {
        //     IsFiring = new ReactiveProperty<bool>(false);
        //
        //     VerticalInput = new ReactiveProperty<float>(0);
        //     HorizontalInput = new ReactiveProperty<float>(0);
        // }
    }
}
