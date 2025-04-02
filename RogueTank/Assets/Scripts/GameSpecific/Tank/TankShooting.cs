using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameSpecific.Tank.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSpecific.Tank
{
    public class TankShooting : MonoBehaviour
    {
        [SerializeField] private InputActionReference fireControl;
        [SerializeField] private InputActionReference bombControl;
        [SerializeField] private Transform fireTransform;
        [SerializeField] private Transform bombTransform;
        [SerializeField] private PlayerTankData playerData;
        [SerializeField] private BulletBehavior bulletBehavior;
        [SerializeField] private BombBehavior bombBehavior;
        [SerializeField] private BulletData bulletData;
        [SerializeField] private BulletData bombData;
        [SerializeField] private List<BulletBehavior> bulletPool;
        [SerializeField] private List<BombBehavior> bombPool;
        public bool canShoot;
        private float _timer1;
        private float _timer2;
        private WaitForSeconds _wfsObj;
        
        private void Awake()
        {
            bulletPool = new List<BulletBehavior>();
            for (var i = 0; i < playerData.maxActiveBullets; i++)
            {
                var bullet = Instantiate(bulletBehavior);
                bullet.gameObject.SetActive(false);
                bulletPool.Add(bullet);
            }
            bombPool = new List<BombBehavior>();
            for (var i = 0; i < playerData.maxActiveMines; i++)
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
            if (!canShoot) return;
        
            if (_timer1 >= bulletData.timeBetweenShots && fireControl.action.triggered)
            {
                Fire();
                _timer1 = 0f;
            }
        
            if (_timer2 >= bombData.timeBetweenShots && bombControl.action.triggered)
            {
                Bomb();
                _timer2 = 0f;
            }
        }
        
        private void Fire()
        {
            bulletBehavior.ResetBullet();
            var direction = fireTransform.forward;
            var eulerAngles = fireTransform.eulerAngles;
            var newRotation = Quaternion.Euler(0, eulerAngles.y + 180, 0);
        
            var bullet = GetBullet();
            if (bullet == null) return;
            bullet.transform.position = fireTransform.position;
            bullet.transform.rotation = newRotation;
            bullet.gameObject.SetActive(true);
            bullet.Shoot(direction.normalized);
            bullet.bounce = 0;
        }
        
        private BulletBehavior GetBullet()
        {
            return bulletPool.FirstOrDefault(bullet => !bullet.gameObject.activeInHierarchy);
        }
        
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
        
        public void ToggleShootOn()
        {
            canShoot = true;
        }
        
        public void ToggleShootOff()
        {
            canShoot = false;
        }
        
        public void ResetBullets()
        {
            foreach (var bullet in bulletPool)
            {
                bullet.ResetBullet();
            }
        }
        
        private void OnEnable()
        {
            fireControl.action.Enable();
            bombControl.action.Enable();
        }
        
        private void OnDisable()
        {
            fireControl.action.Disable();
            bombControl.action.Disable();
        }
    }
}
