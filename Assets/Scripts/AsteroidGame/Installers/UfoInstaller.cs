using AsteroidGame.UfoScripts;
using Zenject;

namespace AsteroidGame.Installers
{
    public class UfoInstaller : Installer<UfoInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UfoExplosionHandler>().AsSingle();
            Container.BindInterfacesTo<UfoCollisionHandler>().AsSingle();
            Container.BindInterfacesTo<UfoFiringHandler>().AsSingle();
        }
    }
}