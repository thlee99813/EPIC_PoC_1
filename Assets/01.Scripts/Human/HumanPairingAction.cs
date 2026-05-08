using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
[RequireComponent(typeof(HumanPairingState))]
public class HumanPairingAction : HumanAction
{
    [SerializeField] private int _priority = 25;
    [SerializeField, Range(0f, 1f)] private float _pairingChance = 0.3f;
    [SerializeField] private int _reservedPairingPriority = 95;


    private HumanPairingState _pairingState;
    private Vector3 _targetPosition;
    private float _timer;
    private bool _isStaying;
    private bool _isFoodPaid;
    private bool _hasBirthReservation;

    public override int Priority => _pairingState != null && _pairingState.HasPairing ? _reservedPairingPriority : _priority;
    public override string StatusText => _isStaying ? HumanActionTextTable.PairingStaying : HumanActionTextTable.MovingToPairing;
    [SerializeField] private int _minInterruptPriority = 100;

    public override bool CanBeInterrupted => true;
    public override int MinInterruptPriority => _minInterruptPriority;




    private void Awake()
    {
        _pairingState = GetComponent<HumanPairingState>();
    }

    public override bool CanRun()
    {
        if (_pairingState.HasPairing)
            return true;

        if (!Agent.Stats.CanReproduce(Agent.Definition))
            return false;

        if (!Agent.SettlementContext.Storage.HasEnough(ResourceType.Food, Agent.Definition.ReproductionFoodCost))
            return false;

        if (Agent.SettlementContext.HouseRegistry.GetNearestVacantHouse(transform.position) == null)
            return false;

        if (Agent.TickSystem.GetAvailablePairingPartner(Agent, Agent.Definition) == null)
            return false;

        float pairingChance = _pairingChance * Agent.SettlementContext.CivilizationModifier.PairingChanceMultiplier;
        return Random.value < pairingChance;
    }

    public override void Begin()
    {
        _isStaying = false;
        _isFoodPaid = false;
        _hasBirthReservation = false;
        _timer = Agent.Definition.ReproductionDuration;

        if (!_pairingState.HasPairing)
            TryStartPairing();

        if (!_pairingState.HasPairing)
            return;

        _targetPosition = _pairingState.TargetHouse.GetRandomAroundPosition(transform.position.y);
    }

    public override bool Tick(float deltaTime)
    {
        if (!_pairingState.HasPairing)
            return true;

        if (!_isStaying)
        {
            bool arrived = MoveTo(_targetPosition, deltaTime);

            if (!arrived)
                return false;

            _isStaying = true;
            _pairingState.SetArrivedAtPairingHouse();
            return false;
        }

        if (!_pairingState.IsInitiator)
            return !_pairingState.HasPairing;

        HumanPairingState partnerState = _pairingState.Partner.GetComponent<HumanPairingState>();

        if (!partnerState.IsAtPairingHouse)
            return false;

        if (!_isFoodPaid)
        {
            if (!Agent.SettlementContext.Storage.HasEnough(ResourceType.Food, Agent.Definition.ReproductionFoodCost))
            {
                FinishPairing(false);
                return true;
            }

            Agent.SettlementContext.Storage.Take(ResourceType.Food, Agent.Definition.ReproductionFoodCost);
            _isFoodPaid = true;
        }

        _timer -= deltaTime;

        if (_timer > 0f)
            return false;

        SpawnChild();
        FinishPairing(true);
        return true;
    }

    public override void End()
    {
        CancelPairing();

        _isStaying = false;
        _isFoodPaid = false;
        _hasBirthReservation = false;
    }
    private void CancelPairing()
    {
        if (_hasBirthReservation && _pairingState.TargetHouse != null)
            _pairingState.TargetHouse.ReleaseBirthSlot();

        HumanAgent partner = _pairingState.Partner;

        if (partner != null)
            partner.GetComponent<HumanPairingState>().ClearPairing();

        _pairingState.ClearPairing();
    }

    private void TryStartPairing()
    {
        House targetHouse = Agent.SettlementContext.HouseRegistry.GetNearestVacantHouse(transform.position);

        if (targetHouse == null || !targetHouse.TryReserveBirthSlot())
            return;

        HumanAgent partner = Agent.TickSystem.GetAvailablePairingPartner(Agent, Agent.Definition);

        if (partner == null)
        {
            targetHouse.ReleaseBirthSlot();
            return;
        }

        HumanPairingState partnerState = partner.GetComponent<HumanPairingState>();

        _hasBirthReservation = true;
        _pairingState.SetPairing(partner, targetHouse, true);
        partnerState.SetPairing(Agent, targetHouse, false);
    }

    private void SpawnChild()
    {
        House targetHouse = _pairingState.TargetHouse;

        targetHouse.ReleaseBirthSlot();
        _hasBirthReservation = false;

        Vector3 spawnPosition = targetHouse.GetRandomAroundPosition(transform.position.y);
        HumanAgent child = Agent.SettlementContext.HumanSpawner.SpawnChild(spawnPosition);
        child.GetComponent<HumanHome>().TrySetHouse(targetHouse);
    }

    private void FinishPairing(bool succeeded)
    {
        HumanAgent partner = _pairingState.Partner;

        if (succeeded)
        {
            Agent.Stats.StartReproductionCooldown(Agent.Definition);

            if (partner != null)
                partner.Stats.StartReproductionCooldown(partner.Definition);
        }

        if (partner != null)
            partner.GetComponent<HumanPairingState>().ClearPairing();

        _pairingState.ClearPairing();
    }
}
