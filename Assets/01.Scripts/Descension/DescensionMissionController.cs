using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class DescensionMissionController : MonoBehaviour
{
    [SerializeField] private GameObject _missionRoot;
    [SerializeField] private SettlementContext _settlementContext;
    [SerializeField] private SimulationSpeedController _speedController;
    [SerializeField] private SimulationEventView _resultView;
    [SerializeField] private MainMenuController _mainMenuController;
    [SerializeField] private BibleRecordView _bibleRecordView;


    [SerializeField] private DescensionPlayerController _playerPrefab;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private CinemachineCamera _simulationCamera;
    [SerializeField] private CinemachineCamera _ingameCamera;
    [SerializeField] private int _activeCameraPriority = 20;
    [SerializeField] private int _inactiveCameraPriority = 10;


    [SerializeField] private Transform _arsonistSpawnPoint;
    [SerializeField] private Transform[] _producerSpawnPoints;
    [SerializeField] private Transform[] _hungrySpawnPoints;
    [SerializeField] private Transform _childSpawnPoint;

    [SerializeField] private Transform _storagePoint;
    [SerializeField] private Transform _childSafePoint;


    [SerializeField] private float _missionDuration = 20f;
    [SerializeField] private int _foodLossOnFire = 2;
    [SerializeField] private int _foodLossOnHungryArrival = 1;

    [SerializeField] private int _producerCount = 2;
    [SerializeField] private int _hungryCount = 2;

    private readonly List<HumanAgent> _aliveHumans = new List<HumanAgent>();
    private readonly List<HumanDescensionActor> _missionActors = new List<HumanDescensionActor>();
    private readonly List<HumanDescensionActor> _producers = new List<HumanDescensionActor>();
    private readonly List<HumanDescensionActor> _hungryPeople = new List<HumanDescensionActor>();

    private DescensionPlayerController _player;
    private HumanDescensionActor _arsonist;
    private HumanDescensionActor _child;

    private bool _isActive;
    private bool _storageIgnited;
    private bool _playerKilled;
    private bool _playerSubdued;
    private bool _childRescued;
    private bool _hungryReachedStorage;
    private float _remainingTime;

    private void Start()
    {
        _missionRoot.SetActive(false);
    }




    private void Update()
    {
        if (!_isActive)
            return;

        float deltaTime = Time.unscaledDeltaTime;
        _remainingTime -= deltaTime;

        _arsonist.MissionTick(deltaTime);
        _child.MissionTick(deltaTime);

        for (int i = 0; i < _producers.Count; i++)
            _producers[i].MissionTick(deltaTime);

        for (int i = 0; i < _hungryPeople.Count; i++)
            _hungryPeople[i].MissionTick(deltaTime);

        if (_remainingTime <= 0f)
            EndMission();
    }

    public void BeginMission()
    {
        _speedController.SetDefaultSpeed();
        _missionRoot.SetActive(true);

        ResetMissionState();

        if (!TryAssignMissionRoles())
        {
            _resultView.Show("강림 실패", "강림 임무를 구성할 인간이 부족합니다.");
            return;
        }

        _player = Instantiate(_playerPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation, _missionRoot.transform);
        _player.BeginMission(this, _targetCamera);

        ActivateIngameCamera(_player.transform);

        _isActive = true;
    }

    private bool TryAssignMissionRoles()
    {
        if (_producerSpawnPoints.Length < _producerCount || _hungrySpawnPoints.Length < _hungryCount)
            return false;

        _settlementContext.TickSystem.GetAliveHumans(_aliveHumans);

        int requiredCount = 1 + _producerCount + _hungryCount + 1;


        if (_aliveHumans.Count < requiredCount)
            return false;

        _arsonist = TakeActorAt(0);
        MoveActorToSpawnPoint(_arsonist, _arsonistSpawnPoint);
        _arsonist.BeginRole(this, DescensionActorRole.Arsonist, _storagePoint);
        _missionActors.Add(_arsonist);


        for (int i = 0; i < _producerCount; i++)
        {
            HumanDescensionActor producer = TakeActorAt(0);
            MoveActorToSpawnPoint(producer, _producerSpawnPoints[i]);
            producer.BeginChaseRole(this, DescensionActorRole.Producer, _arsonist);
            _producers.Add(producer);
            _missionActors.Add(producer);
        }



        for (int i = 0; i < _hungryCount; i++)
        {
            HumanDescensionActor hungry = TakeActorAt(0);
            MoveActorToSpawnPoint(hungry, _hungrySpawnPoints[i]);
            hungry.BeginRole(this, DescensionActorRole.Hungry, _storagePoint);
            _hungryPeople.Add(hungry);
            _missionActors.Add(hungry);
        }


        _child = TakeActorAt(0);
        MoveActorToSpawnPoint(_child, _childSpawnPoint);
        _child.BeginRole(this, DescensionActorRole.Child, null);
        _missionActors.Add(_child);


        for (int i = 0; i < _missionActors.Count; i++)
            _missionActors[i].GetComponent<HumanAgent>().SetMissionControlled(true);

        return true;
    }

    private HumanDescensionActor TakeActorAt(int index)
    {
        HumanAgent human = _aliveHumans[index];
        _aliveHumans.RemoveAt(index);
        return human.GetComponent<HumanDescensionActor>();
    }
    private void MoveActorToSpawnPoint(HumanDescensionActor actor, Transform spawnPoint)
    {
        actor.MoveToGroundPosition(spawnPoint.position);

    }

    private void ResetMissionState()
    {
        _remainingTime = _missionDuration;
        _isActive = false;
        _storageIgnited = false;
        _playerKilled = false;
        _playerSubdued = false;
        _childRescued = false;
        _hungryReachedStorage = false;

        _missionActors.Clear();
        _producers.Clear();
        _hungryPeople.Clear();

        _arsonist = null;
        _child = null;
    }

    public void MarkActorReachedTarget(HumanDescensionActor actor)
    {
        if (actor.Role == DescensionActorRole.Arsonist)
            IgniteStorage();

        if (actor.Role == DescensionActorRole.Hungry)
            HungryTakeFood();
    }

    private void IgniteStorage()
    {
        if (_storageIgnited)
            return;

        _storageIgnited = true;
        _settlementContext.Storage.Take(ResourceType.Food, _foodLossOnFire);
    }

    private void HungryTakeFood()
    {
        _hungryReachedStorage = true;
        _settlementContext.Storage.Take(ResourceType.Food, _foodLossOnHungryArrival);
    }

    public void KillActorByNpc(HumanDescensionActor actor)
    {
        actor.Kill();
    }

    public void PlayerKillActor(HumanDescensionActor actor)
    {
        if (actor == null)
            return;

        Debug.Log($"[Descension] PlayerKillActor: {actor.name}, Role: {actor.Role}");

        _playerKilled = true;
        actor.Kill();
    }


    public void PlayerSubdueActor(HumanDescensionActor actor)
    {
        if (actor == null)
            return;

        _playerSubdued = true;
        actor.Subdue();
    }

    public void PlayerInteractActor(HumanDescensionActor actor)
    {
        if (actor == null || actor.Role != DescensionActorRole.Child)
            return;

        _childRescued = true;
        actor.Rescue(_childSafePoint.position);
    }

    private void EndMission()
    {
        _isActive = false;

        ActivateSimulationCamera();

        if (_player != null)
            _player.EndMission();

        for (int i = 0; i < _missionActors.Count; i++)
        {
            _missionActors[i].EndRole();
            _missionActors[i].GetComponent<HumanAgent>().SetMissionControlled(false);
        }

        _bibleRecordView.SetRecord("최초의 금기", BuildBibleRecordText());
        _resultView.Show("강림 결과", BuildResultText());    

    }

    private string BuildResultText()
    {
        List<string> lines = new List<string>();

        lines.Add("결과");
        lines.Add(_arsonist.IsAlive ? "- 방화자 생존" : "- 방화자 사망");

        if (_arsonist.IsSubdued)
            lines.Add("- 플레이어 비살상 개입");

        if (_playerKilled)
            lines.Add("- 플레이어 직접 살상");

        lines.Add(_childRescued ? "- 아이 구조" : "- 아이 방치");

        if (_storageIgnited || _hungryReachedStorage)
            lines.Add("- 식량 손실");
        else
            lines.Add("- 식량 보존");

        lines.Add("");
        lines.Add("삼좌 평가");

        if (_playerKilled)
            lines.Add("- 레티시아의 신임이 올랐습니다.");

        if (_arsonist.IsSubdued)
        {
            lines.Add("- 레티시아의 신임이 조금 올랐습니다.");
            lines.Add("- 베아트리체의 총애가 올랐습니다.");
        }

        if (_childRescued)
            lines.Add("- 베아트리체의 총애가 크게 올랐습니다.");

        if (_storageIgnited || _hungryReachedStorage)
            lines.Add("- 셀레스티아가 이 혼란에 관심을 보입니다.");

        if (!_playerKilled && !_arsonist.IsSubdued && !_childRescued)
            lines.Add("- 삼좌는 침묵했습니다.");

        return string.Join("\n", lines);
    }
    private string BuildBibleRecordText()
    {
        if (_playerKilled)
        {
            return "한 인간이 불을 들고 양식의 자리로 향했다.\n\n" +
                "그때 하늘의 집행관이 내려와 그의 피를 땅에 쏟았다.\n\n" +
                "그날 인간은 금기의 대가를 배웠다.";
        }

        if (_arsonist.IsSubdued && _childRescued)
        {
            return "불을 들고 온 자는 쓰러졌으나 죽지 않았다.\n\n" +
                "그리고 갇혀 있던 아이는 하늘의 손에 이끌려 밖으로 나왔다.\n\n" +
                "그날 인간은 심판과 구원이 함께 내려올 수 있음을 보았다.";
        }

        if (_arsonist.IsSubdued)
        {
            return "집행관은 불을 들고 온 자의 목숨을 거두지 않았다.\n\n";
        }

        if (_childRescued)
        {
            return "불과 굶주림이 우리를 흔들 때,\n\n" +
                "집행관은 가장 작은 자를 먼저 들어 올렸다.\n\n";
        }

        return "하늘은 내려왔으나 그리 오래 머물지 않았다.\n\n" +
            "인간은 불과 굶주림 앞에서 서로의 손을 보았다.\n\n" +
            "그날 세계에 처음으로 금기가 생겼다.";
    }


    private void ActivateIngameCamera(Transform followTarget)
    {
        _ingameCamera.Follow = followTarget;
        _ingameCamera.Priority = _activeCameraPriority;
        _simulationCamera.Priority = _inactiveCameraPriority;
    }

    private void ActivateSimulationCamera()
    {
        _ingameCamera.Follow = null;
        _ingameCamera.Priority = _inactiveCameraPriority;
        _simulationCamera.Priority = _activeCameraPriority;
    }
    public HumanDescensionActor GetNearestMissionActor(Vector3 position, float range)
    {
        HumanDescensionActor nearestActor = null;
        float nearestDistance = range * range;

        for (int i = 0; i < _missionActors.Count; i++)
        {
            HumanDescensionActor actor = _missionActors[i];

            if (!actor.IsActive || !actor.IsAlive)
                continue;

            float distance = Vector3.SqrMagnitude(actor.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestActor = actor;
        }

        return nearestActor;
    }
    public HumanDescensionActor GetNearestMissionActor(Vector3 position, float range, DescensionActorRole role)
    {
        HumanDescensionActor nearestActor = null;
        float nearestDistance = range * range;

        for (int i = 0; i < _missionActors.Count; i++)
        {
            HumanDescensionActor actor = _missionActors[i];

            if (!actor.IsActive || !actor.IsAlive || actor.Role != role)
                continue;

            float distance = Vector3.SqrMagnitude(actor.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestActor = actor;
        }

        return nearestActor;
    }
    
    public void CloseResultAndReturnToMain()
    {
        _resultView.Hide();
        _missionRoot.SetActive(false);
        _mainMenuController.ReturnToMainFromMission();
    }
    
}
