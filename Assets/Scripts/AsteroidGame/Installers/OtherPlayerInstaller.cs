using System;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Installers
{
    public class OtherPlayerInstaller : MonoInstaller
    {
        [SerializeField]
        private Settings settings;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Player>().AsSingle()
                .WithArguments(
                    settings.meshRenderer,
                    settings.meshCollider,
                    settings.transform).NonLazy(); 
            
            Container.BindInstance(settings.playerType).WhenInjectedInto<Player>();
            
            Container.BindInterfacesTo<PlayerHyperspaceHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerMoveHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerRespawnEffect>().AsSingle();
            Container.BindInterfacesTo<PlayerFiringHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerExplosionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerThrustHandler>().AsSingle();
            Container.BindInterfacesTo<OtherPlayerHandler>().AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerLifecycleHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerCollisionHandler>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public MeshRenderer meshRenderer;
            public MeshCollider meshCollider;
            public Transform transform;
            public ObjectTypes playerType;
        }
    }
}