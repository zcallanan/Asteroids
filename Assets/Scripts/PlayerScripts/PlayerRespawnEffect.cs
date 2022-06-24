using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerRespawnEffect : ITickable
    {
        private readonly Player _player;
        private readonly Settings _settings;
        
        private bool _isTogglingTransparency;
        private bool _isHandlingRespawn;
        private float _whenRespawnEffectStarted;
        private float _whenLastToggleOccurred;

        public PlayerRespawnEffect(Player player, Settings settings)
        {
            _player = player;
            _settings = settings;
        }
    
        // public void Initialize()
        // {
        //     _player.JustRespawned.Subscribe(HandleRespawn);
        // }
        
        public void Tick()
        {
            if (_player.JustRespawned && !_isHandlingRespawn)
            {
                HandleRespawn();
                _isHandlingRespawn = true;
            }
            
            if (_isTogglingTransparency &&
                Time.realtimeSinceStartup - _whenLastToggleOccurred == _settings.toggleEffectDuration)
            {
                TogglePlayerTransparency();
            }
        }

        private void HandleRespawn()
        {
            _isTogglingTransparency = true;
            _whenRespawnEffectStarted = Time.realtimeSinceStartup;
            TogglePlayerTransparency();
        }

        private void TogglePlayerTransparency()
        {
            if (Time.realtimeSinceStartup - _whenRespawnEffectStarted == _settings.totalRespawnEffectDuration)
            {
                _player.MeshRenderer.material = _player.MeshRenderer.material == _settings.defaultMat
                    ? _settings.transparentMat
                    : _settings.defaultMat;
                _whenLastToggleOccurred = Time.realtimeSinceStartup;
            }
            else
            {
                _player.MeshCollider.enabled = true;
                _player.JustRespawned = false;
                _isHandlingRespawn = false;
                
                _player.MeshRenderer.material = _settings.defaultMat;
                
                _isTogglingTransparency = false;
            }
            
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
