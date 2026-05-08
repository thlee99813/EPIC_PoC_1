using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanBuildAction : HumanAction
{
    [SerializeField] private int _priority = 40;
    [SerializeField] private float _buildPowerPerSecond = 1f;
    [SerializeField, Range(0f, 1f)] private float _buildChance = 0.4f;



    public override int Priority => _priority;
    public override string StatusText => _isBuilding ? HumanActionTextTable.BuildingHouse : HumanActionTextTable.MovingToBuildHouse;

    private BuildingSite _targetSite;
    private Vector3 _targetBuildPosition;
    private bool _isBuilding;

    public override bool CanRun()
    {
        if (Agent.SettlementContext.GetAvailableBuildingSite(transform.position) != null)
            return true;

        int humanCount = Agent.TickSystem.AgentCount;
        int houseCapacity = Agent.SettlementContext.HouseRegistry.TotalCapacity;

        int freeCapacity = houseCapacity - humanCount;

        if (freeCapacity >= 1)
            return false;

        if (!Agent.SettlementContext.Storage.HasEnough(ResourceType.Wood, Agent.SettlementContext.HouseDefinition.WoodCost))
            return false;

        return Random.value < _buildChance;
    }


    public override void Begin()
    {
        _targetSite = Agent.SettlementContext.GetAvailableBuildingSite(transform.position);

        if (_targetSite == null)
        {
            if (!Agent.SettlementContext.TryCreateBuildingSite(out _targetSite))
                return;
        }

        _isBuilding = false;

        if (_targetSite == null || !_targetSite.TryReserve(Agent))
            return;

        _targetBuildPosition = _targetSite.GetRandomInteractionPosition();

    }

    public override bool Tick(float deltaTime)
    {
        if (_targetSite == null || _targetSite.IsCompleted)
            return true;

        if (!_isBuilding)
        {
            bool arrived = MoveTo(_targetBuildPosition, deltaTime);
            if (!arrived)
                return false;

            if (!_targetSite.PrepareMaterials(Agent.SettlementContext.Storage))
                return true;

            _isBuilding = true;
            return false;
        }

        _targetSite.AddBuildWork(_buildPowerPerSecond * deltaTime);
        return _targetSite.IsCompleted;
    }

    public override void End()
    {
        if (_targetSite != null)
            _targetSite.Release(Agent);

        _targetSite = null;
        _isBuilding = false;
    }

}
