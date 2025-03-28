using UnityEngine;

namespace GameSpecific.Tank.Data
{
    [CreateAssetMenu(fileName = "BulletData", menuName = "Tank/BulletData")]
    public class BulletData : ScriptableObject
    {
        public GameObject shellPrefab;
        public float speed = 10.0f;
        public float maxBounces = 2.0f;
        public float damage = 10.0f;
        public float timeBetweenShots = 0.5f;
    }
}
