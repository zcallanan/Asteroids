using UniRx;
using UniRx.Triggers;
using Zenject;

namespace PlayerScripts
{
    public class PlayerCollisionHandler : IInitializable
    {
        private readonly Player _player;

        public PlayerCollisionHandler(Player player)
        {
            _player = player;
        }
        
        public void Initialize()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => HandleCollision());
        }

        private void HandleCollision()
        {
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.IsDead = true;
        }
    }
}
