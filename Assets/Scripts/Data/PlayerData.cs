using UnityEngine;

namespace Data
{
    [CreateAssetMenu (fileName = "PlayerData", menuName = "PlayerData", order = 3)]
    public class PlayerData : ScriptableObject
    {
        public float respawnDelay;
        public float invulnerabilityTimer;
        public bool isInvulnerable;
        public float hyperspaceDuration;
        public float shipAlphaValue;
        public float movementSpeed;
        public float movementOverNumberOfFrames;
        public float rotationSpeed;
        public Vector3 rotationAngle;
    }
}
