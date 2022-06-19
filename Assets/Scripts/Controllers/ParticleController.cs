using System;
using System.Collections;
using Models;
using Pools;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Controllers
{
    public class ParticleController : MonoBehaviour
    {
        public static ParticleController sharedInstance;
        
        [SerializeField] private Particle thrustPrefab;
        
        public ReactiveProperty<Player> PlayerInstance { get; private set; }
        
        private GameObject _thrustAttach;
        private GameObject _obj;
        private Particle _thrustInstance;
        private Particle _explosion;
        private ParticleSystem _partSystem;
        private ParticleSystem.MainModule _main;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            sharedInstance = this;

            PlayerInstance = new ReactiveProperty<Player>(null);
            
            _thrustInstance = Instantiate(thrustPrefab, Vector3.zero, Quaternion.identity);
            _thrustInstance.gameObject.SetActive(false);

            GameManager.sharedInstance.GameOver
                .Subscribe(HandleGameOver)
                .AddTo(_disposables);

            PlayerInstance
                .Where(y => y != null)
                .Subscribe(player =>
                {
                    AttachThrustToPlayerThrustAttachmentPoint(player);
                    player
                        .OnEnableAsObservable()
                        .Subscribe( y => AttachThrustToPlayerThrustAttachmentPoint(player))
                        .AddTo(_disposables);
                    
                    player
                        .UpdateAsObservable()
                        .Subscribe(y => DetermineThrustEffectAppearance(player.IsApplyingThrust.Value))
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);
        }
        
        public void HandleAsteroidExplosion(Asteroid asteroid)
        {
            DetermineObjects();
            _obj.transform.position = asteroid.transform.position;
            _explosion.name = $"{asteroid.Names[asteroid.AsteroidSize]} Asteroid Explosion";
            
            // Explosion scales 0 to 2
            _obj.transform.localScale = _explosion.Scales[asteroid.AsteroidSize] * Vector3.one;
            _main.startColor = _explosion.AsteroidExplosionColor;
            _obj.SetActive(true);
            
            StartCoroutine(ExplosionDisableCoroutine(_explosion));
        }

        public void HandlePlayerExplosion(Player player)
        {
            DetermineObjects();
            
            _obj.transform.position = player.transform.position;
            _explosion.name = $"Player Explosion";
            
            // Explosion scale 3
            _obj.transform.localScale = _explosion.Scales[3] * Vector3.one;
            _main.startColor = _explosion.PlayerExplosionColor;
            _obj.SetActive(true);
            
            StartCoroutine(ExplosionDisableCoroutine(_explosion));
        }
        
        public void HandleUfoExplosion(Ufo ufo)
        {
            DetermineObjects();
            
            _obj.transform.position = ufo.transform.position;
            _explosion.name = $"{ufo.Names[ufo.UfoSize]} UFO Explosion";
            
            // Explosion scale 4 and 5
            int explosionScale = 4;
            
            if (ufo.UfoSize == 1)
            {
                explosionScale = 5;
            }
            
            _obj.transform.localScale = _explosion.Scales[explosionScale] * Vector3.one;
            _main.startColor = _explosion.UfoExplosionColor;
            _obj.SetActive(true);
            
            StartCoroutine(ExplosionDisableCoroutine(_explosion));
        }

        private void HandleGameOver(bool gameOver)
        {
            if (gameOver)
            {
                _disposables.Clear();
            }
        }

        private void DetermineThrustEffectAppearance(bool isThrustActive)
        {
            _thrustInstance.gameObject.SetActive(isThrustActive);
            _thrustInstance.transform.forward = -PlayerInstance.Value.PlayerFacing;
        }

        private void AttachThrustToPlayerThrustAttachmentPoint(Player player)
        {
            _thrustAttach = player.gameObject.transform.GetChild(0).gameObject;
            var thrustTransform = _thrustInstance.transform;
            thrustTransform.parent = _thrustAttach.transform;
            thrustTransform.position = _thrustAttach.transform.position;
            
            // GameManager.sharedInstance.OnPlayerAppliedThrust += EnableThrustEffect;
        }

        private void DetermineObjects()
        {
            _explosion = ParticlePool.SharedInstance.GetPooledObject();
            _partSystem = _explosion.GetComponent<ParticleSystem>();
            _main = _partSystem.main;
            _obj = _explosion.gameObject;
        }

        private IEnumerator ExplosionDisableCoroutine(Particle explosion)
        {
            yield return new WaitForSeconds(explosion.ExplosionDuration);
            explosion.gameObject.SetActive(false);
        }
    }
}
