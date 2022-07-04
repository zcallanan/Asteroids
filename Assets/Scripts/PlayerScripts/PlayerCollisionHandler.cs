using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerCollisionHandler : IInitializable
    {
        private readonly Player _player;

        private Collider _collider;
        
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
                .Subscribe(collider =>
                {
                    _collider = collider;
                    
                    SetupPlayerDeathState();
                })
                .AddTo(_player.GameObj);
        }

        private void SetupPlayerDeathState()
        {
            if (IsCollidingWithFiredBullets())
            {
                return;
            }
            
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.IsDead = true;
        }

        private bool IsCollidingWithFiredBullets()
        {
            if (_collider.GetComponent<BulletProjectile>())
            {
                var originType = _collider.GetComponent<BulletProjectile>().OriginType;

                if (originType == ObjectTypes.Player)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
