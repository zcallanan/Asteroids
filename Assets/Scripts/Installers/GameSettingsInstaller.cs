using System;
using System.Collections.Generic;
using Misc;
using PlayerScripts;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "UntitledInstaller", menuName = "Installers/GameSettings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameInstaller.Settings gameInstaller;
        public GameSettings gameSettings;
        public PlayerSettings playerSettings;
        public DifficultySettings difficultySettings;
        // public List<Difficulty> difficultySettings = new List<Difficulty>();

        [Serializable]
        public class GameSettings
        {
            public GameLevelHandler.Settings gameLevelHandler;
        }

        [Serializable]
        public class PlayerSettings
        {
            public PlayerDirectionHandler.Settings playerDirectionHandler;
            public PlayerMoveHandler.Settings playerMoveHandler;
            public PlayerLifecycleHandlerSettings playerLifecycleHandlerSettings;
            public PlayerRespawnEffect.Settings playerRespawnEffect;
            public PlayerFiringHandler.Settings playerFiringHandler;
        }
        
        [Serializable]
        public class PlayerLifecycleHandlerSettings
        {
            public float respawnDelay;
        }

        [Serializable]
        public class DifficultySettings
        {
            public List<Difficulty> difficulties = new List<Difficulty>();
        }
        
        [Serializable]
        public class Difficulty
        {
            public int smallPerMedium;
            public int initLargeAsteroids;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(gameInstaller).IfNotBound();
            
            Container.BindInstance(gameSettings.gameLevelHandler).IfNotBound();
            
            Container.BindInstance(playerSettings.playerDirectionHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerMoveHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerLifecycleHandlerSettings).IfNotBound();
            Container.BindInstance(playerSettings.playerRespawnEffect).IfNotBound();
            Container.BindInstance(playerSettings.playerFiringHandler).IfNotBound();

        }
    }
}