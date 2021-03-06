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
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
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
            if (_gameState.IsGameRunning.Value)
            {
                _thrustAttach = _player.GameObj.transform.GetChild(0).gameObject;
                
                _thrustScale = _settings.thrustScale;

                EnableThrustEffectUponForwardInput();

                DisposeIfGameNotRunning();
            }
        }
        
        public void Tick()
        {
            var verticalInput = _player.PlayerType == ObjectTypes.Player
                ? _inputState.VerticalInput.Value
                : _inputState.VerticalInput2.Value;
            
            var result = verticalInput > 0 && !_player.IsDead &&
                         !_player.HyperspaceWasTriggered.Value;
            
            switch (_player.PlayerType)
            {
                case ObjectTypes.Player:
                    _inputState.IsApplyingThrust.Value = result;
                    break;
                case ObjectTypes.OtherPlayer:
                    _inputState.IsApplyingThrust2.Value = result;
                    break;
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

        private void EnableThrustEffectUponForwardInput()
        {
            var inputSource = _player.PlayerType == ObjectTypes.Player
                ? _inputState.IsApplyingThrust
                : _inputState.IsApplyingThrust2;
            
            inputSource
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
                .AddTo(_disposables);
        }
        
        private void CreateAndAttachThrust()
        {
            _thrust = _thrustFactory.Create();
            
            _thrust.transform.localScale = _thrustScale;

            if (_player.PlayerType == ObjectTypes.Player)
            {
                _thrust.name = "Player1 Thrust";
            }
            else if (_player.PlayerType == ObjectTypes.OtherPlayer)
            {
                _thrust.name = "Player2 Thrust";
            }

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
