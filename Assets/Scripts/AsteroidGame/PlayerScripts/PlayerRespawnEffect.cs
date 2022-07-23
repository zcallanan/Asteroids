using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerRespawnEffect : IInitializable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        private readonly GameState _gameState;
        private readonly PlayerData.Settings _playerDataSettings;

        private bool _isTogglingTransparency;

        private Material _defaultPlayerMaterial;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerRespawnEffect(
            Player player,
            Settings settings,
            GameState gameState,
            PlayerData.Settings playerDataSettings)
        {
            _player = player;
            _settings = settings;
            _gameState = gameState;
            _playerDataSettings = playerDataSettings;
        }
    
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                CheckIfPlayersSpawned();

                SetDefaultMaterial();

                DisposeIfGameNotRunning();
            }
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
                        InitializeRespawnEffect();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void SetDefaultMaterial()
        {
            if (_player.PlayerType == ObjectTypes.Player)
            {
                _defaultPlayerMaterial = _playerDataSettings.playerMat;
            }
            else if (_player.PlayerType == ObjectTypes.OtherPlayer)
            {
                _defaultPlayerMaterial = _playerDataSettings.otherPlayerMat;
            }
        }

        private void InitializeRespawnEffect()
        {
            _player.JustRespawned
                .Subscribe(shouldTriggerEffect =>
            {
                if (shouldTriggerEffect)
                {
                    _player.JustRespawned.Value = false;

                    _isTogglingTransparency = true;
            
                    TogglePlayerTransparency();
                    
                    CleanupRespawnEffectAfterDelay();
                }
            })
                .AddTo(_disposables);
        }

        private void TogglePlayerTransparency()
        {
            var materialName = $"{_defaultPlayerMaterial.name} (Instance)";

            _player.MeshRenderer.material = _player.MeshRenderer.material.name == materialName
                ? _settings.transparentMat
                : _defaultPlayerMaterial;

            ToggleRespawnEffectAfterDelay();
        }

        private void ToggleRespawnEffectAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.toggleEffectDuration))
                .Subscribe(_ =>
                {
                    if (_isTogglingTransparency)
                    {
                        TogglePlayerTransparency();
                    }
                })
                .AddTo(_disposables);
        }

        private void CleanupRespawnEffectAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.totalRespawnEffectDuration))
                .Subscribe(_ =>
                {
                    _player.MeshCollider.enabled = true;
            
                    _player.MeshRenderer.material = _defaultPlayerMaterial;
            
                    _isTogglingTransparency = false;
                })
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float totalRespawnEffectDuration;
            public float toggleEffectDuration;
            public Material transparentMat;
        }
    }
}
