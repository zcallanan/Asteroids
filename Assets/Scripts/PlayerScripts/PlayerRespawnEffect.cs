using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerRespawnEffect : IInitializable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        
        private bool _isTogglingTransparency;
        
        public PlayerRespawnEffect(
            Player player,
            Settings settings)
        {
            _player = player;
            _settings = settings;
        }
    
        public void Initialize()
        {
            InitializeRespawnEffect();
        }

        private void InitializeRespawnEffect()
        {
            _player.JustRespawned.Subscribe(shouldTriggerEffect =>
            {
                if (shouldTriggerEffect)
                {
                    _player.JustRespawned.Value = false;

                    _isTogglingTransparency = true;
            
                    TogglePlayerTransparency();
                    
                    CleanupRespawnEffectAfterDelay();
                }
            }).AddTo(_player.GameObj);
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
                .AddTo(_player.GameObj);
        }

        private void CleanupRespawnEffectAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.totalRespawnEffectDuration))
                .Subscribe(_ =>
                {
                    _player.MeshCollider.enabled = true;
            
                    _player.MeshRenderer.material = _settings.defaultMat;
            
                    _isTogglingTransparency = false;
                })
                .AddTo(_player.GameObj);
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
