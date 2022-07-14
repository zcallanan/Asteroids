using ProjectScripts;
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
        private float _num;

        [Inject]
        public void Construct(
            StartMenuData.Settings settings)
        {
            _settings = settings;
        }

        private void Start()
        {
            _backgroundSprite = GetComponent<Image>();
            _backgroundSprite.color = PickAColor();
            _num = 0;
            
            _targetColor = PickAColor();
        }

        private void Update()
        {
            ChangeTitleBackgroundColor();
        }

        private void ChangeTitleBackgroundColor()
        {
            _backgroundSprite.color = Color.Lerp(_backgroundSprite.color, _targetColor,
                _settings.titleAnimModifier * Time.deltaTime * 2);

            _num = Mathf.Lerp(_num, 1f, _settings.titleAnimModifier * Time.deltaTime * 2);

            if (_num > .99f)
            {
                _num = 0;
                _targetColor = PickAColor();
            }
        }

        private Color PickAColor()
        {
            return Random.ColorHSV(.1f, .9f, .1f, .9f, .1f, .9f);
        }
    }
}
