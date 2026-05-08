using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanEatAction : HumanAction
{
    [SerializeField] private int _priority = 100;
    [SerializeField] private float _hungerThreshold = 70f;

    public override int Priority => _priority;

    public override string StatusText => _isEating ? HumanActionTextTable.EatingFood : HumanActionTextTable.MovingToEatFood;

    private bool _isEating;
    private float _eatTimer;


    public override bool CanRun()
    {
        return Agent.Stats.Hunger >= _hungerThreshold && Agent.SettlementContext.Storage.Food > 0;
    }

    public override void Begin()
    {
        _isEating = false;
        _eatTimer = Agent.Definition.EatDuration;
    }

    public override bool Tick(float deltaTime)
    {
        if (!_isEating)
        {
            bool arrived = MoveTo(Agent.SettlementContext.Storage.transform.position, deltaTime);

            if (!arrived)
                return false;

            _isEating = true;
            return false;
        }

        _eatTimer -= deltaTime;

        if (_eatTimer > 0f)
            return false;

        int eatenAmount = Agent.SettlementContext.Storage.Take(ResourceType.Food, Agent.Definition.EatAmount);

        if (eatenAmount > 0)
            Agent.Stats.RecoverHunger(Agent.Definition.EatHungerRecovery);

        return true;
    }

}
