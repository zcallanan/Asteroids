using System.Collections;
using Controllers;
using Data;
using UnityEngine;

namespace Models
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileData projectileData;
        
        public IEnumerator DisableHidePlayerProjectileCoroutine { get; set; }
        public Vector3 FacingDirection { get; set; }
        public float PlayerProjectileLifespan { get; private set; }
        public float UfoProjectileLifespan { get; private set; }
        public float UfoFireTowardsPlayerFrequency { get; private set; }
        public float ShootingCooldownLength { get; private set; }
        public SphereCollider ProjectileSphereCollider { get; private set; }
        
        private float _playerProjectileSpeed;
        private float _ufoProjectileSpeed;
        private DifficultySettings _difficultySetting;
        
        private void Awake()
        {
            _difficultySetting = GameManager.sharedInstance.difficultySettings;
            UfoFireTowardsPlayerFrequency = _difficultySetting.ufoFireTowardsPlayerFrequency;
            
            ProjectileSphereCollider = GetComponent<SphereCollider>();

            PlayerProjectileLifespan = projectileData.playerProjLifespan;
            UfoProjectileLifespan = projectileData.ufoProjLifespan;
            ShootingCooldownLength = projectileData.playerCooldownLength;
            
            _playerProjectileSpeed = projectileData.playerProjSpeed;
            _ufoProjectileSpeed = projectileData.ufoProjSpeed;
        }

        private Vector3 MoveProjectile(float speed)
        {
            var position = transform.position;
            position += FacingDirection * (Time.deltaTime * speed);
            return BoundManager.sharedInstance.EnforceBounds(position);
        }

        private void Update()
        {
            var projectileTransform = transform;
            projectileTransform.position = gameObject.layer switch
            {
                6 => MoveProjectile(_playerProjectileSpeed),
                12 => MoveProjectile(_ufoProjectileSpeed),
                _ => projectileTransform.position
            };
        }

        private void OnTriggerEnter(Collider other)
        {
            gameObject.SetActive(false);
            StopCoroutine(DisableHidePlayerProjectileCoroutine);
        }
    }
}
