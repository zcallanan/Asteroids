using System;
using StartMenuScripts.Misc;
using StartMenuScripts.StartMenu;
using StartMenuScripts.Views;
using UnityEngine;
using Zenject;

namespace StartMenuScripts.Installers
{
    public class StartMenuInstaller : MonoInstaller
    {
        
        [Inject]
        private Settings _settings;
        
        public override void InstallBindings()
        {
            Container.Bind<StartMenuState>().AsSingle();
            Container.Bind<StartMenuInstanceRegistry>().AsSingle();
            
            Container.BindInterfacesTo<StartMenuHandler>().AsSingle();
            Container.BindInterfacesTo<SetupAsteroidData>().AsSingle();
            Container.BindInterfacesTo<LoadAsteroidScene>().AsSingle();
            
            Container.BindFactory<string, int, DifficultyView, DifficultyView.Factory>()
                .FromComponentInNewPrefab(_settings.difficultyPrefab);
        }

        [Serializable]
        public class Settings
        {
            public GameObject difficultyPrefab;
        }
    }
}