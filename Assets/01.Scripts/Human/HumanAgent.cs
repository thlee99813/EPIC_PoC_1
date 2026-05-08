using UnityEngine;

[RequireComponent(typeof(HumanRuntimeStats))]
public class HumanAgent : MonoBehaviour
{
    [SerializeField] private HumanDefinition _definition;
    [SerializeField] private SettlementContext _settlementContext;
    [SerializeField] private SimulationTickSystem _tickSystem;
    [SerializeField] private string _currentActionName;

    public HumanDefinition Definition => _definition;
    public SettlementContext SettlementContext => _settlementContext;
    public SimulationTickSystem TickSystem => _tickSystem;
    public HumanRuntimeStats Stats => _stats;
    public string CurrentActionName => _currentActionName;
    private bool _hasStatsInitialized;
    private bool _isDeathHandled;

    private HumanRuntimeStats _stats;
    private HumanAction[] _actions;
    private HumanAction _currentAction;
    private float _decisionTimer;
    public void Initialize(SettlementContext settlementContext, SimulationTickSystem tickSystem)
    {
        _settlementContext = settlementContext;
        _tickSystem = tickSystem;

        RegisterToTickSystem();
    }
    public void Initialize(SettlementContext settlementContext, SimulationTickSystem tickSystem, float initialAge)
    {
        _settlementContext = settlementContext;
        _tickSystem = tickSystem;

        _stats.Initialize(_definition, initialAge);
        _hasStatsInitialized = true;

        RegisterToTickSystem();
    }


    private void Awake()
    {
        _stats = GetComponent<HumanRuntimeStats>();
        _actions = GetComponents<HumanAction>();

        for (int i = 0; i < _actions.Length; i++)
            _actions[i].Initialize(this);
    }

    private void OnEnable()
    {
        RegisterToTickSystem();
    }
    private void RegisterToTickSystem()
    {
        if (_tickSystem != null)
            _tickSystem.Register(this);
    }


    private void OnDisable()
    {
        if (_tickSystem != null)
            _tickSystem.Unregister(this);
    }

    private void Start()
    {
        if (!_hasStatsInitialized)
            _stats.Initialize(_definition);

        ResetDecisionTimer();
    }



    public void SimulationTick(float deltaTime)
    {
        if (_stats.IsDead)
        {
            Die();
            return;
        }

        _stats.Tick(_definition, deltaTime);

        if (_stats.IsDead)
        {
            Die();
            return;
        }

        _decisionTimer -= deltaTime;

        if (_decisionTimer <= 0f)
        {
            ResetDecisionTimer();
            TryInterruptCurrentAction();
        }


        if (_currentAction != null)
        {
            bool isFinished = _currentAction.Tick(deltaTime);
            _currentActionName = _currentAction.StatusText;

            if (!isFinished)
                return;

            EndCurrentAction();
        }

        SelectNextAction();

    }
    private void TryInterruptCurrentAction()
    {
        if (_currentAction == null || !_currentAction.CanBeInterrupted)
            return;

        HumanAction betterAction = GetBestRunnableAction();

        if (betterAction == null || betterAction == _currentAction)
            return;

        if (betterAction.Priority <= _currentAction.Priority)
            return;


        if (betterAction.Priority < _currentAction.MinInterruptPriority)
            return;

        EndCurrentAction();
        StartAction(betterAction);


    }




    private void SelectNextAction()
    {
        if (_currentAction != null)
            return;

        HumanAction selectedAction = GetBestRunnableAction();

        if (selectedAction == null)
            return;

        StartAction(selectedAction);
    }
    private HumanAction GetBestRunnableAction()
    {
        HumanAction selectedAction = null;
        int selectedPriority = int.MinValue;

        for (int i = 0; i < _actions.Length; i++)
        {
            HumanAction action = _actions[i];

            if (!action.CanRun() || action.Priority <= selectedPriority)
                continue;

            selectedAction = action;
            selectedPriority = action.Priority;
        }

        return selectedAction;
    }

    private void StartAction(HumanAction action)
    {
        _currentAction = action;
        _currentAction.Begin();
        _currentActionName = action.StatusText;

    }

    private void EndCurrentAction()
    {
        _currentAction.End();
        _currentAction = null;
        _currentActionName = string.Empty;
    }
    private void ResetDecisionTimer()
    {
        _decisionTimer = Random.Range(_definition.DecisionInterval * 0.7f, _definition.DecisionInterval * 1.3f);
    }
    private void Die()
    {
        if (_isDeathHandled)
        return;

        _isDeathHandled = true;
        if (_currentAction != null)
            EndCurrentAction();

        HumanPairingState pairingState = GetComponent<HumanPairingState>();

        if (pairingState.HasPairing && pairingState.Partner != null)
            pairingState.Partner.GetComponent<HumanPairingState>().ClearPairing();

        pairingState.ClearPairing();

        gameObject.SetActive(false);
    }




}
