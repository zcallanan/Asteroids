using Installers;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerLifecycleHandler : MonoBehaviour
    {
        private Player _player;
        private PlayerData.Settings _playerData;
        
        private float _lastDeathTime;
        
        [Inject]
        public void Construct(
            Player player,
            PlayerData.Settings playerData)
        {
            _player = player;
            _playerData = playerData;
        }

        private void Awake()
        {
            _player.JustRespawned = new ReactiveProperty<bool>(false);
            _player.CurrentLives = new ReactiveProperty<int>(2);
        }

        private void Update()
        {
            CheckIfPlayerShouldRespawn();
        }
        
        // TODO: Increase current lives.

        private void CheckIfPlayerShouldRespawn()
        {
            if (_player.IsDead && _player.CurrentLives.Value >= 0 &&
                Time.realtimeSinceStartup - _lastDeathTime >= _playerData.respawnDelay)
            {
                _player.MeshRenderer.enabled = true;
                _player.MeshCollider.enabled = false;
                
                _player.Facing = Vector3.zero;
                _player.Position = new Vector3(0,1,0);
                _player.Rotation = Vector3.up;
                
                _player.JustRespawned.Value = true;
                _player.IsDead = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"other is {other}");
            Debug.Log($"player is {gameObject}");
            _lastDeathTime = Time.realtimeSinceStartup;

            _player.CurrentLives.Value--;
            
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.IsDead = true;
        }
    }
}
