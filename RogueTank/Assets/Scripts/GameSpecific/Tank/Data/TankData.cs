using System.Collections.Generic;
using UnityEngine;

namespace GameSpecific.Tank.Data
{
    [CreateAssetMenu(fileName = "TankData", menuName = "Scriptable Objects/TankData")]
    public class TankData : ScriptableObject
    {
        [SerializeReference, SubclassSelector]
        public TankStats tankData;
    }
    
    [System.Serializable]
    public abstract class TankStats
    {
        [Header("Tank stats")]
        public float health;
        public float moveSpeed;
        public float turnSpeed;
        public float fireRate;
        public int maxActiveBullets;
        public int maxActiveMines;
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
        public bool isStatinary;
    }
}
