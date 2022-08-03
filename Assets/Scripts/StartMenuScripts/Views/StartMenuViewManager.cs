using ProjectScripts;
using StartMenuScripts.Misc;
using StartMenuScripts.StartMenu;
using UniRx;
using UnityEngine;
using Zenject;

namespace StartMenuScripts.Views
{
    public class StartMenuViewManager : MonoBehaviour
    {
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject copyright;
        [SerializeField] private GameObject difficulties;
        [SerializeField] private GameObject gameModes;

        private StartMenuState _startMenuState;
        private GameState _gameState;
        private StartMenuData.Settings _startMenuDataSettings;
        private StartMenuInstanceRegistry _startMenuInstanceRegistry;
        private DifficultyView.Factory _difficultyViewFactory;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            StartMenuState iStartMenuState,
            GameState iGameState,
            StartMenuData.Settings iStartMenuDataSettings,
            StartMenuInstanceRegistry iStartMenuInstanceRegistry,
            DifficultyView.Factory iDifficultyViewFactory)
        {
            _startMenuState = iStartMenuState;
            _gameState = iGameState;
            _startMenuDataSettings = iStartMenuDataSettings;
            _startMenuInstanceRegistry = iStartMenuInstanceRegistry;
            _difficultyViewFactory = iDifficultyViewFactory;
        }

        private void Start()
        {
            CreateViewsAfterDelay();
            
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
        
        private void CreateViewsAfterDelay()
        {
            _startMenuState.IsStartScreenInit
                .Subscribe(isStartScreenInit =>
                {
                    if (isStartScreenInit)
                    {
                        CreateDifficultyViews();
                    }
                })
                .AddTo(_disposables);
        }

        private void CreateDifficultyViews()
        {
            for (var index = 0; index < _startMenuDataSettings.difficultyLevels.Count; index++)
            {
                var view = _difficultyViewFactory.Create(_startMenuDataSettings.difficultyLevels[index], index);
                view.transform.SetParent(difficulties.transform);
                _startMenuInstanceRegistry.difficultyViews.Add(view);
            }

            _startMenuState.AreDifficultyViewsSpawned.Value = true;
        }
    }
}
