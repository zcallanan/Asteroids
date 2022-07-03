using UfoScripts;
using Zenject;

namespace Installers
{
    public class UfoInstaller : Installer<UfoInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UfoExplosionHandler>().AsSingle();
        }
    }
}