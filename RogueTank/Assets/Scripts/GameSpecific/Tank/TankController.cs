using GameSpecific.Tank.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSpecific.Tank
{
    public abstract class TankController : MonoBehaviour
    {
        [SerializeField] protected TankData tankData;
        [SerializeField] protected TankShooting tankShooting;
        
        [SerializeField] protected GameObject barrel;
        
        protected abstract void Move();
        protected abstract void Turn();
        protected abstract void ResetTank();
    }
}
