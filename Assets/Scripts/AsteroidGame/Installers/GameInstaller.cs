using System;
using AsteroidGame.AsteroidScripts;
using AsteroidGame.Misc;
using AsteroidGame.PlayerScripts;
using AsteroidGame.UfoScripts;
using AsteroidGame.Views;
using ProjectScripts;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        Settings _settings;
    
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AsteroidSpawner>().AsSingle();
            Container.BindInterfacesAndSelfTo<UfoSpawner>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerSpawner>().AsSingle();
            
            Container.Bind<InstanceRegistry>().AsSingle();

            Container.BindInterfacesTo<UfoSpawnInit>().AsSingle();

            Container.BindInterfacesAndSelfTo<GameLevelHandler>().AsSingle();
            
            Container.BindInterfacesTo<LoadStartScene>().AsSingle();
            Container.BindInterfacesTo<GameOverViewModel>().AsSingle();
            Container.BindInterfacesTo<LivesViewModel>().AsSingle();

            Container.Bind<BoundHandler>().FromComponentInHierarchy().AsCached();
            
            Container.Bind<ScoreViewModel>().AsSingle();
            
            Container
                .BindFactory<float, float, ObjectTypes, BulletProjectile, BulletProjectile.Factory>()
                .FromPoolableMemoryPool<float, float, ObjectTypes, BulletProjectile, BulletProjectilePool>(x => x
                    .WithInitialSize(20)
                    .FromComponentInNewPrefab(_settings.bulletProjectilePrefab)
                    .UnderTransformGroup("BulletProjectiles"));
            
            Container
                .BindFactory<Thrust, Thrust.Factory>()
                .FromPoolableMemoryPool<Thrust, ThrustPool>(x => x
                    .WithInitialSize(2)
                    .FromComponentInNewPrefab(_settings.thrustPrefab)
                    .UnderTransformGroup("Thrusts"));
            
            Container
                .BindFactory<int, ObjectTypes, Asteroid, Asteroid.Factory>()
                .FromPoolableMemoryPool<int, ObjectTypes, Asteroid, AsteroidPool>(x => x
                    .WithInitialSize(30)
                    .FromSubContainerResolve()
                    .ByNewPrefabInstaller<AsteroidInstaller>(_settings.asteroidPrefab)
                    .UnderTransformGroup("Asteroids"));

            Container
                .BindFactory<ObjectTypes, Ufo, Ufo.Factory>()
                .FromPoolableMemoryPool<ObjectTypes, Ufo, UfoPool>(x => x
                    .WithInitialSize(4)
                    .FromSubContainerResolve()
                    .ByNewPrefabInstaller<UfoInstaller>(_settings.ufoPrefab)
                    .UnderTransformGroup("Ufos"));
            
            Container
                .BindFactory<Explosion, Explosion.Factory>()
                .FromPoolableMemoryPool<Explosion, ExplosionPool>(x => x
                    .WithInitialSize(20)
                    .FromComponentInNewPrefab(_settings.explosionPrefab)
                    .UnderTransformGroup("Explosions"));
            
            Container.BindFactory<ObjectTypes, PlayerFacade, PlayerFacade.Factory>().FromSubContainerResolve()
                .ByNewPrefabInstaller<PlayerInstaller>(_settings.playerPrefab);

            Container.BindFactory<ObjectTypes, LivesView, LivesView.Factory>()
                .FromComponentInNewPrefab(_settings.playerLivesPrefab);
            
            Container.BindFactory<ObjectTypes, ScoreView, ScoreView.Factory>()
                .FromComponentInNewPrefab(_settings.playerScorePrefab);
            
            Container.BindFactory<GameOverView, GameOverView.Factory>()
                .FromComponentInNewPrefab(_settings.gameOverPrefab);
        }
    
        [Serializable]
        public class Settings
        {
            public GameObject asteroidPrefab;
            public GameObject bulletProjectilePrefab;
            public GameObject ufoPrefab;
            public GameObject explosionPrefab;
            public GameObject thrustPrefab;
            public GameObject playerPrefab;
            public GameObject playerLivesPrefab;
            public GameObject playerScorePrefab;
            public GameObject gameOverPrefab;
        }
    
        class BulletProjectilePool : MonoPoolableMemoryPool<float, float, ObjectTypes, IMemoryPool, BulletProjectile>
        {
        }
        
        class ThrustPool : MonoPoolableMemoryPool<IMemoryPool, Thrust>
        {
        }
        
        class AsteroidPool : MonoPoolableMemoryPool<int, ObjectTypes, IMemoryPool, Asteroid>
        {
        }
        
        class UfoPool : MonoPoolableMemoryPool<ObjectTypes, IMemoryPool, Ufo>
        {
        }
    
        class ExplosionPool : MonoPoolableMemoryPool<IMemoryPool, Explosion>
        {
        }
    }
}