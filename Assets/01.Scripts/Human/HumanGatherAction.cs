using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanGatherAction : HumanAction
{
    [SerializeField] private int _foodTargetAmount = 10;
    [SerializeField] private int _woodTargetAmount = 10;

    [SerializeField] private int _foodNeededPriority = 50;
    [SerializeField] private int _woodNeededPriority = 50;
    [SerializeField] private int _foodSurplusPriority = -20;
    [SerializeField] private int _woodSurplusPriority = -20;

    [SerializeField, Range(0f, 1f)] private float _workChanceWhenNeeded = 0.75f;
    [SerializeField, Range(0f, 1f)] private float _workChanceWhenSurplus = 0.15f;


    public override int Priority => GetPriority();

    private ResourceType _targetResourceType;
    private ResourceNode _targetNode;
    private Vector3 _targetGatherPosition;
    private int _carriedAmount;

    private bool _isDelivering;
    private bool _isGathering;
    private bool _hasTarget;

    private float _gatherTimer;

    public override bool CanRun()
    {
        return TrySelectResourceType(out ResourceType resourceType);
    }

   public override void Begin()
    {
        _hasTarget = false;
        _targetNode = null;
        _carriedAmount = 0;
        _isDelivering = false;
        _isGathering = false;
        _gatherTimer = Agent.Definition.GatherDuration;

        if (!TrySelectResourceType(out _targetResourceType))
            return;

        _targetNode = Agent.SettlementContext.GetNearestResource(transform.position, _targetResourceType);

        if (_targetNode == null)
            return;

        _targetGatherPosition = _targetNode.GetRandomInteractionPosition();
        _hasTarget = true;
    }



    public override bool Tick(float deltaTime)
    {
        if (!_hasTarget) return true;

        if (_isDelivering)
        {
            bool arrivedStorage = MoveTo(Agent.SettlementContext.Storage.transform.position, deltaTime);

            if (!arrivedStorage)
                return false;

            Agent.SettlementContext.Storage.Add(_targetResourceType, _carriedAmount);
            return true;
        }

        if (!_isGathering)
        {
            bool arrivedNode = MoveTo(_targetGatherPosition, deltaTime);

            if (!arrivedNode)
                return false;

            _isGathering = true;
            return false;
        }

        _gatherTimer -= deltaTime;


        if (_gatherTimer > 0f)
            return false;

        _targetResourceType = _targetNode.ResourceType;
        _carriedAmount = Mathf.Min(_targetNode.Gather(), Agent.Definition.CarryCapacity);
        _isGathering = false;
        _isDelivering = true;
        return false;

    }

    private int GetPriority()
    {
        if (!TrySelectResourceType(out ResourceType resourceType))
            return int.MinValue;

        if (resourceType == ResourceType.Food)
            return IsNeeded(ResourceType.Food) ? _foodNeededPriority : _foodSurplusPriority;

        return IsNeeded(ResourceType.Wood) ? _woodNeededPriority : _woodSurplusPriority;
    }

    public override string StatusText
    {
        get
        {
            if (_isDelivering)
                return HumanActionTextTable.GetDeliveringText(_targetResourceType);

            if (_isGathering)
                return HumanActionTextTable.GetGatheringText(_targetResourceType);

            return HumanActionTextTable.GetMovingToGatherText(_targetResourceType);
        }
    }




    private bool TrySelectResourceType(out ResourceType resourceType)
    {
        bool hasFoodNode = Agent.SettlementContext.GetNearestResource(transform.position, ResourceType.Food) != null;
        bool hasWoodNode = Agent.SettlementContext.GetNearestResource(transform.position, ResourceType.Wood) != null;

        bool canGatherFood = hasFoodNode && ShouldWork(ResourceType.Food);
        bool canGatherWood = hasWoodNode && ShouldWork(ResourceType.Wood);

        if (canGatherFood && canGatherWood)
        {
            int foodPriority = IsNeeded(ResourceType.Food) ? _foodNeededPriority : _foodSurplusPriority;
            int woodPriority = IsNeeded(ResourceType.Wood) ? _woodNeededPriority : _woodSurplusPriority;

            resourceType = foodPriority >= woodPriority ? ResourceType.Food : ResourceType.Wood;
            return true;
        }

        if (canGatherFood)
        {
            resourceType = ResourceType.Food;
            return true;
        }

        if (canGatherWood)
        {
            resourceType = ResourceType.Wood;
            return true;
        }

        resourceType = ResourceType.Food;
        return false;
    }



    private bool IsNeeded(ResourceType resourceType)
    {
        int currentAmount = Agent.SettlementContext.Storage.GetAmount(resourceType);

        if (resourceType == ResourceType.Food)
            return currentAmount < _foodTargetAmount;

        return currentAmount < _woodTargetAmount;
    }

    private bool ShouldWork(ResourceType resourceType)
    {
        float chance = IsNeeded(resourceType) ? _workChanceWhenNeeded : _workChanceWhenSurplus;
        return Random.value < chance;
    }

}
