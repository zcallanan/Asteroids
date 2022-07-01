using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour, IDisposable
    {
        private Player _player;
        private GameState _gameState;
        private PlayerInputState _playerInputState;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            Player player,
            GameState gameState,
            PlayerInputState playerInputState)
        {
            _player = player;
            _gameState = gameState;
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);
            
            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _playerInputState.IsFiring = new ReactiveProperty<bool>(false);
            
            _playerInputState.IsApplyingThrust = new ReactiveProperty<bool>(false);
        }

        private void Start()
        {
            Dispose();
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
    }
}
