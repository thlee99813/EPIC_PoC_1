using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
[RequireComponent(typeof(HumanHome))]
public class HumanFindHomeAction : HumanAction
{
    [SerializeField] private int _priority = 30;

    public override int Priority => _priority;
    public override string StatusText => HumanActionTextTable.MovingToHouse;

    private HumanHome _home;
    private House _targetHouse;

    private void Awake()
    {
        _home = GetComponent<HumanHome>();
    }

    public override bool CanRun()
    {
        if (_home.HasHouse)
            return false;

        return Agent.SettlementContext.HouseRegistry.GetNearestVacantHouse(transform.position) != null;
    }

    public override void Begin()
    {
        _targetHouse = Agent.SettlementContext.HouseRegistry.GetNearestVacantHouse(transform.position);
    }

    public override bool Tick(float deltaTime)
    {
        if (_targetHouse == null)
            return true;

        bool arrived = MoveTo(_targetHouse.transform.position, deltaTime);

        if (!arrived)
            return false;

        _home.TrySetHouse(_targetHouse);
        return true;
    }

    public override void End()
    {
        _targetHouse = null;
    }
}
