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
        [SerializeField] private BulletData bulletData;
        [SerializeField] private BulletData bombData;
        [SerializeField] private List<BulletBehavior> bulletPool;
        public bool canShoot;
        private float _timer;
        
        private void Awake()
        {
            bulletPool = new List<BulletBehavior>();
            for (var i = 0; i < playerData.maxActiveBullets; i++)
            {
                var bullet = Instantiate(bulletBehavior);
                bullet.gameObject.SetActive(false);
                bulletPool.Add(bullet);
            }
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (!canShoot) return;
            if (!(_timer >= bulletData.timeBetweenShots)) return;
            if (!fireControl.action.triggered) return;
            Fire();
            _timer = 0f;
            // if (_timer >= bombData.timeBetweenShots)
            // {
            //     if (bombControl.action.triggered)
            //     {
            //         Bomb();
            //         _timer = 0f;
            //     }
            // }
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
        
        // private void Bomb()
        // {
        //     
        // }
        
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
