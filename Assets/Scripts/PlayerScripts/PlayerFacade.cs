using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;
        private PlayerInputState _playerInputState;
    
        [Inject]
        public void Construct(Player player, PlayerInputState playerInputState)
        {
            _player = player;
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _playerInputState.IsFiring = new ReactiveProperty<bool>(false);
        }

        public MeshCollider MeshCollider => _player.MeshCollider;
        public MeshRenderer MeshRenderer => _player.MeshRenderer;

        public Vector3 Facing => _player.Facing;

        public Vector3 Position => _player.Position;

        public int CurrentLives => _player.CurrentLives;
    }
    
    
}
