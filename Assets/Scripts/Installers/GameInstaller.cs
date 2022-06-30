using System;
using AsteroidScripts;
using Misc;
using UfoScripts;
using UI;
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
            Container.Bind<GameOverUI>().FromComponentInHierarchy().AsCached();

            Container
                .BindFactory<float, float, BulletProjectileTypes, BulletProjectile, BulletProjectile.Factory>()
                .FromPoolableMemoryPool<float, float, BulletProjectileTypes, BulletProjectile, BulletProjectilePool>(x => x
                    .WithInitialSize(20)
                    .FromComponentInNewPrefab(_settings.bulletProjectilePrefab)
                    .UnderTransformGroup("BulletProjectiles"));
            
            // Container.BindInterfacesAndSelfTo<UfoSpawner>().AsSingle();

            Container
                .BindFactory<int, Asteroid.AsteroidSizes, Asteroid, Asteroid.Factory>()
                .FromPoolableMemoryPool<int, Asteroid.AsteroidSizes, Asteroid, AsteroidPool>(x => x
                    .WithInitialSize(60)
                    .FromSubContainerResolve()
                    .ByNewPrefabInstaller<AsteroidInstaller>(_settings.asteroidPrefab)
                    .UnderTransformGroup("Asteroids"));

            // Container
            //     .BindFactory<Ufo, Ufo.Factory>()
            //     .FromPoolableMemoryPool<Ufo, UfoPool>(x => x
            //         .WithInitialSize(4)
            //         .FromSubContainerResolve()
            //         .ByNewPrefabInstaller<UfoInstaller>(_settings.ufoPrefab)
            //         .UnderTransformGroup("Ufos"));
            //

            
            Container
                .BindFactory<Explosion, Explosion.Factory>()
                .FromPoolableMemoryPool<Explosion, ExplosionPool>(x => x
                    .WithInitialSize(20)
                    .FromComponentInNewPrefab(_settings.explosionPrefab)
                    .UnderTransformGroup("Explosions"));


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
        class AsteroidPool : MonoPoolableMemoryPool<int, Asteroid.AsteroidSizes, IMemoryPool, Asteroid>
        {
        }
        
        class UfoPool : MonoPoolableMemoryPool<IMemoryPool, Ufo>
        {
        }
    
        class ExplosionPool : MonoPoolableMemoryPool<IMemoryPool, Explosion>
        {
        }
    }
}