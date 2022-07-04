using System;
using AsteroidScripts;
using UnityEngine;
using Zenject;

namespace Installers
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