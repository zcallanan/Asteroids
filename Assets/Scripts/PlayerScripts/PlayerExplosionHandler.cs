using System;
using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerExplosionHandler : IInitializable
    {
        private readonly Player _player;
        private readonly Explosion.Factory _explosionFactory;

        private Explosion _explosion;
        
        private ParticleSystem _expParticleSystem;
        private ParticleSystem.MainModule _expMain;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerExplosionHandler(
            Player player,
            Explosion.Factory explosionFactory)
        {
            _player = player;
            _explosionFactory = explosionFactory;
        }
        
        public void Initialize()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => CreateExplosion())
                .AddTo(_disposables);
            
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
        
        private void CreateExplosion()
        {
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _player.Position;
            
            _expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            _expMain = _expParticleSystem.main;
            
            _expMain.startSpeed = 1f;

            _expParticleSystem.Clear();
            _expParticleSystem.Play();
        }
    }
}
