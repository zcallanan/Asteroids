using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerRespawnEffect : IInitializable, ITickable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        
        private bool _isTogglingTransparency;
        private float _whenRespawnEffectStarted;
        private float _whenLastToggleOccurred;
        
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
        
        public void Tick()
        {
            if (_isTogglingTransparency &&
                Time.realtimeSinceStartup - _whenLastToggleOccurred >= _settings.toggleEffectDuration)
            {
                TogglePlayerTransparency();
            }

            if (_player.JustRespawned.Value && Time.realtimeSinceStartup - _whenRespawnEffectStarted >=
                _settings.totalRespawnEffectDuration)
            {
                CleanupRespawnEffect();
            }
        }
        
        private void TogglePlayerTransparency()
        {
            _player.MeshRenderer.material = _player.MeshRenderer.material.name == $"{_settings.defaultMat.name} (Instance)"
                ? _settings.transparentMat
                : _settings.defaultMat;
            _whenLastToggleOccurred = Time.realtimeSinceStartup;
        }

        private void InitiateRespawnEffect()
        {
            _isTogglingTransparency = true;
            _whenRespawnEffectStarted = Time.realtimeSinceStartup;
            
            TogglePlayerTransparency();
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
