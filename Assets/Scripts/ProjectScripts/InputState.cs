using UniRx;

namespace ProjectScripts
{
    public class InputState
    {
        public ReactiveProperty<bool> IsHyperspaceActive { get; set; }
        
        public ReactiveProperty<bool> IsFiring { get; set; }
        
        public ReactiveProperty<bool> IsApplyingThrust { get; set; }
        
        public ReactiveProperty<float> VerticalInput { get; set; }

        public ReactiveProperty<float> HorizontalInput { get; set; }
        
        public ReactiveProperty<bool> IsHyperspaceActive2 { get; set; }
        
        public ReactiveProperty<bool> IsFiring2 { get; set; }
        
        public ReactiveProperty<bool> IsApplyingThrust2 { get; set; }
        
        public ReactiveProperty<float> VerticalInput2 { get; set; }

        public ReactiveProperty<float> HorizontalInput2 { get; set; }
        
    }
}
