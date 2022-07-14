using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerThrustHandler : IInitializable, ITickable
    {
        private readonly Player _player;
        private readonly InputState _inputState;
        private readonly Settings _settings;
        private readonly Thrust.Factory _thrustFactory;
        private readonly GameState _gameState;

        private Vector3 _thrustScale;

        private GameObject _thrustAttach;
        private Thrust _thrust;
        
        public PlayerThrustHandler(
            Player player,
            InputState inputState,
            Settings settings,
            Thrust.Factory thrustFactory,
            GameState gameState)
        {
            _player = player;
            _inputState = inputState;
            _settings = settings;
            _thrustFactory = thrustFactory;
            _gameState = gameState;
        }

        public void Initialize()
        {
            _thrustAttach = _player.GameObj.transform.GetChild(0).gameObject;

            _thrustScale = _settings.thrustScale;

            EnableThrustEffectUponForwardInput();
            
            DisposeOfThrust();
        }
        
        public void Tick()
        {
            _inputState.IsApplyingThrust.Value = _inputState.VerticalInput.Value > 0 && !_player.IsDead &&
                                                 !_player.HyperspaceWasTriggered.Value;
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
            _inputState.IsApplyingThrust
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
            
            _thrust.transform.localScale = _thrustScale;
            _thrust.name = "Player Thrust";

            _thrust.Parent = _thrustAttach.transform;
            _thrust.SetPosition(_thrust.Parent.position);
            _thrust.SetFacing(-_thrust.Parent.forward);
        }

        [Serializable]
        public class Settings
        {
            public Vector3 thrustScale;
        }
    }
}
