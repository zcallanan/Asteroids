using Zenject;

namespace ProjectScripts.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameState>().AsSingle();
            Container.Bind<InputState>().AsSingle();
            
            Container.BindInterfacesTo<InputHandler>().AsSingle();
        }
    }
}