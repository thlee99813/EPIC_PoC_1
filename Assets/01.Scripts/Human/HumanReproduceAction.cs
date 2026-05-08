using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
[RequireComponent(typeof(HumanHome))]
public class HumanReproduceAction : HumanAction
{
    [SerializeField] private int _priority = 25;
    [SerializeField, Range(0f, 1f)] private float _reproduceChance = 0.3f;


    private HumanHome _home;
    private Vector3 _targetPosition;
    private float _timer;
    private bool _isReproducing;
    private House _birthHouse;
    private bool _hasBirthReservation;



    public override int Priority => _priority;
    public override string StatusText => _isReproducing ? HumanActionTextTable.Reproducing : HumanActionTextTable.MovingToReproduce;

    private void Awake()
    {
        _home = GetComponent<HumanHome>();
    }

    public override bool CanRun()
    {
        if (!_home.HasHouse)
            return false;

        if (!Agent.Stats.CanReproduce(Agent.Definition))
            return false;

        if (!Agent.SettlementContext.Storage.HasEnough(ResourceType.Food, Agent.Definition.ReproductionFoodCost))
            return false;

        if (!Agent.SettlementContext.HouseRegistry.HasVacancy)
            return false;

        if (Agent.TickSystem.GetAdultAgentCount(Agent.Definition) < 2)
            return false;

        return Random.value < _reproduceChance;

    }

    public override void Begin()
    {
        _isReproducing = false;
        _hasBirthReservation = false;
        _timer = Agent.Definition.ReproductionDuration;

        _birthHouse = Agent.SettlementContext.HouseRegistry.GetNearestVacantHouse(transform.position);

        if (_birthHouse == null || !_birthHouse.TryReserveBirthSlot())
            return;

        _hasBirthReservation = true;
        _targetPosition = _birthHouse.GetRandomAroundPosition(transform.position.y);
    }



    public override bool Tick(float deltaTime)
    {
        if (_birthHouse == null || !_hasBirthReservation)
            return true;

        if (!_isReproducing)

        {
            bool arrived = MoveTo(_targetPosition, deltaTime);

            if (!arrived)
                return false;

            if (!Agent.SettlementContext.Storage.HasEnough(ResourceType.Food, Agent.Definition.ReproductionFoodCost))
                return true;

            Agent.SettlementContext.Storage.Take(ResourceType.Food, Agent.Definition.ReproductionFoodCost);
            _isReproducing = true;
            return false;
        }

        _timer -= deltaTime;

        if (_timer > 0f)
            return false;

        Vector3 spawnPosition = _birthHouse.GetRandomAroundPosition(transform.position.y);

        HumanAgent child = Agent.SettlementContext.HumanSpawner.SpawnChild(spawnPosition);
        child.GetComponent<HumanHome>().TrySetHouse(_birthHouse);

        _birthHouse.ReleaseBirthSlot();
        _hasBirthReservation = false;

        Agent.Stats.StartReproductionCooldown(Agent.Definition);

        return true;

    }
    

    public override void End()
    {
        if (_hasBirthReservation && _birthHouse != null)
            _birthHouse.ReleaseBirthSlot();

        _isReproducing = false;
        _hasBirthReservation = false;
        _birthHouse = null;
    }


}
