using System;
using System.Collections.Generic;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Views
{
    public class GameViewManager : MonoBehaviour
    {
        private GameState _gameState;
        private InstanceRegistry _instanceRegistry;
        private LivesView.Factory _livesViewFactory;
        private ScoreView.Factory _scoreViewFactory;
        private GameOverView.Factory _gameOverFactory;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(
            GameState gameState,
            InstanceRegistry instanceRegistry,
            LivesView.Factory livesViewFactory,
            ScoreView.Factory scoreViewFactory,
            GameOverView.Factory gameOverFactory)
        {
            _gameState = gameState;
            _instanceRegistry = instanceRegistry;
            _livesViewFactory = livesViewFactory;
            _scoreViewFactory = scoreViewFactory;
            _gameOverFactory = gameOverFactory;
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

                        CreateGameOverView();
                    }
                    else
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void CreatePlayerView<T1, T2, T3>(T1 playerType, List<T2> instanceList, T3 factory) 
            where T1 : Enum
            where T2 : BaseGameView
            where T3 : PlaceholderFactory<T1, T2>
        {
            var view = factory.Create(playerType);
            view.transform.SetParent(gameObject.transform);
            instanceList.Add(view);
        }
        
        private void CreateView<T1, T2>(List<T1> instanceList, T2 factory) 
            where T1 : BaseGameView
            where T2 : PlaceholderFactory<T1>
        {
            var view = factory.Create();
            view.transform.SetParent(gameObject.transform);
            instanceList.Add(view);
        }
        
        private void CreateLivesViews()
        {
            CreatePlayerView(ObjectTypes.Player, _instanceRegistry.playerLivesViews, _livesViewFactory);
            if (_gameState.GameMode.Value != 0)
            {
                CreatePlayerView(ObjectTypes.OtherPlayer, _instanceRegistry.playerLivesViews, _livesViewFactory);
            }
            _gameState.AreLivesViewsSpawned.Value = true;
        }
        
        private void CreateScoreViews()
        {
            CreatePlayerView(ObjectTypes.Player, _instanceRegistry.playerScoreViews, _scoreViewFactory);
            if (_gameState.GameMode.Value != 0)
            {
                CreatePlayerView(ObjectTypes.OtherPlayer, _instanceRegistry.playerScoreViews, _scoreViewFactory);
            }
            _gameState.AreScoreViewsSpawned.Value = true;
        }
        
        private void CreateGameOverView()
        {
            CreateView(_instanceRegistry.gameOverView, _gameOverFactory);

            _gameState.IsGameOverViewSpawned.Value = true;
        }
    }
}
