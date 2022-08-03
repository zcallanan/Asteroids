using System;
using ProjectScripts;
using StartMenuScripts.Misc;
using UniRx;
using Zenject;

namespace StartMenuScripts.StartMenu
{
    public class StartMenuHandler : IInitializable
    {
        private readonly InputState _inputState;
        private readonly GameState _gameState;
        private readonly StartMenuState _startMenuState;
        private readonly StartMenuInteractions _startMenuInteractions;
        private readonly StartMenuInstanceRegistry _startMenuInstanceRegistry;
        private readonly Settings _settings;

        private bool _throttleInput;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public StartMenuHandler(
            InputState inputState,
            GameState iGameState,
            StartMenuState iStartMenuState,
            StartMenuInteractions iStartMenuInteractions,
            StartMenuInstanceRegistry iStartMenuInstanceRegistry,
            Settings iSettings)
        {
            _inputState = inputState;
            _gameState = iGameState;
            _startMenuState = iStartMenuState;
            _startMenuInteractions = iStartMenuInteractions;
            _startMenuInstanceRegistry = iStartMenuInstanceRegistry;
            _settings = iSettings;
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
                        WhenInFocusSetGameModeOrGameDifficulty(inputVal);

                        _throttleInput = true;
                        PreventSpammingInputThroughInputDelay();
                    }
                })
                .AddTo(_disposables);
        }

        private void WhenInFocusSetGameModeOrGameDifficulty(float inputVal)
        {
            // If MenuFocus.Value = false -> set GameMode, true -> set GameDifficulty
            if (!_startMenuState.MenuFocus.Value)
            {
                _gameState.GameMode.Value = SetValue(inputVal, _startMenuInteractions.gameModeCanvases.Count,
                    _gameState.GameMode.Value);
            }
            else
            {
                _gameState.GameDifficulty.Value = SetValue(inputVal,
                    _startMenuInstanceRegistry.difficultyViews.Count, 
                    _gameState.GameDifficulty.Value);
            }
        }


        private int SetValue(float inputVal, int listCount, int indexVal)
        {
            var result = 0;
            
            if (inputVal > 0)
            {
                // Right input, if at end of list, move to start index, or +1 to next index
                result = indexVal == listCount - 1 ? 0 : indexVal + 1;
            }
            else if (inputVal < 0)
            {
                // Left input, if at start of list, move to end index, or -1 to previous index
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
