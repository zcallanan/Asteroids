using ProjectScripts;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace AsteroidGame.Misc
{
    public class LoadStartScene : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public LoadStartScene(
            GameState gameState)
        {
            _gameState = gameState;
        }

        public void Initialize()
        {
            LoadAsteroidGameScene();
        }

        private void LoadAsteroidGameScene()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                        
                        SceneManager.LoadScene("StartScreen");
                    }
                })
                .AddTo(_disposables);
        }
    }
}
