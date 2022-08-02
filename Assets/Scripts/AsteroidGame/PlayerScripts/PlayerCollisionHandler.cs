using AsteroidGame.Misc;
using AsteroidGame.ViewModels;
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
        private readonly ScoreViewModel _scoreViewModel;

        private Collider _collider;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerCollisionHandler(
            Player player,
            GameState gameState,
            ScoreViewModel scoreViewModel)
        {
            _player = player;
            _gameState = gameState;
            _scoreViewModel = scoreViewModel;
        }
        
        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                HandleCollisionOnTriggerEnter();

                DisposeIfGameNotRunning();
            }
        }
        
        public bool IsCollidingWithPlayerBullets(Collider collider)
        {
            if (collider.GetComponent<BulletProjectile>())
            {
                var originType = collider.GetComponent<BulletProjectile>().OriginType;

                if (originType == PlayerFiredTheBullet() || TeammateFiredTheBullet(originType))
                {
                    return true;
                }
            }

            return false;
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
        
        private ObjectTypes PlayerFiredTheBullet()
        {
            return _player.PlayerType == ObjectTypes.Player ? ObjectTypes.Player : ObjectTypes.OtherPlayer;
        }
        
        private bool TeammateFiredTheBullet(ObjectTypes originType)
        {
            return _gameState.GameMode.Value == 2 && originType == ObjectTypes.Player ||
                   originType == ObjectTypes.OtherPlayer;
        }

        private void HandleCollisionOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    _collider = collider;

                    if (_collider.GetComponent<PlayerFacade>())
                    {
                        PreventTeamWorkCollision(_collider);
                    }
                    else
                    {
                        SetupPlayerDeathState();
                    }
                })
                .AddTo(_disposables);
        }

        private void PreventTeamWorkCollision(Collider collider)
        {
            if (_gameState.GameMode.Value != 2)
            {
                SetupPlayerDeathState();
                _scoreViewModel.UpdateScore(_player.PlayerType, collider);
            }
        }

        private void SetupPlayerDeathState()
        {
            if (IsCollidingWithPlayerBullets(_collider))
            {
                return;
            }
            
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.IsDead = true;
        }
    }
}
