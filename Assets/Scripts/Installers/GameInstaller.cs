using System;
using AsteroidScripts;
using Misc;
using PlayerScripts;
using Ufo;
using UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        Settings _settings;
    
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AsteroidSpawner>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameLevelHandler>().AsSingle();

            Container.Bind<BoundHandler>().FromComponentInHierarchy().AsCached();
            Container.Bind<GameState>().FromComponentInHierarchy().AsCached();
            
            Container.Bind<ScoreHandler>().AsSingle();
            
            Container.Bind<ScoreUI>().FromComponentInHierarchy().AsCached();
            Container.Bind<LivesUI>().FromComponentInHierarchy().AsCached();
            
            Container
                .BindFactory<float, float, BulletProjectileTypes, BulletProjectile, BulletProjectile.Factory>()
                .FromPoolableMemoryPool<float, float, BulletProjectileTypes, BulletProjectile, BulletProjectilePool>(x => x
                    .WithInitialSize(20)
                    .FromComponentInNewPrefab(_settings.bulletProjectilePrefab)
                    .UnderTransformGroup("BulletProjectiles"));
            
            // Container.BindInterfacesAndSelfTo<UfoSpawner>().AsSingle();

            Container
                .BindFactory<int, AsteroidFacade.AsteroidSizes, AsteroidFacade, AsteroidFacade.Factory>()
                .FromPoolableMemoryPool<int, AsteroidFacade.AsteroidSizes, AsteroidFacade, AsteroidFacadePool>(x => x
                    .WithInitialSize(60)
                    .FromSubContainerResolve()
                    .ByNewPrefabInstaller<AsteroidInstaller>(_settings.asteroidPrefab)
                    .UnderTransformGroup("Asteroids"));

            // Container
            //     .BindFactory<UfoFacade, UfoFacade.Factory>()
            //     .FromPoolableMemoryPool<UfoFacade, UfoFacadePool>(x => x
            //         .WithInitialSize(4)
            //         .FromSubContainerResolve()
            //         .ByNewPrefabInstaller<UfoInstaller>(_settings.ufoPrefab)
            //         .UnderTransformGroup("Ufos"));
            //

            //
            // Container
            //     .BindFactory<ExplosionP, ExplosionP.Factory>()
            //     .FromPoolableMemoryPool<ExplosionP, ExplosionPool>(x => x
            //         .WithInitialSize(20)
            //         .FromComponentInNewPrefab(_settings.explosionPrefab)
            //         .UnderTransformGroup("Explosions"));


        }
    
        [Serializable]
        public class Settings
        {
            public GameObject asteroidPrefab;
            public GameObject bulletProjectilePrefab;
            public GameObject ufoPrefab;
            public GameObject explosionPrefab;
            public GameObject boundManagerPrefab;
        }
    
        class BulletProjectilePool : MonoPoolableMemoryPool<float, float, BulletProjectileTypes, IMemoryPool, BulletProjectile>
        {
        }
        class AsteroidFacadePool : MonoPoolableMemoryPool<int, AsteroidFacade.AsteroidSizes, IMemoryPool, AsteroidFacade>
        {
        }
        
        class UfoFacadePool : MonoPoolableMemoryPool<IMemoryPool, UfoFacade>
        {
        }
    
        class ExplosionPool : MonoPoolableMemoryPool<IMemoryPool, ExplosionP>
        {
        }
    }
}