using GameSpecific.Tank.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSpecific.Tank
{
    public class TankController : MonoBehaviour
    {
        //[SerializeField] private PlayerTankStats playerTank;
        [SerializeField] private TankData playerTank;
        [SerializeField] private TankShooting tankShooting;
        private Rigidbody _rb;
        [SerializeField] private GameObject barrel;
        [SerializeField] private InputActionReference moveControl;
        [SerializeField] private InputActionReference turnControl;
        [SerializeField] private InputActionReference fireControl;
        [SerializeField] private InputActionReference bombControl;
        
        private Vector2 _movementInput;
        private Vector2 _turnInput;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            ResetTank();
        }
        
        public void OnEnable()
        {
            _rb.linearVelocity = Vector3.zero;
            moveControl.action.Enable();
            turnControl.action.Enable();
            
            moveControl.action.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            moveControl.action.canceled += ctx => _movementInput = Vector2.zero;
            turnControl.action.performed += ctx => _turnInput = ctx.ReadValue<Vector2>();
            turnControl.action.canceled += ctx => _turnInput = Vector2.zero;
            
            fireControl.action.Enable();
            bombControl.action.Enable();
            
            fireControl.action.performed += ctx => tankShooting.FirePreformed();
            bombControl.action.performed += ctx => tankShooting.BombPreformed();
        }
        
        public void OnDisable()
        {
            _rb.linearVelocity = Vector3.zero;
            moveControl.action.Disable();
            turnControl.action.Disable();
            
            moveControl.action.performed -= ctx => _movementInput = ctx.ReadValue<Vector2>();
            turnControl.action.performed -= ctx => _turnInput = ctx.ReadValue<Vector2>();
            
            fireControl.action.Disable();
            bombControl.action.Disable();
        }

        private void Update()
        {
            // Convert mouse position to world space
            if (Camera.main != null)
            {
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    var targetPosition = hitInfo.point;
                    targetPosition.y = barrel.transform.position.y; // Keep the barrel's height constant
                    var direction = targetPosition - barrel.transform.position;
                    
                    // Rotate the barrel to face the mouse
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    lookRotation *= Quaternion.Euler(0, 90, 0);
                    barrel.transform.rotation = lookRotation;
                }
            }
        }

        private void FixedUpdate()
        {
            Move();
            Turn();
        }
        
        private void Move()
        {
            if (_movementInput == Vector2.zero) return;
            var move = _movementInput.y * playerTank.tankData.moveSpeed * Time.deltaTime;
            var movement = transform.forward * move;
            _rb.MovePosition(_rb.position + movement);
        }
        
        private void Turn()
        {
            // var turn = _turnInput.x;
            // var turnSpeed = playerTankData.moveSpeed * 2;
            // var turnAngle = turn * turnSpeed * Time.fixedDeltaTime;
            // var turnRotation = Quaternion.Euler(0, turnAngle, 0);
            // _rb.MoveRotation(_rb.rotation * turnRotation);

            if (_turnInput == Vector2.zero) return;
            var turn = _turnInput.x * playerTank.tankData.turnSpeed * Time.deltaTime;
            var turnRotation = Quaternion.Euler(0f, turn, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);
        }
        
        private void ResetTank()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }
}
