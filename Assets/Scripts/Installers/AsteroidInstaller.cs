using AsteroidScripts;
using Misc;
using Zenject;

namespace Installers
{
    public class AsteroidInstaller : Installer<AsteroidInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AsteroidExplosionHandler>().AsSingle();
        }
    }
}