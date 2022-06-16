using UnityEngine;

namespace Data
{
    [CreateAssetMenu (fileName = "ProjectileData", menuName = "ProjectileData", order = 4)]
    public class ProjectileData : ScriptableObject
    {
        public float playerCooldownLength;
        public float playerProjSpeed;
        public float playerProjLifespan;
        public float ufoProjSpeed;
        public float ufoProjLifespan;
    }
}
