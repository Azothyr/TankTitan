using System.Collections.Generic;
using System.Linq;
using GameSpecific.Tank.Data;
using UnityEngine;

namespace GameSpecific.Tank
{
    public class TankShooting : MonoBehaviour
    {
        [SerializeField] private TankData tankData;

        [SerializeField] private BulletBehavior bulletBehavior;
        public BulletData bulletData;
        public Transform fireTransform;
        private ZP_Tools.ObjectPool<BulletBehavior> bulletPool;
        
        [SerializeField] private BombBehavior bombBehavior;
        public BulletData bombData;
        public Transform bombTransform;
        [SerializeField] private List<BombBehavior> bombPool;
        
        public bool canShoot;
        private uint _activeBullets;
        
        private float _timer1;
        private float _timer2;
        private WaitForSeconds _wfsObj;
        
        private void Awake()
        {
            _activeBullets = 0;
            bulletPool = ZP_Tools.PoolManager.Instance.GetPool(bulletBehavior, tankData.stats.maxActiveBullets);
            
            bombPool = new List<BombBehavior>();
            for (var i = 0; i < tankData.stats.maxActiveMines; i++)
            {
                var bomb = Instantiate(bombBehavior);
                bomb.gameObject.SetActive(false);
                bombPool.Add(bomb);
            }
        }
        
        private void Update()
        {
            _timer1 += Time.deltaTime;
            _timer2 += Time.deltaTime;
        }

        public void FirePreformed()
        {
            if (!canShoot || !(_timer1 >= bulletData.timeBetweenShots)) return;
            
            Fire();
            _timer1 = 0f;
        }
        
        public void BombPreformed()
        {
            if (!canShoot) return;
            if (!(_timer2 >= bombData.timeBetweenShots)) return;
            Bomb();
            _timer2 = 0f;
        }
        
        private void Fire()
        {
            if (_activeBullets >= tankData.stats.maxActiveBullets) return;
            _activeBullets++;
            
            bulletBehavior.ResetBullet();
            var direction = fireTransform.forward;
            var rotation = Quaternion.Euler(0, fireTransform.eulerAngles.y + 180, 0);
        
            var bullet = GetBullet();
            if (bullet == null)
            {
                Debug.LogError("Bullet not found.", this);
                return;
            }
            bullet.transform.position = fireTransform.position;
            bullet.transform.rotation = rotation;
            bullet.gameObject.SetActive(true);
            
            bullet.Shoot(direction.normalized, () => 
            { 
                bulletPool.ReturnToPool(bullet);
                _activeBullets--; 
            });
            
            bullet.bounce = 0;
        }
        
        private BulletBehavior GetBullet() => bulletPool.Get();
        
        private BombBehavior GetBomb()
        {
            return bombPool.FirstOrDefault(bomb => !bomb.gameObject.activeInHierarchy);
        }

        private void Bomb()
        {
            //_wfsObj = new WaitForSeconds(bombData.timeBetweenShots);
            bombBehavior.ResetBomb();
            
            var bomb = GetBomb();
            if (bomb == null) return;
            bomb.transform.position = bombTransform.position;
            bomb.transform.rotation = bombTransform.rotation;
            bomb.gameObject.SetActive(true);
            bomb.StartExplosion();
        }
    }
}
