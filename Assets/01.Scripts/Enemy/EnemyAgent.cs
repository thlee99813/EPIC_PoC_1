using UnityEngine;

public class EnemyAgent : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyDefinition _definition;
    [SerializeField] private SimulationTickSystem _tickSystem;
    [SerializeField] private float _health;
    [SerializeField] private float _patrolRadius = 8f;
    [SerializeField] private float _patrolArriveDistance = 0.5f;

    private Vector3 _spawnPosition;
    private Vector3 _patrolTarget;
    private WalkableGroundChecker _groundChecker;



    private HumanAgent _targetHuman;
    private float _attackTimer;

    public bool IsDead => _health <= 0f;

    public void Initialize(SimulationTickSystem tickSystem)
    {
        _tickSystem = tickSystem;
        _health = _definition.MaxHealth;
        _groundChecker = GetComponent<WalkableGroundChecker>();
        _spawnPosition = transform.position;
        PickPatrolTarget();
        _tickSystem.RegisterEnemy(this);
    }



    private void OnDisable()
    {
        if (_tickSystem != null)
            _tickSystem.UnregisterEnemy(this);
    }

    public void SimulationTick(float deltaTime)
    {
        if (IsDead)
        {
            gameObject.SetActive(false);
            return;
        }

        if (_targetHuman == null || _targetHuman.Stats.IsDead)
            _targetHuman = _tickSystem.GetNearestHuman(transform.position, _definition.DetectRange);

        if (_targetHuman == null)
        {
            Patrol(deltaTime);
            return;
        }

        MoveOrAttack(deltaTime);
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0f)
        gameObject.SetActive(false);
    }

    private void MoveOrAttack(float deltaTime)
    {
        Vector3 targetPosition = _targetHuman.transform.position;
        Vector3 flatTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        float distance = Vector3.Distance(transform.position, flatTarget);

        if (distance > _definition.AttackRange)
        {
            MoveTo(flatTarget, _definition.MoveSpeed, deltaTime);
            return;
        }

        _attackTimer -= deltaTime;

        if (_attackTimer > 0f)
            return;

        _targetHuman.Stats.TakeDamage(_definition.AttackDamage);
        _attackTimer = _definition.AttackInterval;
    }
    private void Patrol(float deltaTime)
    {
        Vector3 flatTarget = new Vector3(_patrolTarget.x, transform.position.y, _patrolTarget.z);

        MoveTo(flatTarget, _definition.MoveSpeed * 0.5f, deltaTime);


        if (Vector3.Distance(transform.position, flatTarget) <= _patrolArriveDistance)
            PickPatrolTarget();
    }

    private void PickPatrolTarget()
    {
        const int maxTryCount = 20;

        for (int i = 0; i < maxTryCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _patrolRadius;
            Vector3 candidate = new Vector3(_spawnPosition.x + randomCircle.x, transform.position.y, _spawnPosition.z + randomCircle.y);

            if (_groundChecker.TryGetWalkablePosition(candidate, out Vector3 walkablePosition))
            {
                _patrolTarget = walkablePosition;
                return;
            }
        }

        _patrolTarget = transform.position;
    }

    private bool MoveTo(Vector3 targetPosition, float moveSpeed, float deltaTime)
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * deltaTime);

        if (_groundChecker.TryGetWalkablePosition(nextPosition, out Vector3 walkablePosition))
        {
            transform.position = walkablePosition;
            return true;
        }

        return false;
    }


}
