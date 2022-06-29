using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerRespawnEffect : IInitializable, IDisposable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        private readonly GameState _gameState;
        
        private bool _isTogglingTransparency;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerRespawnEffect(
            Player player,
            Settings settings,
            GameState gameState)
        {
            _player = player;
            _settings = settings;
            _gameState = gameState;
        }
    
        public void Initialize()
        {
            InitializeRespawnEffect();
        }
        
        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void InitializeRespawnEffect()
        {
            _player.JustRespawned.Subscribe(shouldTriggerEffect =>
            {
                if (shouldTriggerEffect)
                {
                    _isTogglingTransparency = true;
            
                    TogglePlayerTransparency();
                    
                    CleanupRespawnEffectAfterDelay();
                }
            }).AddTo(_disposables);
        }

        private void TogglePlayerTransparency()
        {
            _player.MeshRenderer.material = _player.MeshRenderer.material.name == $"{_settings.defaultMat.name} (Instance)"
                ? _settings.transparentMat
                : _settings.defaultMat;

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
                    if (_player.JustRespawned.Value)
                    {
                        _player.MeshCollider.enabled = true;
                        _player.JustRespawned.Value = false;
                
                        _player.MeshRenderer.material = _settings.defaultMat;
                
                        _isTogglingTransparency = false;
                    }
                })
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float totalRespawnEffectDuration;
            public float toggleEffectDuration;
            public Material defaultMat;
            public Material transparentMat;
        }
    }
}
