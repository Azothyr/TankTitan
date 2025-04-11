using System.Collections;
using GameSpecific.Tank;
using GameSpecific.Tank.Data;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameAction startTankAction;
    [SerializeField] private Transform target;
    [SerializeField] private TankData enemyTank;
    [SerializeField] private TankShooting tankShooting;
    [SerializeField] private BulletData weaponData;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float walkPointRange;
    [SerializeField] private LayerMask groundLayer, targetLayer;
    [SerializeField] private float searchAngleRange = 180;
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    public Vector3 startingPosition;
    
    private float _pathUpdateDeadline;
    private Vector3 _walkPoint;
    private bool _walkPointSet;
    private bool _isRotating;
    private Vector3 _direction;
    private bool _alreadyAttacked;
    private bool _bombTriggered;
    private bool _canMove = true;
    private float _attackTimer;
    private float _bombTimer;
    
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;
        agent.speed = enemyTank.tankData.moveSpeed;
        agent.stoppingDistance = stoppingDistance;
    }
    
    private void OnEnable()
    {
        startTankAction.RaiseEvent += TankStart;
    }

    private void TankStart(GameAction _)
    {
        Debug.Log("StartTank");
        EnemyTankStats enemyTankStats = GetComponent<EnemyTankStats>();
        
        if (enemyTankStats.isStatinary)
        {
            Debug.Log("isStationary");
            agent.isStopped = true;
            _canMove = false;
        }
        else
        {
            Debug.Log("isMoving");
            agent.isStopped = false;
            _canMove = true;
            StartCoroutine(TankMovement());
        }
    }
    
    private void TankStop(GameAction _)
    {
        Debug.Log("StopTank");
        agent.isStopped = true;
        _canMove = false;
    }
    
    private IEnumerator TankMovement()
    {
        while (_canMove)
        {
            Vector3 position = transform.position;
            bool playerInSightRange = Physics.CheckSphere(position, sightRange, targetLayer);
            bool playerInAttackRange = Physics.CheckSphere(position, attackRange, targetLayer);
            Patrolling();
            if (playerInSightRange && playerInAttackRange)
            {
                RotateBarrel();
            }

            if (CanSeePlayer())
            {
                _isRotating = false;
                if (_attackTimer >= enemyTank.tankData.fireRate)
                {
                    AttackPlayer();
                    _attackTimer = 0f;
                }
                if (playerInSightRange && _bombTriggered)
                {
                    if (_bombTimer >= weaponData.timeBetweenShots)
                    {
                        tankShooting.BombPreformed();
                        _bombTimer = 0f;
                    }
                }
            }
            yield return null;
        }
    }
    
    private void Patrolling()
    {
        if (!_walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(_walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
    }
    
    private void SearchWalkPoint()
    {
        float randomAngle = Random.Range(-searchAngleRange / 2, searchAngleRange / 2);
        float randomDistance = Random.Range(0, walkPointRange);
    
        Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
        _walkPoint = transform.position + direction * randomDistance;
    
        if (Physics.Raycast(_walkPoint, -transform.up, 2f, groundLayer))
        {
            _walkPointSet = true;
        }
    }
     
    private void FollowPlayer()
    {
        agent.SetDestination(target.position);
    }

    private void FindPlayer()
    {
        const int maxReflections = 1;
        int reflections = 0;

        var transform1 = tankShooting.fireTransform.transform;
        Vector3 rayDirection = transform1.up;
        Vector3 nextStartPosition = transform1.position;
        RaycastHit hit;
        while (reflections <= maxReflections)
        {
            if (Physics.Raycast(nextStartPosition, rayDirection, out hit))
            {
                Debug.DrawRay(nextStartPosition, rayDirection * hit.distance,
                    reflections == 0 ? Color.red : Color.green);
                nextStartPosition = hit.point;

                if (hit.transform == target)
                {
                    // If the ray hits the player, perform the desired action
                    _isRotating = false;
                    //StopCoroutine(RotateTimer());
                    AttackPlayer();
                    break;
                }
                else
                {
                    // Calculate the reflection vector
                    rayDirection = Vector3.Reflect(rayDirection, hit.normal);
                    reflections++;
                }
            }
            else
            {
                // If the ray does not hit anything, stop the loop
                break;
            }
        }
    }
    
    // private void UpdatePath()
    // {
    //     if(Time.time >= _pathUpdateDeadline)
    //     {
    //         Debug.Log("Updating Path");
    //         _pathUpdateDeadline = Time.time + _pathUpdateDelay;
    //         agent.SetDestination(target.position);
    //     }
    // }
    
    private void AttackPlayer()
    {
        //barrelPrefab.transform.LookAt(player);
        if (!_alreadyAttacked)
        {
            // Attack code here
            tankShooting.FirePreformed();
            _alreadyAttacked = true;
            //Invoke(nameof(ResetAttack), enemyTankData.timeBetweenShots);
        }
    }
    
    private IEnumerator RotateTimer()
    {
        yield return new WaitForSeconds(5f);
        RandomRotateBarrel();
    }

    private void RotateBarrel()
    {
        Vector3 targetPosition = target.position;
        var position = tankShooting.fireTransform.position;
        targetPosition.y = position.y;
        _direction = targetPosition - position;

        Quaternion lookRotation = Quaternion.LookRotation(_direction);
        lookRotation *= Quaternion.Euler(0, 90, 0);
        tankShooting.fireTransform.rotation = Quaternion.Slerp(tankShooting.fireTransform.rotation, lookRotation, Time.deltaTime * enemyTank.tankData.turnSpeed);
    }

    private void RandomRotateBarrel()
    {
        if (!_isRotating)
        {
            float randomAngle = Random.Range(0f, 360f);
            Quaternion newRotation = Quaternion.Euler(0f, randomAngle, 0f);
            StartCoroutine(SmoothRotate(newRotation));
        }
    }

    private IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        _isRotating = true;
        float elapsedTime = 0f;
        float duration = 5f; // Adjust duration as needed for smoother transitions
        Quaternion startRotation = tankShooting.fireTransform.rotation;

        while (elapsedTime < duration)
        {
            tankShooting.fireTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tankShooting.fireTransform.rotation = targetRotation;
        _isRotating = false;
    }
    
    private bool CanSeePlayer()
    {
        _direction = target.position - transform.position;
        if (Physics.Raycast(transform.position, _direction, out var hit))
        {
            if (hit.transform == target)
            {
                return true;
            }
        }
        return false;
    }
    
    public void ResetTank()
    {
        //transform.position = startingPosition;
        agent.Warp(startingPosition);
        _canMove = true;
    }
}