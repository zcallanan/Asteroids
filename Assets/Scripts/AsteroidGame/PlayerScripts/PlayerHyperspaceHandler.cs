using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerHyperspaceHandler : IInitializable
    {
        private readonly InputState _inputState;
        private readonly Player _player;
        private readonly BoundHandler _boundHandler;

        private Vector3 _maxBounds;
        private Vector3 _minBounds;
        
        public PlayerHyperspaceHandler(
            InputState inputState, 
            Player player,
            BoundHandler boundHandler)
        {
            _inputState = inputState;
            _player = player;
            _boundHandler = boundHandler;
        }
        
        public void Initialize()
        {
            GetGameBounds();

            HandleHyperspaceInput();
        }

        private void GetGameBounds()
        {
            _boundHandler.MaxBounds.Subscribe(maxGameBounds => _maxBounds = maxGameBounds);
            _boundHandler.MinBounds.Subscribe(minGameBounds => _minBounds = minGameBounds);
        }

        private void HandleHyperspaceInput()
        {
            _inputState.IsHyperspaceActive.Subscribe(hyperspaceInput =>
            {
                if (hyperspaceInput && !_player.HyperspaceWasTriggered.Value && _player.MeshRenderer.enabled)
                {
                    HyperSpaceTriggered();
                }
            }).AddTo(_player.GameObj);
        }

        private void HyperSpaceTriggered()
        {
            _player.HyperspaceWasTriggered.Value = true;
            
            _player.MeshRenderer.enabled = false;
            _player.MeshCollider.enabled = false;
            
            _player.Position = DetermineRandomHyperspacePosition();
            
            EndHyperspaceAfterDelay();
        }

        private void EndHyperspaceAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(2f))
                .Subscribe(_ =>
                {
                    if (_player.HyperspaceWasTriggered.Value && !_player.IsDead)
                    {
                        _player.MeshRenderer.enabled = true;
                        _player.MeshCollider.enabled = true;
                        
                        _player.AdjustedSpeed = 0;
            
                        _player.HyperspaceWasTriggered.Value = false;
                    }
                })
                .AddTo(_player.GameObj);
        }

        private Vector3 DetermineRandomHyperspacePosition()
        {
            return new Vector3(Random.Range(_minBounds.x + 1, _maxBounds.x - 1), 1,
                Random.Range(_minBounds.z + 1, _maxBounds.z - 1));
        }
    }
}
