using System;
using AsteroidScripts;
using Misc;
using PlayerScripts;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "UntitledInstaller", menuName = "Installers/GameSettings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public AsteroidSpawner.Settings asteroidSpawner;
        public GameInstaller.Settings gameInstaller;
        public GameSettings gameSettings;
        public PlayerSettings playerSettings;
        public AsteroidSettings asteroidSettings;

        [Serializable]
        public class GameSettings
        {
            public GameLevelHandler.Settings gameLevelHandler;
            public ScoreHandler.Settings scoreHandler;
            public Difficulty.Settings difficulty;
        }

        [Serializable]
        public class PlayerSettings
        {
            public PlayerDirectionHandler.Settings playerDirectionHandler;
            public PlayerMoveHandler.Settings playerMoveHandler;
            public PlayerData.Settings playerData;
            public PlayerRespawnEffect.Settings playerRespawnEffect;
            public PlayerFiringHandler.Settings playerFiringHandler;
        }

        [Serializable]
        public class AsteroidSettings
        {
            public AsteroidData.Settings asteroidData;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(gameInstaller).IfNotBound();
            
            Container.BindInstance(gameSettings.difficulty).IfNotBound();
            Container.BindInstance(gameSettings.gameLevelHandler).IfNotBound();
            Container.BindInstance(gameSettings.scoreHandler).IfNotBound();
            
            Container.BindInstance(asteroidSpawner).IfNotBound();
            
            Container.BindInstance(asteroidSettings.asteroidData).IfNotBound();

            Container.BindInstance(playerSettings.playerDirectionHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerMoveHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerData).IfNotBound();
            Container.BindInstance(playerSettings.playerRespawnEffect).IfNotBound();
            Container.BindInstance(playerSettings.playerFiringHandler).IfNotBound();

        }
    }
}