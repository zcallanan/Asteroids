using UniRx;

namespace StartMenu
{
    public class StartMenuState
    {
        public ReactiveProperty<bool> MenuFocus { get; set; }
        public ReactiveProperty<bool> IsStartScreenInit { get; set; }
    }
}
