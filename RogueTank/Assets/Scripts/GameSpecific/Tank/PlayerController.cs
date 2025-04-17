using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSpecific.Tank
{
    public class PlayerController : TankController
    {
        private Rigidbody _rb;
        [SerializeField] private InputActionReference moveControl;
        [SerializeField] private InputActionReference turnControl;
        [SerializeField] private InputActionReference fireControl;
        [SerializeField] private InputActionReference bombControl;
        
        private Vector3 _moveTarget;
        private Quaternion _turnTarget;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
            {
                Debug.LogError("Rigidbody not found on this GameObject.", this);
            }
            
            if (tankData == null)
            {
                Debug.LogError("Tank data is not assigned.", this);
            }

            if (tankShooting == null)
            {
                Debug.LogError("Tank shooting is not assigned.", this);
            }
        }
        
        private void Start()
        {
            // ResetTank();
        }
        
        public void OnEnable()
        {
            _rb.linearVelocity = Vector3.zero;
            moveControl.action.Enable();
            turnControl.action.Enable();

            moveControl.action.performed += HandleMoveInput;
            turnControl.action.performed += HandleTurnInput;
            
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
            
            moveControl.action.performed -= HandleMoveInput;
            turnControl.action.performed -= HandleTurnInput;
            
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

        private bool _wallCollision;
        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.layer)
            {
                case 10:
                    _wallCollision = true;
                    break;
                default:
                    break;
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            switch (collision.gameObject.layer)
            {
                case 10:
                    _wallCollision = false;
                    break;
                default:
                    break;
            }
        }

        private bool _isMoving;
        private void HandleMoveInput(InputAction.CallbackContext context)
        {
            if (_isMoving) return;
            _isMoving = true;
            
            StartCoroutine(PerformAction(context, MoveAction, StopMoveAction));
        }
        
        private void MoveAction(InputAction.CallbackContext context)
        {
            var unbiasedMove = _wallCollision ? -transform.forward * 5 : transform.forward;
            _moveTarget = unbiasedMove * context.ReadValue<Vector2>().y * tankData.stats.moveSpeed * Time.deltaTime;
            Move();
        }

        private void StopMoveAction()
        {
            _moveTarget = Vector3.zero;
            Move();
            _isMoving = false;
        }

        protected override void Move()
        {
            _rb?.MovePosition(_rb.position + _moveTarget);
        }

        private bool _isTurning;
        private void HandleTurnInput(InputAction.CallbackContext context)
        {
            if (_isTurning) return;
            _isTurning = true;
            
            StartCoroutine(PerformAction(context, TurnAction, StopTurnAction));
        }
        
        private void TurnAction(InputAction.CallbackContext context)
        {
            _turnTarget = Quaternion.Euler(0f, context.ReadValue<Vector2>().x * tankData.stats.turnSpeed * Time.deltaTime, 0f);
            Turn();
        }

        private void StopTurnAction()
        {
            _turnTarget = Quaternion.identity;
            Turn();
            _isTurning = false;
        }
        
        protected override void Turn()
        {
            _rb?.MoveRotation(_rb.rotation * _turnTarget);
        }
        
        private IEnumerator PerformAction(InputAction.CallbackContext context, System.Action<InputAction.CallbackContext> action,
            System.Action stopAction)
        {
            var inputValue = context.ReadValue<Vector2>();

            while (inputValue != Vector2.zero)
            {
                action(context);
                yield return null;
                inputValue = context.ReadValue<Vector2>();
            }

            stopAction();
        }
        
        protected override void ResetTank()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }
}