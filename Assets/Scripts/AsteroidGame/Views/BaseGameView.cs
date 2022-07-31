using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Views
{
    public abstract class BaseGameView : MonoBehaviour
    {
        [Inject]
        protected readonly GameState gameState;

        protected readonly CompositeDisposable disposables = new CompositeDisposable();
        
        protected void CheckIfSpawned(ReactiveProperty<bool> source)
        {
            source
                .Subscribe(areSpawned =>
                {
                    if (areSpawned)
                    {
                        SetUpView();
                    }
                })
                .AddTo(disposables);
        }

        protected abstract void SetUpView();

        protected void CheckForChange<T>(ReactiveProperty<T> source)
        {
            source
                .Subscribe(UpdateVal)
                .AddTo(disposables);
        }

        protected abstract void UpdateVal<T>(T val);

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
