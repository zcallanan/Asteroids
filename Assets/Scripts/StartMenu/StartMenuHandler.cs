using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace StartMenu
{
    public class StartMenuHandler : IInitializable
    {
        private readonly InputState _inputState;
        private readonly GameState _gameState;
        private readonly StartMenuState _startMenuState;
        private readonly StartMenuInteractions _startMenuInteractions;
        private readonly Settings _settings;

        private bool _throttleInput;
        
        // TODO: Dispose of these
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public StartMenuHandler(
            InputState inputState,
            GameState gameState,
            StartMenuState startMenuState,
            StartMenuInteractions startMenuInteractions,
            Settings settings)
        {
            _inputState = inputState;
            _gameState = gameState;
            _startMenuState = startMenuState;
            _startMenuInteractions = startMenuInteractions;
            _settings = settings;
        }
        
        public void Initialize()
        {
            OneTimeStartScreenInitTimer();

            DisposeOnGameRunning();
        }

        private void DisposeOnGameRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void OneTimeStartScreenInitTimer()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.startMenuInitDelay))
                .Subscribe(_ =>
                {
                    _startMenuState.IsStartScreenInit.Value = true;
                    
                    CheckForInputOnStartScreenToStartTheGame();
            
                    CheckForVerticalInput();

                    CheckForHorizontalInput();
                })
                .AddTo(_disposables);
        }
        
        private void CheckForInputOnStartScreenToStartTheGame()
        {
            _inputState.IsFiring
                .Subscribe(isFiring =>
                {
                    if (isFiring && !_gameState.IsGameRunning.Value && _startMenuState.IsStartScreenInit.Value)
                    {
                        _gameState.IsGameRunning.Value = true;
                    }
                })
                .AddTo(_disposables);
        }

        private void CheckForVerticalInput()
        {
            _inputState.VerticalInput
                .Subscribe(inputVal =>
                {
                    if (!_gameState.IsGameRunning.Value && inputVal != 0 && !_throttleInput && _startMenuState
                        .IsStartScreenInit.Value)
                    {
                        _startMenuState.MenuFocus.Value = !_startMenuState.MenuFocus.Value;
                        _throttleInput = true;
                        PreventSpammingInputThroughInputDelay();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void CheckForHorizontalInput()
        {
            _inputState.HorizontalInput
                .Subscribe(inputVal =>
                {
                    if (!_gameState.IsGameRunning.Value && inputVal != 0 && !_throttleInput &&
                        _startMenuState.IsStartScreenInit.Value)
                    {
                        if (!_startMenuState.MenuFocus.Value)
                        {
                            _gameState.GameMode.Value = SetValue(inputVal, _startMenuInteractions.gameModeCanvases.Count,
                                _gameState.GameMode.Value);
                        }
                        else
                        {
                            _gameState.GameDifficulty.Value = SetValue(inputVal,
                                _startMenuInteractions.gameDifficultyCanvases.Count, 
                                _gameState.GameDifficulty.Value);
                        }
                        
                        _throttleInput = true;
                        PreventSpammingInputThroughInputDelay();
                    }
                })
                .AddTo(_disposables);
        }
        

        private int SetValue(float inputVal, int listCount, int indexVal)
        {
            var result = 0;
            
            if (inputVal > 0)
            {
                result = indexVal == listCount - 1 ? 0 : indexVal + 1;
            }
            else if (inputVal < 0)
            {
                result = indexVal == 0 ? listCount - 1 : indexVal - 1;
            }

            return result;
        }

        private void PreventSpammingInputThroughInputDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_settings.startMenuInputDelay))
                .Subscribe(_ => _throttleInput = false)
                .AddTo(_disposables);
        }

        [Serializable]
        public class Settings
        {
            public float startMenuInitDelay;
            public float startMenuInputDelay;
        }
    }
}
