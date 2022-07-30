using System;
using AsteroidGame.Views;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Misc
{
    public class ViewManager : MonoBehaviour
    {
        private GameState _gameState;
        private InstanceRegistry _instanceRegistry;
        private LivesView.Factory _livesViewFactory;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            GameState gameState,
            InstanceRegistry instanceRegistry,
            LivesView.Factory livesViewFactory)
        {
            _gameState = gameState;
            _instanceRegistry = instanceRegistry;
            _livesViewFactory = livesViewFactory;
        }

        private void Start()
        {
            CheckForGameStart();
        }

        private void CheckForGameStart()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (isGameRunning)
                    {
                        CreateLivesViews();
                    }
                    else
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void CreateLivesViews()
        {
            var view = _livesViewFactory.Create(ObjectTypes.Player);
            view.transform.SetParent(gameObject.transform);

            _instanceRegistry.playerLivesViews.Add(view);
            
            if (_gameState.GameMode.Value != 0)
            {
                view = _livesViewFactory.Create(ObjectTypes.OtherPlayer);
                view.transform.SetParent(gameObject.transform);
                
                _instanceRegistry.playerLivesViews.Add(view);
            }

            _gameState.AreLivesViewsSpawned.Value = true;
        }
    }
}
