using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data;
using UnityEngine;

namespace Models
{
    public class Ufo : MonoBehaviour
    {
        [SerializeField] private UfoData ufoData;
        public bool IsRecentlySpawned { get; set; }
        public int FromSide { get; set; }
        public Vector3 DirectionToFace { get; set; }
        public int UfoSize { get; set; }
        public Coroutine UfoIsReadyToFireCoroutine { get; set; }
        public List<float> scales;
        public List<string> names;
        public float UfoProjectileCooldownLengthLower { get; private set; }
        public float UfoProjectileCooldownLengthUpper { get; private set; }
        public float UfoEnforceBoundaryDelayLength { get; private set; }
        public MeshCollider UfoMeshCollider { get; private set; }
        private DifficultySettings _difficultySettings;
        private float _ufoLowerSpeed;
        private float _ufoUpperSpeed;

        private void Awake()
        {
            scales = ufoData.scales;
            names = ufoData.names;
            UfoProjectileCooldownLengthLower = ufoData.ufoProjectileCooldownLengthLower;
            UfoProjectileCooldownLengthUpper = ufoData.ufoProjectileCooldownLengthUpper;
            UfoEnforceBoundaryDelayLength = ufoData.ufoEnforceBoundaryDelayLength;
            UfoMeshCollider = GetComponent<MeshCollider>();
            _difficultySettings = GameManager.sharedInstance.DifficultySettings;
            _ufoLowerSpeed = _difficultySettings.ufoLowerSpeed;
            _ufoUpperSpeed = _difficultySettings.ufoUpperSpeed;
        }
        private void Update()
        {
            var position = transform.position;
            position +=
                DirectionToFace * (Time.deltaTime * Random.Range(_ufoLowerSpeed, _ufoUpperSpeed));
            if (!IsRecentlySpawned)
            {
                position = BoundManager.sharedInstance.EnforceBounds(position);
            }
            transform.position = position;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var ufoCollider = gameObject.GetComponent<Ufo>();
            if (other.gameObject.GetComponent<Asteroid>())
            {
                GameManager.sharedInstance.AsteroidCollided(other.gameObject.GetComponent<Asteroid>());
            }
            other.gameObject.SetActive(false);
            GameManager.sharedInstance.UfoCollided(ufoCollider);
            gameObject.SetActive(false);
        }
        
    }
}
