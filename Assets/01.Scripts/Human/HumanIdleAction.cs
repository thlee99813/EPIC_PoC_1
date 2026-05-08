using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanIdleAction : HumanAction
{
    [SerializeField] private int _priority = -100;
    [SerializeField] private float _wanderRadius = 5f;
    [SerializeField] private float _arriveDistance = 0.3f;
    [SerializeField] private Vector2 _restDurationRange = new Vector2(1.5f, 4f);
    [SerializeField] private float _restChance = 0.45f;

    private Vector3 _targetPosition;
    private bool _isResting;
    private float _restTimer;


    public override int Priority => _priority;
    public override bool CanBeInterrupted => true;

    public override string StatusText => _isResting ? HumanActionTextTable.Resting : HumanActionTextTable.Wandering;


    public override bool CanRun()
    {
        return true;
    }

    public override void Begin()
    {
        PickNextTargetPosition();
    }

    public override bool Tick(float deltaTime)
    {
        if (_isResting)
        {
            _restTimer -= deltaTime;

            if (_restTimer <= 0f)
                PickNextTargetPosition();

            return false;
        }

        bool arrived = MoveTo(_targetPosition, deltaTime);

        if (arrived || Vector3.Distance(transform.position, _targetPosition) <= _arriveDistance)
        {
            if (Random.value < _restChance)
                StartRest();
            else
                PickNextTargetPosition();
        }

        return false;
    }
    private void StartRest()
    {
        _isResting = true;
        _restTimer = Random.Range(_restDurationRange.x, _restDurationRange.y);
    }



    private void PickNextTargetPosition()
    {
        _isResting = false;

        Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;
        Vector3 origin = Agent.SettlementContext.Storage.transform.position;

        _targetPosition = origin + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

}
