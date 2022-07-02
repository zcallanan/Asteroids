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

        private Color _startColor;
        
        public PlayerExplosionHandler(
            Player player,
            Explosion.Factory explosionFactory)
        {
            _player = player;
            _explosionFactory = explosionFactory;
        }
        
        public void Initialize()
        {
            _startColor = new Color(0, 1, 1, 1f);
            
            ExplodeOnTriggerEnter();

            DelayThenDespawnExplosion();
        }

        private void ExplodeOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => CreateExplosion())
                .AddTo(_player.GameObj);
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
                        .AddTo(_player.GameObj);
                })
                .AddTo(_player.GameObj);
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
