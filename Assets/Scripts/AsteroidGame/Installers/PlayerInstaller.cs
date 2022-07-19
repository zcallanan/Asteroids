using AsteroidGame.PlayerScripts;
using ProjectScripts;
using Zenject;

namespace AsteroidGame.Installers
{
    public class PlayerInstaller : Installer<PlayerInstaller>
    {
        [Inject] 
        private ObjectTypes _playerType;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Player>().AsSingle();
            Container.BindInstance(_playerType).WhenInjectedInto<Player>();

            Container.BindInterfacesTo<PlayerHyperspaceHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerRespawnEffect>().AsSingle();
            Container.BindInterfacesTo<PlayerFiringHandler>().AsSingle();
            
            Container.BindInterfacesTo<PlayerExplosionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerThrustHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerMoveHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerLifecycleHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerCollisionHandler>().AsSingle();
        }
    }
}