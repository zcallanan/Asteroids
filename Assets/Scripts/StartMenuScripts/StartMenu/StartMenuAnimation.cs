using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace StartMenuScripts.StartMenu
{
    public class StartMenuAnimation : MonoBehaviour
    {
        private StartMenuData.Settings _settings;
        
        private Image _backgroundSprite;
        private Color _targetColor;
        private float _selfIncrementingNumber;

        [Inject]
        public void Construct(
            StartMenuData.Settings settings)
        {
            _settings = settings;
        }

        private void Start()
        {
            SetInitialColors();
            
            _selfIncrementingNumber = 0;
        }

        private void Update()
        {
            ChangeTitleBackgroundColor();
        }

        private void SetInitialColors()
        {
            _backgroundSprite = GetComponent<Image>();
            _backgroundSprite.color = PickAColor();
            _targetColor = PickAColor();
        }

        private void ChangeTitleBackgroundColor()
        {
            _backgroundSprite.color = Color.Lerp(_backgroundSprite.color, _targetColor,
                _settings.titleAnimModifier * Time.deltaTime);

            _selfIncrementingNumber =
                Mathf.Lerp(_selfIncrementingNumber, 1f, _settings.titleAnimModifier * Time.deltaTime);

            ResetIncrementingAndChooseNewColorTarget();
        }

        private void ResetIncrementingAndChooseNewColorTarget()
        {
            if (_selfIncrementingNumber > .95f)
            {
                _selfIncrementingNumber = 0;
                _targetColor = PickAColor();
            }
        }

        private static Color PickAColor()
        {
            return Random.ColorHSV(.1f, .9f, .1f, .9f, .1f, .9f);
        }
    }
}
