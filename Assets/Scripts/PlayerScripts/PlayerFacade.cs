using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour, IDisposable
    {
        private GameState _gameState;
        private PlayerInputState _playerInputState;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            GameState gameState,
            PlayerInputState playerInputState)
        {
            _gameState = gameState;
            _playerInputState = playerInputState;
        }

        private void Awake()
        {
            _playerInputState.IsHyperspaceActive = new ReactiveProperty<bool>(false);
            _playerInputState.IsFiring = new ReactiveProperty<bool>(false);
        }

        private void Start()
        {
            AddOnTriggerEnterObservable();

            AddOnEnabledTriggerObservable();

            Dispose();
        }
        
        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void AddOnTriggerEnterObservable()
        {
            if (gameObject.GetComponent<ObservableTriggerTrigger>() == null)
            {
                gameObject
                    .AddComponent<ObservableTriggerTrigger>()
                    .UpdateAsObservable()
                    .SampleFrame(60)
                    .Subscribe(_ => Debug.Log($"Observable added"))
                    .AddTo(_disposables);
            }
        }
        
        private void AddOnEnabledTriggerObservable()
        {
            if (gameObject.GetComponent<ObservableEnableTrigger>() == null)
            {
                gameObject
                    .AddComponent<ObservableEnableTrigger>()
                    .UpdateAsObservable()
                    .SampleFrame(60)
                    .Subscribe(_ => Debug.Log("Enable trigger added"))
                    .AddTo(_disposables);
            }
        }
    }
}
