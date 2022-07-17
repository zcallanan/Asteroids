using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerCollisionHandler : IInitializable
    {
        private readonly Player _player;
        private readonly GameState _gameState;

        private Collider _collider;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerCollisionHandler(
            Player player,
            GameState gameState)
        {
            _player = player;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                HandleCollisionOnTriggerEnter();

                DisposeIfGameNotRunning();
            }
        }
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void HandleCollisionOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    if (_player.CanCollide)
                    {
                        _collider = collider;
                        
                        SetupPlayerDeathState();
                    }
                })
                .AddTo(_disposables);
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

                if (originType == (_player.PlayerType == ObjectTypes.Player ? ObjectTypes.Player : ObjectTypes.OtherPlayer))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
