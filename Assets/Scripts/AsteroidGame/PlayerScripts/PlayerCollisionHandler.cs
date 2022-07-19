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
        private readonly ScoreHandler _scoreHandler;

        private Collider _collider;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerCollisionHandler(
            Player player,
            GameState gameState,
            ScoreHandler scoreHandler)
        {
            _player = player;
            _gameState = gameState;
            _scoreHandler = scoreHandler;
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
                    _collider = collider;
                        
                    SetupPlayerDeathState();

                    if (collider.GetComponent<PlayerFacade>())
                    {
                        _scoreHandler.UpdateScore(_player.PlayerType, collider);
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
