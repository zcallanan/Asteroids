using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerHyperspaceHandler : IInitializable, ITickable
    {
        private readonly PlayerInputState _playerInputState;
        private readonly Player _player;
        private readonly BoundManager _boundManager;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;

        private bool _hyperspaceWasTriggered;

        private float _whenHyperspaceTriggered;

        public PlayerHyperspaceHandler(
            PlayerInputState playerInputState, 
            Player player,
            BoundManager boundManager)
        {
            _playerInputState = playerInputState;
            _player = player;
            _boundManager = boundManager;
        }
        
        public void Initialize()
        {
            _boundManager.MaxBounds.Subscribe(maxGameBounds => _maxBounds = maxGameBounds);
            _boundManager.MinBounds.Subscribe(minGameBounds => _minBounds = minGameBounds);
            
            _playerInputState.IsHyperspaceActive.Subscribe(hyperspaceInput =>
            {
                if (hyperspaceInput && !_hyperspaceWasTriggered && _player.MeshRenderer.enabled)
                {
                    HyperSpaceTriggered();
                }
            });
        }
        
        public void Tick()
        {
            if (_hyperspaceWasTriggered && Time.realtimeSinceStartup - _whenHyperspaceTriggered >= 2f &&
                _whenHyperspaceTriggered != 0)
            {
                _player.MeshRenderer.enabled = true;
                _player.AdjustedSpeed = 0;
            
                _hyperspaceWasTriggered = false;
                _whenHyperspaceTriggered = 0;
            }
        }
        
        private void HyperSpaceTriggered()
        {
            _hyperspaceWasTriggered = true;
            _whenHyperspaceTriggered = Time.realtimeSinceStartup;

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
