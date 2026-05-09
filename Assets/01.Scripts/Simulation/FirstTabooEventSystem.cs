using UnityEngine;

public class FirstTabooEventSystem : MonoBehaviour
{
    [SerializeField] private SettlementContext _settlementContext;
    [SerializeField] private SimulationSpeedController _speedController;
    [SerializeField] private SimulationEventView _eventView;
    [SerializeField] private DescensionMissionController _descensionMissionController;


    [SerializeField] private float _delayAfterObservation = 30f;
    [SerializeField] private int _requiredHumanCount = 6;
    [SerializeField] private int _requiredFoodAmount = 3;
    [SerializeField] private int _stolenFoodAmount = 1;


    [SerializeField] private string _eventTitle = "최초의 금기";

    [SerializeField, TextArea]
    private string _eventBody =
        "한 인간이 다른 인간의 양식에 손을 뻗었다.\n\n그들은 아직 소유를 모른다.\n그들은 아직 죄를 모른다.\n하지만 이제, 처음으로 금기가 태어나려 한다.";

    private bool _isObservationStarted;
    private bool _hasTriggered;
    private float _timer;
    private void OnEnable()
    {
        _eventView.Closed += OnEventClosed;
    }

    private void OnDisable()
    {
        _eventView.Closed -= OnEventClosed;
    }


    public void StartObservation()
    {
        if (_isObservationStarted)
            return;

        _isObservationStarted = true;
        _timer = 0f;
    }

    private void Update()
    {
        if (!_isObservationStarted || _hasTriggered)
            return;

        if (_settlementContext.SettlementDoctrine.CurrentDoctrine == DoctrineType.None)
            return;

        if (_settlementContext.TickSystem.AgentCount < _requiredHumanCount)
            return;

        if (!_settlementContext.Storage.HasEnough(ResourceType.Food, _requiredFoodAmount))
            return;


        _timer += Time.deltaTime;

        if (_timer < _delayAfterObservation)
            return;

        TriggerFirstTaboo();
    }

    private void TriggerFirstTaboo()
    {
        _hasTriggered = true;

        int stolenAmount = _settlementContext.Storage.Take(ResourceType.Food, _stolenFoodAmount);
        _settlementContext.CivilizationStats.AddFear(1);

        string body = $"{_eventBody}\n\n사라진 양식: {stolenAmount}\n공포 +1";

        _speedController.SetPaused();
        _eventView.Show(_eventTitle, body);
    }
    private void OnEventClosed()
    {
        _eventView.Closed -= OnEventClosed;
        _descensionMissionController.BeginMission();
    }


}
