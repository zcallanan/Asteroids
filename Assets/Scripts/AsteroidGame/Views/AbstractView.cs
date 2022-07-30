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

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        protected void CheckIfSpawned(ReactiveProperty<bool> source, ObjectTypes playerType)
        {
            source
                .Subscribe(areSpawned =>
                {
                    if (areSpawned &&
                        (playerType == ObjectTypes.Player || playerType == ObjectTypes.OtherPlayer))
                    {
                        SetUp();
                    }
                })
                .AddTo(_disposables);
        }
        
        protected abstract void SetUp();
        
        protected void CheckForChange<T>(ReactiveProperty<T> source)
        {
            source
                .Subscribe(UpdateVal)
                .AddTo(_disposables);
        }

        protected abstract void UpdateVal<T>(T val);

        protected void DisposeIfGameNotRunning()
        {
            gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
    }
}
