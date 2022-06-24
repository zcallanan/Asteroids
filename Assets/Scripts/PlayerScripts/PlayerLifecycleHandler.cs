using System;
using System.Collections;
using Installers;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerLifecycleHandler : MonoBehaviour
    {
        private Player _player;
        private GameSettingsInstaller.PlayerLifecycleHandlerSettings _settings;
        
        private float _lastDeathTime;

        [Inject]
        public void Construct(
            Player player,
            GameSettingsInstaller.PlayerLifecycleHandlerSettings settings)
        {
            _player = player;
            _settings = settings;
        }

        private void Start()
        {
            _player.CurrentLives = 2;
        }

        private void Update()
        {
            CheckIfPlayerShouldRespawn();
        }

        private void CheckIfPlayerShouldRespawn()
        {
            if (_player.CurrentLives >= 0 && Time.realtimeSinceStartup - _lastDeathTime == _settings.respawnDelay)
            {
                _player.MeshRenderer.enabled = true;
                _player.MeshCollider.enabled = false;
                
                _player.Facing = Vector3.zero;
                _player.Position = new Vector3(0,1,0);
                _player.Rotation = Vector3.up;
                
                _lastDeathTime = 0;
                _player.JustRespawned = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"other is {other}");
            Debug.Log($"player is {gameObject}");
            _lastDeathTime = Time.realtimeSinceStartup;
            _player.MeshRenderer.enabled = false;
        }
    }
}