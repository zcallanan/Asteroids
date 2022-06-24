using System;
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