using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu (fileName = "AsteroidData", menuName = "AsteroidData", order = 2)]
    public class AsteroidData : ScriptableObject
    {
        public List<string> names = new List<string>();
        public List<int> scales = new List<int>();
        public float upperRotationSpeed;
        public float lowerRotationSpeed;
    }
}
