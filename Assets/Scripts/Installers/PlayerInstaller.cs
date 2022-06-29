using System;
using PlayerScripts;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers
{
    public class PlayerInstaller : MonoInstaller
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

            Container.Bind<PlayerInputState>().AsSingle();
            
            Container.BindInterfacesTo<PlayerInputHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerHyperspaceHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerMoveHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerRespawnEffect>().AsSingle();
            Container.BindInterfacesTo<PlayerFiringHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerExplosionHandler>().AsSingle();
            
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