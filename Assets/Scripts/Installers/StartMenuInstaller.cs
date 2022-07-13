using Misc;
using StartMenu;
using Zenject;

namespace Installers
{
    public class StartMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StartMenuState>().AsSingle();
            Container.BindInterfacesTo<StartMenuHandler>().AsSingle();
        }
    }
}