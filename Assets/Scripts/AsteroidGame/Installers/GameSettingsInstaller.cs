using System;
using AsteroidGame.AsteroidScripts;
using AsteroidGame.Misc;
using AsteroidGame.PlayerScripts;
using AsteroidGame.UfoScripts;
using AsteroidGame.ViewModels;
using AsteroidGame.Views;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Installers
{
    [CreateAssetMenu(fileName = "UntitledInstaller", menuName = "Installers/GameSettings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public AsteroidSpawner.Settings asteroidSpawner;
        public UfoSpawner.Settings ufoSpawner;
        public PlayerSpawner.Settings playerSpawner;
        public GameInstaller.Settings gameInstaller;
        public GameSettings gameSettings;
        public PlayerSettings playerSettings;
        public AsteroidSettings asteroidSettings;
        public UfoSettings ufoSettings;

        [Serializable]
        public class GameSettings
        {
            public GameLevelHandler.Settings gameLevelHandler;
            public ScoreViewModel.Settings scoreViewModel;
            public Difficulty.Settings difficulty;
            public GameOverViewModel.Settings gameOverViewModel;
            public LivesViewModel.Settings livesViewModel;
            public ViewData.Settings livesViewData;
        }

        [Serializable]
        public class PlayerSettings
        {
            public PlayerDirectionHandler.Settings playerDirectionHandler;
            public PlayerMoveHandler.Settings playerMoveHandler;
            public PlayerData.Settings playerData;
            public PlayerRespawnEffect.Settings playerRespawnEffect;
            public PlayerFiringHandler.Settings playerFiringHandler;
            public PlayerThrustHandler.Settings playerThrustHandler;
        }

        [Serializable]
        public class UfoSettings
        {
            public UfoFiringHandler.Settings ufoFiringHandler;
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
            Container.BindInstance(gameSettings.scoreViewModel).IfNotBound();
            Container.BindInstance(gameSettings.gameOverViewModel).IfNotBound();
            Container.BindInstance(gameSettings.livesViewModel).IfNotBound();
            Container.BindInstance(gameSettings.livesViewData).IfNotBound();

            Container.BindInstance(asteroidSpawner).IfNotBound();
            Container.BindInstance(asteroidSettings.asteroidData).IfNotBound();
            
            Container.BindInstance(playerSpawner).IfNotBound();

            Container.BindInstance(playerSettings.playerDirectionHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerMoveHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerData).IfNotBound();
            
            Container.BindInstance(playerSettings.playerRespawnEffect).IfNotBound();
            Container.BindInstance(playerSettings.playerFiringHandler).IfNotBound();
            Container.BindInstance(playerSettings.playerThrustHandler).IfNotBound();

            Container.BindInstance(ufoSpawner).IfNotBound();
            Container.BindInstance(ufoSettings.ufoFiringHandler).IfNotBound();
        }
    }
}