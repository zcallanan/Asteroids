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
        
        private int _previousScore;
        private int _getALifeEveryTenK = 10000;

        private bool _gameIsOver;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerLifecycleHandler(
            Player player,
            PlayerData.Settings playerData,
            GameState gameState)
        {
            _player = player;
            _playerData = playerData;
            _gameState = gameState;
        }

        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                IncrementCurrentLivesEveryTenKScoreUnlessGameIsOver();

                DisposeIfGameNotRunning();
            }
        }
        
        public void PlayerDeathEvents()
        {
            if (_gameState.CurrentLives.Value > -1)
            {
                _gameState.CurrentLives.Value--;
            }

            RestorePlayerFromDeathAfterDelay();

            DisablePlayerObjectOnDeath();

            if (!_gameIsOver)
            {
                SetGameOver();
            }
        }

        private void IncrementCurrentLivesEveryTenKScoreUnlessGameIsOver()
        {
            _gameState.Score.Subscribe(currentScore =>
            {
                if (_previousScore < _getALifeEveryTenK && currentScore >= _getALifeEveryTenK && !_gameIsOver)
                {  
                    _getALifeEveryTenK += 10000;
                    
                    _gameState.CurrentLives.Value++;
                }
                
                _previousScore = currentScore;
            });
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

        private void SetGameOver()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _gameIsOver = true;

                        DelayBeforeGameOver();
                    }
                })
                .AddTo(_disposables);
        }

        private void DelayBeforeGameOver()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(7))
                .Subscribe(_ =>
                {
                    _gameState.IsGameRunning.Value = false;
                    _gameState.IsGameReset.Value = true;
                })
                .AddTo(_disposables);
        }

        private void DisablePlayerObjectOnDeath()
        {
            if (_gameState.CurrentLives.Value < 0)
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
                    if (_player.IsDead && _gameState.CurrentLives.Value >= 0)
                    {
                        _player.MeshRenderer.enabled = true;
                        _player.MeshCollider.enabled = false;
                
                        _player.Facing = Vector3.forward;
                        _player.Position = Vector3.up;
                        _player.SetRotation(Vector3.up);
                
                        _player.JustRespawned.Value = true;
                        _player.IsDead = false;
                    }
                })
                .AddTo(_disposables);
        }
    }
}
