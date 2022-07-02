using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace PlayerScripts
{
    public class PlayerCollisionHandler : IInitializable
    {
        private readonly Player _player;
        
        public PlayerCollisionHandler(
            Player player)
        {
            _player = player;
        }
        
        public void Initialize()
        {
            HandleCollisionOnTriggerEnter();
        }

        private void HandleCollisionOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => SetupPlayerDeathState())
                .AddTo(_player.GameObj);
        }

        private void SetupPlayerDeathState()
        {
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.IsDead = true;
        }
    }
}
