using System;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private ObjectTypes playerType;
        [SerializeField] private Settings settings;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Player>().AsSingle()
            .WithArguments(
                settings.meshRenderer,
                settings.meshCollider,
                settings.transform).NonLazy(); 
            
            Container.BindInstance(playerType).WhenInjectedInto<Player>();

            Container.BindInterfacesTo<PlayerHyperspaceHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerRespawnEffect>().AsSingle();
            Container.BindInterfacesTo<PlayerFiringHandler>().AsSingle();
            
            Container.BindInterfacesTo<PlayerExplosionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerThrustHandler>().AsSingle();
            Container.BindInterfacesTo<OtherPlayerHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerMoveHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerLifecycleHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerCollisionHandler>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public MeshRenderer meshRenderer;
            public MeshCollider meshCollider;
            public Transform transform;
        }
    }
}