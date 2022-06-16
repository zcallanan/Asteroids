using System.Collections;
using System.Collections.Generic;
using Models;
using Pools;
using UnityEngine;

namespace Controllers
{
    public class ProjectileController : MonoBehaviour
    {
        private bool _isPlayerFiring;
        private bool _isPlayerFiringInputDisregarded;
        private bool _isPlayerFiringInCooldown;
        private IEnumerator _hideProjectileCoroutine;
        private Projectile _playerProjectile;
        private Projectile _ufoProjectile;
        private Player _playerInstance;
        private Transform _playerTransform;
        private readonly List<Ufo> _allUfoInstancesInScene = new List<Ufo>();

        private void Start()
        {
            GameManager.sharedInstance.OnPlayerSpawn += HandlePlayerSpawn;
            GameManager.sharedInstance.OnPlayerDied += PreventFiring;
            GameManager.sharedInstance.OnHyperspaceTrigger += PreventFiring;
            GameManager.sharedInstance.OnHyperspaceEnd += HandleHyperspaceEnded;
            GameManager.sharedInstance.OnUfoReadyToFire += SpawnUfoProjectile;
            GameManager.sharedInstance.OnUfoCollisionOccurred += HandleUfoDeath;
            _isPlayerFiringInCooldown = false;
        }

        private void HandleUfoDeath(Ufo ufo)
        {
            if (_allUfoInstancesInScene.Contains(ufo))
            {
                _allUfoInstancesInScene.Remove(ufo);
            }
        }

        private void SpawnUfoProjectile(Ufo ufo)
        {
            _ufoProjectile = ProjectilePool.SharedInstance.GetPooledObject();
            if (!_allUfoInstancesInScene.Contains(ufo))
            {
                _allUfoInstancesInScene.Add(ufo);
            }
            
            Physics.IgnoreCollision(ufo.UfoMeshCollider, _ufoProjectile.ProjectileSphereCollider);
            Physics.IgnoreCollision(_playerInstance.PlayerMeshCollider,
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

        private Vector3 DetermineUfoProjectileFacing()
        {
            var position = _playerInstance.transform.position + new Vector3(
                Random.Range(GameManager.sharedInstance.UfoXTargetOffsetLower,
                    GameManager.sharedInstance.UfoXTargetOffsetUpper), 1,
                Random.Range(GameManager.sharedInstance.UfoZTargetOffsetLower,
                    GameManager.sharedInstance.UfoZTargetOffsetUpper));
            var n = Random.Range(0, 10);
            return (n < _ufoProjectile.UfoFireTowardsPlayerFrequency) ? position : -position;;
        }

        private void HandleHyperspaceEnded(Player player)
        {
            EnableFiring();
        }
        
        private void HandlePlayerSpawn(Player playerInstance)
        {
            _playerInstance = playerInstance;
            EnableFiring();
        }
        
        private void EnableFiring()
        {
            _isPlayerFiringInputDisregarded = false;
        }

        private void PreventFiring(Player playerInstance)
        {
            _isPlayerFiringInputDisregarded = true;
            _isPlayerFiring = false;
        }

        private void Update()
        {
            if (_playerInstance)
            {
                _playerTransform = _playerInstance.transform;

                if (!_isPlayerFiringInputDisregarded)
                {
                    _isPlayerFiring = InputController.SharedInstance.IsFiring;
                }
                
                if (_isPlayerFiring && !_isPlayerFiringInCooldown)
                {
                    _playerProjectile = ProjectilePool.SharedInstance.GetPooledObject();
                    PlayerProjectileSetup();
                }
            }
        }

        private void PlayerProjectileSetup()
        {
            
            _playerProjectile.gameObject.layer = 6;
            Physics.IgnoreCollision(_playerInstance.PlayerMeshCollider, _playerProjectile.ProjectileSphereCollider);
                    
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


