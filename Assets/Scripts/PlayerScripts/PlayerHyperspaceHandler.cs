using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace PlayerScripts
{
    public class PlayerHyperspaceHandler : IInitializable
    {
        private readonly PlayerInputState _playerInputState;
        private readonly Player _player;
        private readonly BoundHandler _boundHandler;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;

        private bool _hyperspaceWasTriggered;
        
        // TODO: clear on game over
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerHyperspaceHandler(
            PlayerInputState playerInputState, 
            Player player,
            BoundHandler boundHandler)
        {
            _playerInputState = playerInputState;
            _player = player;
            _boundHandler = boundHandler;
        }
        
        public void Initialize()
        {
            _boundHandler.MaxBounds.Subscribe(maxGameBounds => _maxBounds = maxGameBounds);
            _boundHandler.MinBounds.Subscribe(minGameBounds => _minBounds = minGameBounds);
            
            _playerInputState.IsHyperspaceActive.Subscribe(hyperspaceInput =>
            {
                if (hyperspaceInput && !_hyperspaceWasTriggered && _player.MeshRenderer.enabled)
                {
                    HyperSpaceTriggered();
                }
            }).AddTo(_disposables);
        }

        private void HyperSpaceTriggered()
        {
            _hyperspaceWasTriggered = true;

            Observable
                .Timer(TimeSpan.FromSeconds(2f))
                .Subscribe(_ =>
                {
                    if (_hyperspaceWasTriggered && !_player.IsDead)
                    {
                        _player.MeshRenderer.enabled = true;
                        _player.AdjustedSpeed = 0;
            
                        _hyperspaceWasTriggered = false;
                    }
                })
                .AddTo(_disposables);

            _player.MeshRenderer.enabled = false;
            _player.Position = DetermineRandomHyperspacePosition();
        }

        private Vector3 DetermineRandomHyperspacePosition()
        {
            return new Vector3(Random.Range(_minBounds.x + 1, _maxBounds.x - 1), 1,
                Random.Range(_minBounds.z + 1, _maxBounds.z - 1));
        }
    }
}
