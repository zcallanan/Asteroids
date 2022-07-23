using ProjectScripts;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace StartMenuScripts.Misc
{
    public class LoadAsteroidScene : IInitializable
    {
        private readonly GameState _gameState;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public LoadAsteroidScene(
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
                    if (isGameRunning)
                    {
                        _disposables.Clear();
                        
                        SceneManager.LoadScene("AsteroidGame");
                    }
                })
                .AddTo(_disposables);
        }
    }
}
