using System;
using PlayerScripts;
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
            Container.Bind<Player>().AsSingle()
                .WithArguments(
                    settings.meshRenderer,
                    settings.meshFilter,
                    settings.meshCollider,
                    settings.rigidbody,
                    settings.transform).NonLazy(); 

            Container.Bind<PlayerInputState>().AsSingle();
            
            Container.BindInterfacesTo<PlayerInputHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerHyperspaceHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerDirectionHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerMoveHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerRespawnEffect>().AsSingle();
            Container.BindInterfacesTo<PlayerFiringHandler>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public MeshRenderer meshRenderer;
            public MeshFilter meshFilter;
            public MeshCollider meshCollider;
            public Rigidbody rigidbody;
            public Transform transform;
        }
    }
}