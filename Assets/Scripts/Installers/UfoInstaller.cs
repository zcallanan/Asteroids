using Ufo;
using Zenject;

namespace Installers
{
    public class UfoInstaller : Installer<UfoInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<UfoFacade>().AsSingle();
        }
    }
}