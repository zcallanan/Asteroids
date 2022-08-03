using UniRx;
using UnityEngine;
using Zenject;

namespace ProjectScripts
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

        protected void ToggleDispose(bool whenGameIsRun)
        {
            gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (isGameRunning == whenGameIsRun)
                    {
                        disposables.Clear();
                    }
                })
                .AddTo(disposables);
        }
    }
}
