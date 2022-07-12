using System.Collections.Generic;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
    public class StartScreenUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> verticalPanels = new List<GameObject>();
        
        private GameState _gameState;

        private Canvas _canvas;
        
        [Inject]
        public void Construct(GameState gameState)
        {
            _gameState = gameState;
        }
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }
        
        private void Start()
        {
            EnableOnceStartScreenTimerEnds();
        }

        private void EnableOnceStartScreenTimerEnds()
        {
            _gameState.IsStartScreenInit
                .Subscribe(isStartScreenInit =>
                {
                    if (isStartScreenInit)
                    {
                        _canvas.enabled = true;
                    }
                })
                .AddTo(this);
        }
    }
}
