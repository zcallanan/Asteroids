using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Models
{
    public class Particle : MonoBehaviour
    {
        [SerializeField] private ParticleData particleData;
        
        public List<float> Scales { get; private set; }
        public ParticleSystem.MinMaxGradient AsteroidExplosionColor { get; private set; }
        public ParticleSystem.MinMaxGradient PlayerExplosionColor { get; private set; }
        public ParticleSystem.MinMaxGradient UfoExplosionColor { get; private set; }
        public float ExplosionDuration { get; private set; }

        private void Awake()
        {
            AsteroidExplosionColor = particleData.asteroidExplosionColor;
            PlayerExplosionColor = particleData.playerExplosionColor;
            UfoExplosionColor = particleData.ufoExplosionColor;
            ExplosionDuration = particleData.explosionDuration;
            Scales = particleData.scales;
        }
    }
    
    
}
