using System.Collections;
using System.Collections.Generic;
using Models;
using Pools;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class ProjectileController : MonoBehaviour
    {
        public static ProjectileController sharedInstance;
        
        public ReactiveProperty<Player> PlayerInstance { get; private set; }
        
        private bool _isPlayerFiring;
        private bool _isPlayerFiringInputDisregarded;
        private bool _isPlayerFiringInCooldown;
        private IEnumerator _hideProjectileCoroutine;
        private Projectile _playerProjectile;
        private Projectile _ufoProjectile;
        private Transform _playerTransform;
        private readonly List<Ufo> _allUfoInstancesInScene = new List<Ufo>();
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            sharedInstance = this;

            PlayerInstance = new ReactiveProperty<Player>(null);
            
            GameManager.sharedInstance.GameOver
                .Subscribe(HandleGameOver)
                .AddTo(_disposables);

            PlayerInstance
                .Where(y => y != null)
                .Subscribe(player =>
                {
                    player
                        .OnEnableAsObservable()
                        .Subscribe(y => _isPlayerFiringInputDisregarded = false)
                        .AddTo(_disposables);
                    
                    player
                        .UpdateAsObservable()
                        .Subscribe(y => HandleDisableDuringHyperspace(player.IsHyperspaceActive.Value))
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);

            _isPlayerFiringInCooldown = false;
        }
        
        private void Update()
        {
            if (PlayerInstance.Value)
            {
                _playerTransform = PlayerInstance.Value.transform;

                if (!_isPlayerFiringInputDisregarded)
                {
                    _isPlayerFiring = InputController.sharedInstance.IsFiring;
                }
                
                if (_isPlayerFiring && !_isPlayerFiringInCooldown)
                {
                    _playerProjectile = ProjectilePool.SharedInstance.GetPooledObject();
                    PlayerProjectileSetup();
                }
            }
        }

        public void RemoveDeadUfoFromActiveList(Ufo ufo)
        {
            if (_allUfoInstancesInScene.Contains(ufo))
            {
                _allUfoInstancesInScene.Remove(ufo);
            }
        }
        
        public void PreventFiring()
        {
            _isPlayerFiringInputDisregarded = true;
            _isPlayerFiring = false;
        }

        public void SpawnUfoProjectile(Ufo ufo)
        {
            _ufoProjectile = ProjectilePool.SharedInstance.GetPooledObject();
            // ufo.IsReadyToFire.Value = false;
            
            if (!_allUfoInstancesInScene.Contains(ufo))
            {
                _allUfoInstancesInScene.Add(ufo);
            }
            
            Physics.IgnoreCollision(ufo.UfoMeshCollider, _ufoProjectile.ProjectileSphereCollider);
            Physics.IgnoreCollision(PlayerInstance.Value.PlayerMeshCollider,
                _ufoProjectile.ProjectileSphereCollider, false);
            
            var obj = _ufoProjectile.gameObject;
            obj.SetActive(true);
            obj.layer = 12;
            _ufoProjectile.transform.position = ufo.transform.position;
            _ufoProjectile.name = "Ufo Projectile";
            _ufoProjectile.FacingDirection = DetermineUfoProjectileFacing();
            _ufoProjectile.gameObject.transform.localScale = .15f * Vector3.one;
            
            _hideProjectileCoroutine = HideProjectileCoroutine(_ufoProjectile, _ufoProjectile.UfoProjectileLifespan);
            StartCoroutine(_hideProjectileCoroutine);
            _ufoProjectile.DisableHidePlayerProjectileCoroutine = _hideProjectileCoroutine;
        }

        private void HandleGameOver(bool gameOver)
        {
            if (gameOver)
            {
                _disposables.Clear();
            }
        }

        private Vector3 DetermineUfoProjectileFacing()
        {
            var position = PlayerInstance.Value.transform.position + new Vector3(
                Random.Range(GameManager.sharedInstance.UfoOffsetMin.x,
                    GameManager.sharedInstance.UfoOffsetMax.x), 1,
                Random.Range(GameManager.sharedInstance.UfoOffsetMin.z,
                    GameManager.sharedInstance.UfoOffsetMax.z));
                
            var n = Random.Range(0, 10);
            
            return (n < _ufoProjectile.UfoFireTowardsPlayerFrequency) ? position : -position;;
        }

        private void HandleDisableDuringHyperspace(bool isHyperspaceActive)
        {
            if (isHyperspaceActive)
            {
                PreventFiring();
            }
            else
            {
                _isPlayerFiringInputDisregarded = false;
            }
        }

        private void PlayerProjectileSetup()
        {
            
            _playerProjectile.gameObject.layer = 6;
            Physics.IgnoreCollision(PlayerInstance.Value.PlayerMeshCollider, _playerProjectile.ProjectileSphereCollider);
                    
            foreach (var ufo in _allUfoInstancesInScene)
            {
                Physics.IgnoreCollision(ufo.UfoMeshCollider, 
                    _playerProjectile.ProjectileSphereCollider, false);
            }

            var obj = _playerProjectile.gameObject;
            obj.SetActive(true);
            _playerProjectile.name = "Player Projectile";
            obj.transform.localScale = .1f * Vector3.one;
            _playerProjectile.transform.position = _playerTransform.position;
            _playerProjectile.FacingDirection = _playerTransform.forward;
                    
            _hideProjectileCoroutine = HideProjectileCoroutine(_playerProjectile, _playerProjectile.PlayerProjectileLifespan);
            StartCoroutine(_hideProjectileCoroutine);
            _playerProjectile.DisableHidePlayerProjectileCoroutine = _hideProjectileCoroutine;

            _isPlayerFiringInCooldown = true;
            StartCoroutine(PlayerCanFireProjectileCoroutine());
        }

        private IEnumerator HideProjectileCoroutine(Projectile projectile, float lifespan)
        {
            yield return new WaitForSeconds(lifespan);
            projectile.gameObject.SetActive(false); 
        }
    
        private IEnumerator PlayerCanFireProjectileCoroutine()
        {
            yield return new WaitForSeconds(_playerProjectile.ShootingCooldownLength);
            _isPlayerFiringInCooldown = false;
        }
    }
}


