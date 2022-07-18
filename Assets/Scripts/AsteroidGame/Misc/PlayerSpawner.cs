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

        private PlayerFacade _playerFacade;
        private PlayerFacade _otherPlayerFacade;

        public PlayerSpawner(
            GameState gameState,
            PlayerFacade.Factory playerFactory,
            PlayerRegistry playerRegistry,
            Settings settings)
        {
            _gameState = gameState;
            _playerFactory = playerFactory;
            _playerRegistry = playerRegistry;
            _settings = settings;
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
            SetSprite(playerFacade);
            SetProperties(playerFacade);
            SetPlayerPosition(playerFacade);
        }

        private void SetProperties(PlayerFacade playerFacade)
        {
            playerFacade.MeshCollider = playerFacade.gameObject.GetComponent<MeshCollider>();
            playerFacade.MeshRenderer = playerFacade.gameObject.GetComponent<MeshRenderer>();
            playerFacade.Transform = playerFacade.gameObject.GetComponent<Transform>();
        }
        
        private void SetSprite(PlayerFacade playerFacade)
        {
            
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

        [Serializable]
        public class Settings
        {
            public Vector3 singlePlayerSpawnPos;
            public Vector3 playerSpawnPos;
            public Vector3 otherPlayerSpawnPos;
        }
    }
}
