using AsteroidGame.AsteroidScripts;
using Zenject;

namespace AsteroidGame.Installers
{
    public class AsteroidInstaller : Installer<AsteroidInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AsteroidExplosionHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<AsteroidCollisionHandler>().AsSingle();
        }
    }
}