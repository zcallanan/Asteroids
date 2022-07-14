using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerExplosionHandler : IInitializable
    {
        private readonly Player _player;
        private readonly Explosion.Factory _explosionFactory;

        private Collider _collider;
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
        
        private void CreateExplosion()
        {
            if (IsExplodingFromFiredBullets())
            {
                return;
            }
            _explosion = _explosionFactory.Create();

            _explosion.transform.position = _player.Position;
            
            var expParticleSystem = _explosion.GetComponent<ParticleSystem>();
            var expMain = expParticleSystem.main;
            
            expMain.startSpeed = 1f;
            expMain.startColor = _startColor;

            expParticleSystem.Clear();
            expParticleSystem.Play();
        }

        private bool IsExplodingFromFiredBullets()
        {
            if (_collider.GetComponent<BulletProjectile>())
            {
                var originType = _collider.GetComponent<BulletProjectile>().OriginType;

                if (originType == ObjectTypes.Player)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExplodeOnTriggerEnter()
        {
            _player.GameObj
                .OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    _collider = collider;
                    
                    CreateExplosion();
                })
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
    }
}
