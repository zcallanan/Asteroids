using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Models
{
    public class Asteroid : MonoBehaviour
    {
        [SerializeField] private AsteroidData asteroidData;

        public int AsteroidSize { get; set; }
        public List<int> Scales { get; private set; }
        public List<string> Names { get; private set; }

        private DifficultySettings _difficultySetting;
        
        private float _upperRotationSpeed;
        private float _lowerRotationSpeed;
        private float _asteroidSpeedRotationModifier;
        private float _upperMoveSpeed;
        private float _lowerMoveSpeed;
        
        private Vector3 _randomDirectionVector;
        private Vector3 _randomRotationVector;

        private void Awake()
        {
            Scales = asteroidData.scales;
            Names = asteroidData.names;
        }

        private void Start()
        {
            _difficultySetting = GameManager.sharedInstance.difficultySettings;
            _upperMoveSpeed = _difficultySetting.asteroidUpperSpeed;
            _lowerMoveSpeed = _difficultySetting.asteroidLowerSpeed;
            _randomRotationVector = new Vector3(Random.value/10, Random.value/10, Random.value/10);
            _randomDirectionVector = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
            _upperRotationSpeed = asteroidData.upperRotationSpeed;
            _lowerRotationSpeed = asteroidData.lowerRotationSpeed;
            _asteroidSpeedRotationModifier = Random.Range(_lowerRotationSpeed, _upperRotationSpeed);
        }

        private void Update()
        {
            var asteroidTransform = gameObject.transform;
            asteroidTransform.Rotate(_randomRotationVector * (Time.deltaTime * _asteroidSpeedRotationModifier));
            var position = asteroidTransform.position;
            position += _randomDirectionVector * (Time.deltaTime * Random.Range(_lowerMoveSpeed, _upperMoveSpeed));
            position = BoundManager.sharedInstance.EnforceBounds(position);
            transform.position = position;
        }
    }
}
