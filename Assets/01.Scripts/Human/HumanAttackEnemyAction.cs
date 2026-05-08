using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
[RequireComponent(typeof(ThreatSensor))]
public class HumanAttackEnemyAction : HumanAction
{
    [SerializeField] private int _priority = 110;
    [SerializeField] private float _attackRange = 1.1f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackInterval = 1.2f;
    [SerializeField, Range(0f, 1f)] private float _fightChance = 0.45f;

    private ThreatSensor _sensor;
    private EnemyAgent _targetEnemy;
    private float _attackTimer;

    public override int Priority => _priority;
    public override string StatusText => HumanActionTextTable.Fighting;

    private void Awake()
    {
        _sensor = GetComponent<ThreatSensor>();
    }

    public override bool CanRun()
    {
        _sensor.Refresh();

        if (!_sensor.HasThreat)
            return false;

        float fightChance = _fightChance * Agent.SettlementContext.CivilizationModifier.FightChanceMultiplier;
        return Random.value < fightChance;
    }

    public override void Begin()
    {
        _targetEnemy = _sensor.NearestEnemy;
        _attackTimer = 0f;
    }

    public override bool Tick(float deltaTime)
    {
        if (_targetEnemy == null || _targetEnemy.IsDead)
            return true;

        Vector3 targetPosition = _targetEnemy.transform.position;
        Vector3 flatTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        float distance = Vector3.Distance(transform.position, flatTarget);

        if (distance > _attackRange)
            return MoveTo(flatTarget, deltaTime);

        _attackTimer -= deltaTime;

        if (_attackTimer > 0f)
            return false;

        _targetEnemy.TakeDamage(_attackDamage);
        _attackTimer = _attackInterval;

        return false;
    }

    public override void End()
    {
        _targetEnemy = null;
    }
}
