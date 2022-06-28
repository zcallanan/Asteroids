using System;
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

        // TODO: clear on game over
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerRespawnEffect(Player player, Settings settings)
        {
            _player = player;
            _settings = settings;
        }
    
        public void Initialize()
        {
            _player.JustRespawned.Subscribe(shouldTriggerEffect =>
            {
                if (shouldTriggerEffect)
                {
                    InitiateRespawnEffect();
                }
            }).AddTo(_disposables);
        }

        private void TogglePlayerTransparency()
        {
            _player.MeshRenderer.material = _player.MeshRenderer.material.name == $"{_settings.defaultMat.name} (Instance)"
                ? _settings.transparentMat
                : _settings.defaultMat;
            
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

        private void InitiateRespawnEffect()
        {
            _isTogglingTransparency = true;
            
            TogglePlayerTransparency();

            Observable
                .Timer(TimeSpan.FromSeconds(_settings.totalRespawnEffectDuration))
                .Subscribe(_ =>
                {
                    if (_player.JustRespawned.Value)
                    {
                        CleanupRespawnEffect();
                    }
                })
                .AddTo(_disposables);
        }

        private void CleanupRespawnEffect()
        {
            _player.MeshCollider.enabled = true;
            _player.JustRespawned.Value = false;
                
            _player.MeshRenderer.material = _settings.defaultMat;
                
            _isTogglingTransparency = false;
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
