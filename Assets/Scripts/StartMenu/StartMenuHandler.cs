using System;
using Misc;
using UniRx;
using Zenject;

namespace StartMenu
{
    public class StartMenuHandler : IInitializable
    {
        private readonly InputState _inputState;
        private readonly GameState _gameState;
        private readonly StartMenuState _startMenuState;
        private readonly StartMenuInteractions _startMenuInteractions;

        private bool _throttleInput;
        
        // TODO: Dispose of these
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public StartMenuHandler(
            InputState inputState,
            GameState gameState,
            StartMenuState startMenuState,
            StartMenuInteractions startMenuInteractions)
        {
            _inputState = inputState;
            _gameState = gameState;
            _startMenuState = startMenuState;
            _startMenuInteractions = startMenuInteractions;
        }
        
        public void Initialize()
        {
            OneTimeStartScreenInitTimer();

            CheckForInputOnStartScreenToStartTheGame();
            
            CheckForVerticalInput();

            CheckForHorizontalInput();
        }
        
        private void OneTimeStartScreenInitTimer()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(2.5))
                .Subscribe(_ => _startMenuState.IsStartScreenInit.Value = true)
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
                    if (!_gameState.IsGameRunning.Value && inputVal != 0 && !_throttleInput)
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
                    if (!_gameState.IsGameRunning.Value && inputVal != 0 && !_throttleInput)
                    {
                        if (!_startMenuState.MenuFocus.Value)
                        {
                            _gameState.GameMode.Value = SetValue(inputVal, _startMenuInteractions.gameModeCanvases.Count,
                                _gameState.GameMode.Value);
                        }
                        else
                        {
                            _gameState.GameDifficulty.Value = SetValue(inputVal, _startMenuInteractions.gameDifficultyCanvases.Count,
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
                .Timer(TimeSpan.FromSeconds(.5))
                .Subscribe(_ => _throttleInput = false)
                .AddTo(_disposables);
        }
    }
}
