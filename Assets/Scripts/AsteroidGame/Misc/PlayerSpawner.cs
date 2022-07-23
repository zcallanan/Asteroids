using System;
using AsteroidGame.PlayerScripts;
using ProjectScripts;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Misc
{
    public class PlayerSpawner : IInitializable
    {
        private readonly GameState _gameState;
        private readonly PlayerFacade.Factory _playerFactory;
        private readonly PlayerRegistry _playerRegistry;
        private readonly Settings _settings;
        private readonly PlayerData.Settings _playerDataSettings;

        private PlayerFacade _playerFacade;
        private PlayerFacade _otherPlayerFacade;

        public PlayerSpawner(
            GameState gameState,
            PlayerFacade.Factory playerFactory,
            PlayerRegistry playerRegistry,
            Settings settings,
            PlayerData.Settings playerDataSettings)
        {
            _gameState = gameState;
            _playerFactory = playerFactory;
            _playerRegistry = playerRegistry;
            _settings = settings;
            _playerDataSettings = playerDataSettings;
        }

        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                SpawnPlayers();
            }
        }

        private void SpawnPlayers()
        {
            _playerFacade = _playerFactory.Create(ObjectTypes.Player);
            _playerFacade.name = "Player";
            
            _playerRegistry.playerFacades.Add(_playerFacade);
            SetupPlayer(_playerFacade);

            if (_gameState.GameMode.Value != 0)
            {
                _otherPlayerFacade = _playerFactory.Create(ObjectTypes.OtherPlayer);
                _playerFacade.name = "Other Player";
                
                _playerRegistry.playerFacades.Add(_otherPlayerFacade);
                SetupPlayer(_otherPlayerFacade);
            }

            _gameState.ArePlayersSpawned.Value = true;
        }

        private void SetupPlayer(PlayerFacade playerFacade)
        {
            SetProperties(playerFacade);
            SetPlayerPosition(playerFacade);
            SetOtherPlayerMat(playerFacade);
        }

        private void SetProperties(PlayerFacade playerFacade)
        {
            playerFacade.MeshCollider = playerFacade.gameObject.GetComponent<MeshCollider>();
            playerFacade.MeshRenderer = playerFacade.gameObject.GetComponent<MeshRenderer>();
            playerFacade.Transform = playerFacade.gameObject.GetComponent<Transform>();
        }

        private void SetPlayerPosition(PlayerFacade playerFacade)
        {
            if (_gameState.GameMode.Value == 0 && playerFacade.PlayerType == ObjectTypes.Player)
            {
                playerFacade.Position = _settings.singlePlayerSpawnPos;
            }
            else if (_gameState.GameMode.Value != 0)
            {
                if (playerFacade.PlayerType == ObjectTypes.Player)
                {
                    playerFacade.Position = _settings.playerSpawnPos;
                }
                else if (playerFacade.PlayerType == ObjectTypes.OtherPlayer)
                {
                    playerFacade.Position = _settings.otherPlayerSpawnPos;
                }
            }
        }

        private void SetOtherPlayerMat(PlayerFacade playerFacade)
        {
            if (playerFacade.PlayerType == ObjectTypes.OtherPlayer)
            {
                var meshRenderer = playerFacade.MeshRenderer;
                meshRenderer.material = _playerDataSettings.otherPlayerMat;
            }
        }

        [Serializable]
        public class Settings
        {
            public Vector3 singlePlayerSpawnPos;
            public Vector3 playerSpawnPos;
            public Vector3 otherPlayerSpawnPos;
        }
    }
}
