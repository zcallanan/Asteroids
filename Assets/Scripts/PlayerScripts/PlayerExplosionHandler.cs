using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerExplosionHandler : IInitializable, IDisposable
    {
        private readonly Player _player;
        private readonly Explosion.Factory _explosionFactory;
        private readonly GameState _gameState;

        private Explosion _explosion;

        private Color _startColor;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerExplosionHandler(
            Player player,
            Explosion.Factory explosionFactory,
            GameState gameState)
        {
            _player = player;
            _explosionFactory = explosionFactory;
            _gameState = gameState;
        }
        
        public void Initialize()
        {
            _startColor = new Color(0, 1, 1, 1f);
            
            ExplodeOnTriggerEnter();

            DelayThenDespawnExplosion();
            
            Dispose();
        }

        private void ExplodeOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => CreateExplosion())
                .AddTo(_disposables);
        }
        
        private void DelayThenDespawnExplosion()
        {
            _player.GameObj
                .OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    Observable
                        .Timer(TimeSpan.FromSeconds(1))
                        .Subscribe(_ => _explosion.Dispose())
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _disposables.Clear();
                    }
                })
                .AddTo(_disposables);
        }
        
        private void CreateExplosion()
        {
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _player.Position;
            
            var expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            var expMain = expParticleSystem.main;
            
            expMain.startSpeed = 1f;
            expMain.startColor = _startColor;

            expParticleSystem.Clear();
            expParticleSystem.Play();
        }
    }
}
