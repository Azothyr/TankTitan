using GameSpecific.Tank.Data;
using UnityEngine;

namespace GameSpecific.Tank
{
    public class BulletBehavior : MonoBehaviour
    {
        [SerializeField] private BulletData bulletData;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private LayerMask destructibleLayer;
        private Vector3 _direction;
        public float bounce;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (bounce >= bulletData.maxBounces)
            {
                ResetBullet();
            }
            if (collision.gameObject.layer == 6)
            {
                collision.gameObject.SetActive(false);
                ResetBullet();
            }
            else if ((destructibleLayer.value & (1 << collision.gameObject.layer)) != 0)
            {
                collision.gameObject.SetActive(false);
                ResetBullet();
            }
            else
            {
                var firstContact = collision.GetContact(0);
                Vector3 newVelocity = Vector3.Reflect(_direction.normalized, firstContact.normal);
                Shoot(newVelocity.normalized);
                
                Vector3 newForward = newVelocity.normalized;
                Quaternion rotationToApply = Quaternion.LookRotation(newForward) * Quaternion.Euler(0, 180, 0);
                
                transform.rotation = rotationToApply;
                
                bounce++;
            }
        }

        public void Shoot(Vector3 dir)
        {
            _direction = dir;
            rb.linearVelocity = dir * bulletData.speed;
        }
        
        public void ResetBullet()
        {
            gameObject.SetActive(false);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
