using System.Collections.Generic;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace StartMenuScripts.StartMenu
{
    public class StartMenuInteractions : MonoBehaviour
    {
        public List<Canvas> gameModeCanvases = new List<Canvas>();

        private StartMenuState _startMenuState;
        private GameState _gameState;

        private Canvas _canvas;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            StartMenuState iStartMenuState,
            GameState iGameState)
        {
            _startMenuState = iStartMenuState;
            _gameState = iGameState;
        }
        
        private void Awake()
        {
            _startMenuState.IsStartScreenInit = new ReactiveProperty<bool>(false);
            _startMenuState.MenuFocus = new ReactiveProperty<bool>(false);
            _startMenuState.AreDifficultyViewsSpawned = new ReactiveProperty<bool>(false);
            
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            HideMenuOptions(gameModeCanvases);
        }
        
        private void Start()
        {
            EnableOnceStartScreenTimerEnds();

            EnableGameModeCanvas();

            // EnableGameDifficultyCanvas();

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
        
        private void HideMenuOptions(List<Canvas> canvasList)
        {
            foreach (var canvas in canvasList)
            {
                canvas.enabled = false;
            }
        }
        
        private void EnableGameModeCanvas()
        {
            _gameState.GameMode
                .Subscribe(index =>
                {
                    HideMenuOptions(gameModeCanvases);

                    gameModeCanvases[index].enabled = true;
                })
                .AddTo(_disposables);
        }
        
        // private void EnableGameDifficultyCanvas()
        // {
        //     _gameState.GameDifficulty
        //         .Subscribe(index =>
        //         {
        //             HideMenuOptions(gameDifficultyCanvases);
        //
        //             gameDifficultyCanvases[index].enabled = true;
        //         })
        //         .AddTo(_disposables);
        // }

        private void EnableOnceStartScreenTimerEnds()
        {
            _startMenuState.IsStartScreenInit
                .Subscribe(isStartScreenInit =>
                {
                    if (isStartScreenInit)
                    {
                        _canvas.enabled = true;
                    }
                })
                .AddTo(_disposables);
        }
    }
}
