using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu (fileName = "ParticleData", menuName = "ParticleData", order = 6)]
    public class ParticleData : ScriptableObject
    {
        public ParticleSystem.MinMaxGradient asteroidExplosionColor;
        public ParticleSystem.MinMaxGradient playerExplosionColor;
        public ParticleSystem.MinMaxGradient ufoExplosionColor;
        public float explosionDuration;
        public List<float> scales;
    }
}
