using System;
using System.Collections.Generic;
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
        private ScoreView.Factory _scoreViewFactory;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            GameState gameState,
            InstanceRegistry instanceRegistry,
            LivesView.Factory livesViewFactory,
            ScoreView.Factory scoreViewFactory)
        {
            _gameState = gameState;
            _instanceRegistry = instanceRegistry;
            _livesViewFactory = livesViewFactory;
            _scoreViewFactory = scoreViewFactory;
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
                        
                        CreateScoreViews();
                    }
                    else
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void CreateView<T1, T2, T3>(T1 playerType, List<T2> instanceList, T3 factory ) 
            where T1 : Enum
            where T2 : AbstractView
            where T3 : PlaceholderFactory<T1, T2>
        {
            var view = factory.Create(playerType);
            view.transform.SetParent(gameObject.transform);
            instanceList.Add(view);
        }
        
        private void CreateLivesViews()
        {
            CreateView(ObjectTypes.Player, _instanceRegistry.playerLivesViews, _livesViewFactory);
            if (_gameState.GameMode.Value != 0)
            {
                CreateView(ObjectTypes.OtherPlayer, _instanceRegistry.playerLivesViews, _livesViewFactory);
            }
            _gameState.AreLivesViewsSpawned.Value = true;
        }
        
        private void CreateScoreViews()
        {
            CreateView(ObjectTypes.Player, _instanceRegistry.playerScoreViews, _scoreViewFactory);
            if (_gameState.GameMode.Value != 0)
            {
                CreateView(ObjectTypes.OtherPlayer, _instanceRegistry.playerScoreViews, _scoreViewFactory);
            }
            _gameState.AreScoreViewsSpawned.Value = true;
        }
    }
}
