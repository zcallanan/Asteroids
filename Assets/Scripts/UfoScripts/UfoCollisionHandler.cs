using Misc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace UfoScripts
{
    public class UfoCollisionHandler : IInitializable
    {
        private readonly Ufo _ufo;
        private readonly ScoreHandler _scoreHandler;

        private Collider _collider;

        public UfoCollisionHandler(
            Ufo ufo,
            ScoreHandler scoreHandler)
        {
            _ufo = ufo;
            _scoreHandler = scoreHandler;
        }

        public void Initialize()
        {
            OnCollisionUpdateScore();
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
                    
                    _scoreHandler.UpdateScore(_ufo.Size);
                })
                .AddTo(_ufo.gameObject);
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
