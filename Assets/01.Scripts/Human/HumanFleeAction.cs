using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
[RequireComponent(typeof(ThreatSensor))]
public class HumanFleeAction : HumanAction
{
    [SerializeField] private int _priority = 120;
    [SerializeField] private float _fleeDistance = 8f;
    [SerializeField] private float _safeDistance = 10f;
    [SerializeField, Range(0f, 1f)] private float _fleeChance = 0.65f;


    private ThreatSensor _sensor;
    private Vector3 _fleeTarget;

    public override int Priority => _priority;
    public override string StatusText => HumanActionTextTable.Fleeing;

    private void Awake()
    {
        _sensor = GetComponent<ThreatSensor>();
    }

    public override bool CanRun()
    {
        _sensor.Refresh();

        if (!_sensor.HasThreat)
            return false;

        float fleeChance = _fleeChance * Agent.SettlementContext.CivilizationModifier.FleeChanceMultiplier;
        return Random.value < fleeChance;
    }

    public override void Begin()
    {
        Vector3 awayDirection = transform.position - _sensor.NearestEnemy.transform.position;
        awayDirection.y = 0f;

        if (awayDirection.sqrMagnitude <= 0.01f)
            awayDirection = Random.insideUnitSphere;

        awayDirection.Normalize();
        _fleeTarget = transform.position + awayDirection * _fleeDistance;
    }

    public override bool Tick(float deltaTime)
    {
        _sensor.Refresh();

        if (!_sensor.HasThreat)
            return true;

        float distance = Vector3.Distance(transform.position, _sensor.NearestEnemy.transform.position);

        if (distance >= _safeDistance)
            return true;

        return MoveTo(_fleeTarget, deltaTime);
    }
}
