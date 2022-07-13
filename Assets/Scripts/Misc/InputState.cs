using UniRx;

namespace Misc
{
    public class InputState
    {
        public ReactiveProperty<bool> IsHyperspaceActive { get; set; }
        public ReactiveProperty<bool> IsFiring { get; set; }
        
        public ReactiveProperty<bool> IsApplyingThrust { get; set; }
        
        public ReactiveProperty<float> VerticalInput { get; set; }

        public ReactiveProperty<float> HorizontalInput { get; set; }
        
    }
}
