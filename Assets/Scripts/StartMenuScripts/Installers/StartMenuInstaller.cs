using StartMenuScripts.Misc;
using StartMenuScripts.StartMenu;
using Zenject;

namespace StartMenuScripts.Installers
{
    public class StartMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StartMenuState>().AsSingle();
            Container.BindInterfacesTo<StartMenuHandler>().AsSingle();
            
            Container.BindInterfacesTo<SetupAsteroidData>().AsSingle();
            Container.BindInterfacesTo<LoadAsteroidScene>().AsSingle();
        }
    }
}