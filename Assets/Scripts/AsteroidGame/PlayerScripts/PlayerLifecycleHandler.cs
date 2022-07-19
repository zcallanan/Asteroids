using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerLifecycleHandler : IInitializable
    {
        private readonly Player _player;
        private readonly PlayerData.Settings _playerData;
        private readonly GameState _gameState;
        private readonly PlayerSpawner.Settings _playerSpawnerSettings;
        
        private int _previousScore;
        private int _getALifeEveryTenK = 10000;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerLifecycleHandler(
            Player player,
            PlayerData.Settings playerData,
            GameState gameState,
            PlayerSpawner.Settings playerSpawnerSettings)
        {
            _player = player;
            _playerData = playerData;
            _gameState = gameState;
            _playerSpawnerSettings = playerSpawnerSettings;
        }

        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                CheckIfPlayersSpawned();

                DisposeIfGameNotRunning();
            }
        }
        
        public void PlayerDeathEvents()
        {
            if (_player.CurrentLives.Value > -1)
            {
                _player.CurrentLives.Value--;
            }

            RestorePlayerFromDeathAfterDelay();

            DisablePlayerObjectOnDeath();
        }
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void CheckIfPlayersSpawned()
        {
            _gameState.ArePlayersSpawned
                .Subscribe(playersSpawned =>
                {
                    if (playersSpawned)
                    {
                        InitializePlayerLifecycleHandler();
                    }
                })
                .AddTo(_disposables);
        }

        private void InitializePlayerLifecycleHandler()
        {
            IncrementCurrentLivesEveryTenKScoreUnlessGameIsOver();
        }

        private void IncrementCurrentLivesEveryTenKScoreUnlessGameIsOver()
        {
            _player.Score.Subscribe(currentScore =>
            {
                if (_previousScore < _getALifeEveryTenK && currentScore >= _getALifeEveryTenK && !_gameState.IsGameOver.Value)
                {  
                    _getALifeEveryTenK += 10000;
                    
                    _player.CurrentLives.Value++;
                }
                
                _previousScore = currentScore;
            });
        }

        private void DisablePlayerObjectOnDeath()
        {
            if (_player.CurrentLives.Value < 0)
            {
                _player.GameObj.SetActive(false);
            }
        }
        
        private void RestorePlayerFromDeathAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_playerData.respawnDelay))
                .Subscribe(_ =>
                {
                    if (_player.IsDead && _player.CurrentLives.Value >= 0)
                    {
                        _player.MeshRenderer.enabled = true;
                        _player.MeshCollider.enabled = false;
                
                        _player.Facing = Vector3.forward;

                        SetPlayerRespawnPositions();
                        
                        _player.SetRotation(Vector3.up);
                
                        _player.JustRespawned.Value = true;
                        _player.IsDead = false;
                    }
                })
                .AddTo(_disposables);
        }

        private void SetPlayerRespawnPositions()
        {
            if (_gameState.GameMode.Value == 0)
            {
                _player.Position = _playerSpawnerSettings.singlePlayerSpawnPos;
            }
            else
            {
                _player.Position = _player.PlayerType == ObjectTypes.Player
                    ? _playerSpawnerSettings.playerSpawnPos
                    : _playerSpawnerSettings.otherPlayerSpawnPos;
            }
        }
    }
}
