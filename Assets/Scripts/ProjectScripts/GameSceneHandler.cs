using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace ProjectScripts
{
    public class GameSceneHandler : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public GameSceneHandler(
            GameState gameState)
        {
            _gameState = gameState;
        }

        public void Initialize()
        {
            CheckForGameRunningAndChangeScene();
        }
            
        private void CheckForGameRunningAndChangeScene()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    var scene = SceneManager.GetActiveScene();
                    var targetSceneToChangeTo = isGameRunning ? "AsteroidGame" : "StartScreen";

                    if (scene.name != targetSceneToChangeTo)
                    {
                        SceneManager.LoadScene(targetSceneToChangeTo);
                    }
                })
                .AddTo(_disposables);
        }
    }
}
