using UnityEngine;

namespace GameSpecific.Tank.Data
{
    [CreateAssetMenu(fileName = "TankData", menuName = "Scriptable Objects/TankData")]
    public class TankData : ScriptableObject
    {
        [SerializeReference, SubclassSelector]
        public TankStats stats;
    }
    
    [System.Serializable]
    public abstract class TankStats
    {
        [Header("Tank stats")]
        public float health = 1f;
        public float moveSpeed = 1f;
        public float turnSpeed = 1f;
        public float fireRate = 1f;
        public int maxActiveBullets = 1;
        public int maxActiveMines = 1;
        
        [Header("Tank parts")]
        public GameObject bulletPrefab;
        public GameObject minePrefab;
        public GameObject hitEffect;
        public GameObject moveEffect;
        public GameObject fireEffect;
    }
    
    [System.Serializable]
    public class PlayerTankStats : TankStats
    {
        [Header("Player stats")]
        public int playerNumber;
        public GameObject tankBody;
        public GameObject tankWheels;
        public GameObject tankTurret;
        public GameObject tankBarrel;
    }
    
    [System.Serializable]
    public class EnemyTankStats : TankStats
    {
        [Header("Enemy stats")]
        public bool isBoss;
        public bool isStationary;
    }
}
