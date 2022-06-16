using System.Collections;
using Models;
using Pools;
using UnityEngine;

namespace Controllers
{
    public class ParticleController : MonoBehaviour
    {
        [SerializeField] private Particle thrustPrefab;
        private GameObject _thrustAttach;
        private Particle _thrustInstance;
        private Particle _explosion;
        private ParticleSystem _partSystem;
        private ParticleSystem.MainModule _main;
        private GameObject _obj;

        private void Start()
        {
            _thrustInstance = Instantiate(thrustPrefab, Vector3.zero, Quaternion.identity);
            _thrustInstance.gameObject.SetActive(false);
            GameManager.sharedInstance.OnPlayerSpawn += AttachThrustToPlayerThrustAttachmentPoint;
            GameManager.sharedInstance.OnPlayerAppliedThrust += EnableThrustEffect;
            GameManager.sharedInstance.OnAsteroidCollisionOccurred += HandleAsteroidExplosion;
            GameManager.sharedInstance.OnPlayerDied += HandlePlayerExplosion;
            GameManager.sharedInstance.OnUfoCollisionOccurred += HandleUfoExplosion;
        }

        private void EnableThrustEffect(bool thrustIsActive, Player player)
        {
            _thrustInstance.gameObject.SetActive(thrustIsActive);
            _thrustInstance.transform.forward = -player.PlayerFacing;
        }

        private void AttachThrustToPlayerThrustAttachmentPoint(Player player)
        {
            _thrustAttach = player.gameObject.transform.GetChild(0).gameObject;
            var thrustTransform = _thrustInstance.transform;
            thrustTransform.parent = _thrustAttach.transform;
            thrustTransform.position = _thrustAttach.transform.position;
        }

        private void DetermineObjects()
        {
            _explosion = ParticlePool.SharedInstance.GetPooledObject();
            _partSystem = _explosion.GetComponent<ParticleSystem>();
            _main = _partSystem.main;
            _obj = _explosion.gameObject;
        }

        private void HandleAsteroidExplosion(Asteroid asteroid)
        {
            DetermineObjects();
            _obj.transform.position = asteroid.transform.position;
            _explosion.name = $"{asteroid.names[asteroid.asteroidSize]} Asteroid Explosion";
            // Explosion scales 0 to 2
            _obj.transform.localScale = _explosion.scales[asteroid.asteroidSize] * Vector3.one;
            _main.startColor = _explosion.AsteroidExplosionColor;
            _obj.SetActive(true);
            StartCoroutine(ExplosionDisableCoroutine(_explosion));
        }

        private void HandlePlayerExplosion(Player player)
        {
            DetermineObjects();
            _obj.transform.position = player.transform.position;
            _explosion.name = $"Player Explosion";
            // Explosion scale 3
            _obj.transform.localScale = _explosion.scales[3] * Vector3.one;
            _main.startColor = _explosion.PlayerExplosionColor;
            _obj.SetActive(true);
            StartCoroutine(ExplosionDisableCoroutine(_explosion));
        }
        
        private void HandleUfoExplosion(Ufo ufo)
        {
            DetermineObjects();
            _obj.transform.position = ufo.transform.position;
            _explosion.name = $"{ufo.names[ufo.UfoSize]} UFO Explosion";
            // Explosion scale 4 and 5
            int explosionScale = 4;
            if (ufo.UfoSize == 1)
            {
                explosionScale = 5;
            }
            _obj.transform.localScale = _explosion.scales[explosionScale] * Vector3.one;
            _main.startColor = _explosion.UfoExplosionColor;
            _obj.SetActive(true);
            StartCoroutine(ExplosionDisableCoroutine(_explosion));
        }

        private IEnumerator ExplosionDisableCoroutine(Particle explosion)
        {
            yield return new WaitForSeconds(explosion.ExplosionDuration);
            explosion.gameObject.SetActive(false);
        }
    }
}
