using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerThrustHandler : IInitializable
    {
        private readonly Player _player;
        private readonly PlayerInputState _playerInputState;
        private readonly Thrust.Factory _thrustFactory;
        private readonly GameState _gameState;

        private GameObject _thrustAttach;
        private Thrust _thrust;
        
        public PlayerThrustHandler(
            Player player,
            PlayerInputState playerInputState,
            Thrust.Factory thrustFactory,
            GameState gameState)
        {
            _player = player;
            _playerInputState = playerInputState;
            _thrustFactory = thrustFactory;
            _gameState = gameState;
        }

        public void Initialize()
        {
            _thrustAttach = _player.GameObj.transform.GetChild(0).gameObject;

            EnableThrustEffectUponForwardInput();
            
            DisposeOfThrust();
        }
        
        private void DisposeOfThrust()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _thrust.Dispose();
                    }
                })
                .AddTo(_player.GameObj);
        }

        private void EnableThrustEffectUponForwardInput()
        {
            _playerInputState.IsApplyingThrust
                .Subscribe(isForwardInputPositive =>
                {
                    if (isForwardInputPositive)
                    {
                        CreateAndAttachThrust(); 
                    }
                    else
                    {
                        if (_thrust != null)
                        {
                            _thrust.Dispose();
                        }
                    }
                })
                .AddTo(_player.GameObj);
        }
        
        private void CreateAndAttachThrust()
        {
            _thrust = _thrustFactory.Create();
            
            _thrust.Parent = _thrustAttach.transform;
            _thrust.SetPosition(_thrust.Parent.position);
            _thrust.SetFacing(-_thrust.Parent.forward);
        }
    }
}
