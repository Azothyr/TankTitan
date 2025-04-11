using UnityEngine;

namespace GameSpecific.Tank.Data
{
    [CreateAssetMenu(fileName = "EnemyTankData", menuName = "Tank/EnemyTankData")]
    public class EnemyTankData : ScriptableObject
    {
        [Header("Enemy stats")]
        public bool isBoss;
        public bool canMove;
        public float health;
        public float speed;
        public float turnSpeed;
        public float fireRate;
        public int maxActiveBullets;
        public int maxActiveMines;

        [Header("Enemy tank parts")]
        public GameObject bulletPrefab;
        public GameObject minePrefab;

        [Header("Enemy tank effects")]
        public GameObject hitEffect;
        public GameObject moveEffect;
        public GameObject fireEffect;
    }
}
