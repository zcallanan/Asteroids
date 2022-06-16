using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu (fileName = "UfoData", menuName = "UfoData", order = 5)]
    public class UfoData : ScriptableObject
    {
        public List<string> names = new List<string>();
        public List<float> scales = new List<float>();
        public float ufoProjectileCooldownLengthLower;
        public float ufoProjectileCooldownLengthUpper;
        public float ufoEnforceBoundaryDelayLength;
    }
}
