using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Misc
{
    public class GameSceneHandler : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public GameSceneHandler(GameState gameState)
        {
            _gameState = gameState;
        }

        public void Initialize()
        {
            OneTimeStartScreenInitTimer();

            CheckForGameRunningAndChangeScene();
        }
            
        private void CheckForGameRunningAndChangeScene()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    var scene = SceneManager.GetActiveScene();
                    
                    Debug.Log($"{scene.name}");
                    
                    var targetSceneToChangeTo = isGameRunning ? "AsteroidGame" : "StartScreen";

                    if (scene.name != targetSceneToChangeTo)
                    {
                        SceneManager.LoadScene(targetSceneToChangeTo);
                    }
                })
                .AddTo(_disposables);
        }

        private void OneTimeStartScreenInitTimer()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(2.5))
                .Subscribe(_ => _gameState.IsStartScreenInit.Value = true)
                .AddTo(_disposables);
        }
    }
}
