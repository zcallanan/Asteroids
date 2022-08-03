using ProjectScripts;
using StartMenuScripts.StartMenu;
using TMPro;
using UnityEngine;
using Zenject;

namespace StartMenuScripts.Views
{
    public class DifficultyView : BaseGameView
    {
        [SerializeField] private TMP_Text text;

        private StartMenuState _startMenuState;
        private StartMenuData.Settings _startMenuDataSettings;
        private GameState _gameState;
        private string _difficultyText;
        private int _difficultyIndex;

        private Canvas _canvas;

        [Inject]
        public void Construct(
            StartMenuState iStartMenuState,
            StartMenuData.Settings iStartMenuDataSettings,
            GameState iGameState,
            [InjectOptional] string iDifficultyText,
            [InjectOptional] int iDifficultyIndex)
        {
            _startMenuState = iStartMenuState;
            _startMenuDataSettings = iStartMenuDataSettings;
            _gameState = iGameState;
            _difficultyText = iDifficultyText;
            _difficultyIndex = iDifficultyIndex;
        }

        private void Start()
        {
            if (_startMenuDataSettings.difficultyLevels.Contains(_difficultyText))
            {
                CheckIfSpawned(_startMenuState.AreDifficultyViewsSpawned);
                
                CheckForChange(_gameState.GameDifficulty);
            }

            ToggleDispose(true);
        }

        protected override void SetUpView()
        {
            gameObject.name = _difficultyText;

            _canvas = GetComponent<Canvas>();
            _canvas.enabled = _difficultyIndex == 0;
            
            text.text = _difficultyText;
            
            var rectTransform = text.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.pivot = Vector2.zero;
            
            // rectTransform.localScale = Vector3.one;
            // rectTransform.localRotation = Quaternion.Euler(0,0,0);
            
            Vector2 pos = _startMenuDataSettings.positionsV2s[_difficultyIndex];
            Vector2 widthHeight = _startMenuDataSettings.widthHeights[_difficultyIndex];
            
            rectTransform.sizeDelta = new Vector2(widthHeight.x, widthHeight.y);
            rectTransform.localPosition = new Vector3(pos.x, pos.y, transform.position.z);
        }

        protected override void UpdateVal<T>(T val)
        {
            if (_canvas)
            {
                _canvas.enabled = (int) (object) val == _difficultyIndex;
                text.enableWordWrapping = false;
            }
        }

        public class Factory : PlaceholderFactory<string, int, DifficultyView>
        {
        }
    }
}
