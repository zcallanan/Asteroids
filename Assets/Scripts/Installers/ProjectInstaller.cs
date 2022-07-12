using Misc;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameSceneHandler>().AsSingle();
            Container.Bind<GameState>().AsSingle();
        }
    }
}