using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Views
{
    public abstract class AbstractView : MonoBehaviour
    {
        [Inject]
        protected readonly GameState gameState;

        protected readonly CompositeDisposable disposables = new CompositeDisposable();
        protected abstract void CheckIfSpawned();
        protected abstract void SetUp();
        protected abstract void CheckForChange();
        
        protected void DisposeIfGameNotRunning()
        {
            gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        disposables.Clear();
                    }
                })
                .AddTo(disposables);
        }
    }
}
