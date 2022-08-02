using AsteroidGame.Misc;
using AsteroidGame.ViewModels;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.UfoScripts
{
    public class UfoCollisionHandler : IInitializable
    {
        private readonly Ufo _ufo;
        private readonly ScoreViewModel _scoreViewModel;
        private readonly GameState _gameState;

        private Collider _collider;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UfoCollisionHandler(
            Ufo ufo,
            ScoreViewModel scoreViewModel,
            GameState gameState)
        {
            _ufo = ufo;
            _scoreViewModel = scoreViewModel;
            _gameState = gameState;
        }

        public void Initialize()
        {
            if (_gameState.IsGameRunning.Value)
            {
                OnCollisionUpdateScore();

                DisposeIfGameNotRunning();
            }
        }
        
        private void DisposeIfGameNotRunning()
        {
            _gameState.IsGameRunning
                .Subscribe(isGameRunning =>
                {
                    if (!isGameRunning)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }

        private void OnCollisionUpdateScore()
        {
            _ufo.OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    _collider = collider;
                    
                    if (IsCollidingWithFiredBullets())
                    {
                        return;
                    }

                    _scoreViewModel.UpdateScore(_ufo.Size, _collider);
                })
                .AddTo(_disposables);
        }

        private bool IsCollidingWithFiredBullets()
        {
            if (_collider.GetComponent<BulletProjectile>())
            {
                var originType = _collider.GetComponent<BulletProjectile>().OriginType;

                if (originType == ObjectTypes.LargeUfo || originType == ObjectTypes.SmallUfo)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
