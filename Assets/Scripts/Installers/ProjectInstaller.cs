using Misc;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameState>().AsSingle();
            Container.Bind<InputState>().AsSingle();
            
            Container.BindInterfacesTo<InputHandler>().AsSingle();
            Container.BindInterfacesTo<GameSceneHandler>().AsSingle();
        }
    }
}