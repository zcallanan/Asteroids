using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace PlayerScripts
{
    public class PlayerCollisionHandler : IInitializable, IDisposable
    {
        private readonly Player _player;
        private readonly GameState _gameState;
        
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
            HandleCollisionOnTriggerEnter();
        }
        
        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
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
                .Subscribe(_ => SetupPlayerDeathState())
                .AddTo(_disposables);
        }

        private void SetupPlayerDeathState()
        {
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.IsDead = true;
        }
    }
}
