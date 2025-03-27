using UnityEngine;

namespace GameSpecific.Tank.Data
{
    [CreateAssetMenu(fileName = "PlayerTankData", menuName = "Tank/PlayerTankData")]
    public class PlayerTankData : ScriptableObject
    {
        [Header ("Player stats")]
        public int playerNumber;
        public float health;
        public float moveSpeed;
        public float turnSpeed;
        public float fireRate;
        public int maxActiveBullets;
        public float bounceCount;
        public int maxActiveMines;
    
        [Header ("Player tank parts")]
        public GameObject tankBody;
        public GameObject tankWheels;
        public GameObject tankTurret;
        public GameObject tankBarrel;
        public GameObject bulletPrefab;
        public GameObject minePrefab;
    
        [Header ("Player tank effects")]
        public GameObject hitEffect;
        public GameObject moveEffect;
        public GameObject fireEffect;
    }
}
