using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanDescensionActor : MonoBehaviour
{
    [SerializeField] private HumanAgent _humanAgent;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _arriveDistance = 1f;
    [SerializeField] private float _producerKillDistance = 1.5f;
    [SerializeField] private float _groundOffset = 1f;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _arsonistColor = Color.red;
    [SerializeField] private Color _producerColor = Color.yellow;
    [SerializeField] private Color _hungryColor = Color.cyan;
    [SerializeField] private Color _childColor = Color.green;


    public DescensionActorRole Role => _role;
    public bool IsActive => _isActive;
    public bool IsAlive => !_humanAgent.Stats.IsDead;
    public bool IsSubdued => _isSubdued;
    public bool IsRescued => _isRescued;
    public bool HasReachedTarget => _hasReachedTarget;
    private WalkableGroundChecker _groundChecker;
    private DescensionMissionController _missionController;
    private DescensionActorRole _role;
    private HumanDescensionActor _targetActor;
    private Transform _targetPoint;

    private bool _isActive;
    private bool _isSubdued;
    private bool _isRescued;
    private bool _hasReachedTarget;
    private void Awake()
    {
        _humanAgent = GetComponent<HumanAgent>();
        _groundChecker = GetComponent<WalkableGroundChecker>();
        _renderer = GetComponentInChildren<Renderer>();

    }


    public void BeginRole(DescensionMissionController missionController, DescensionActorRole role, Transform targetPoint)
    {
        _missionController = missionController;
        _role = role;
        _targetPoint = targetPoint;
        _targetActor = null;

        ApplyRoleVisual();
        ApplyRoleStatusText();

        _isActive = true;
        _isSubdued = false;
        _isRescued = false;
        _hasReachedTarget = false;
    }

    public void BeginChaseRole(DescensionMissionController missionController, DescensionActorRole role, HumanDescensionActor targetActor)
    {
        _missionController = missionController;
        _role = role;
        _targetActor = targetActor;
        _targetPoint = null;

        ApplyRoleVisual();
        ApplyRoleStatusText();

        _isActive = true;
        _isSubdued = false;
        _isRescued = false;
        _hasReachedTarget = false;
    }


    public void EndRole()
    {
        _humanAgent.ClearOverrideStatusText();

        _isActive = false;
        _targetActor = null;
        _targetPoint = null;
    }


    public void MissionTick(float deltaTime)
    {
        if (!_isActive || !IsAlive || _isSubdued || _isRescued || _hasReachedTarget)
            return;

        if (_role == DescensionActorRole.Child)
            return;

        if (_role == DescensionActorRole.Producer)
        {
            TickProducer(deltaTime);
            return;
        }

        MoveTo(_targetPoint.position, deltaTime);

        if (Vector3.Distance(transform.position, _targetPoint.position) <= _arriveDistance)
        {
            _hasReachedTarget = true;
            _missionController.MarkActorReachedTarget(this);
        }
    }

    private void TickProducer(float deltaTime)
    {
        if (!_targetActor.IsAlive || _targetActor.IsSubdued)
            return;

        MoveTo(_targetActor.transform.position, deltaTime);

        if (Vector3.Distance(transform.position, _targetActor.transform.position) <= _producerKillDistance)
            _missionController.KillActorByNpc(_targetActor);
    }

    private void MoveTo(Vector3 targetPosition, float deltaTime)
    {
        Vector3 currentPosition = transform.position;
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);
        Vector3 nextPosition = Vector3.MoveTowards(currentPosition, flatTargetPosition, _moveSpeed * deltaTime);

        if (_groundChecker.TryGetWalkablePosition(nextPosition, out Vector3 walkablePosition))
        {
            transform.position = walkablePosition + Vector3.up * _groundOffset;
        }
    }


    public void Kill()
    {
        _humanAgent.Stats.TakeDamage(9999f);
    }

    public void Subdue()
    {
        _isSubdued = true;

        if (_role == DescensionActorRole.Arsonist)
            _humanAgent.SetOverrideStatusText(DescensionTextTable.ArsonistSubdued);
        else
            _humanAgent.SetOverrideStatusText("제압됨");
    }



    public void Rescue(Vector3 safePosition)
    {
        _isRescued = true;
        _humanAgent.SetOverrideStatusText(DescensionTextTable.ChildRescued);
        MoveToGroundPosition(safePosition);
    }

    public void MoveToGroundPosition(Vector3 position)
    {
        if (_groundChecker.TryGetWalkablePosition(position, out Vector3 walkablePosition))
        transform.position = walkablePosition + Vector3.up * _groundOffset;
    }
    private void ApplyRoleVisual()
    {
        if (_renderer == null)
            return;

        if (_role == DescensionActorRole.Arsonist)
            _renderer.material.color = _arsonistColor;
        else if (_role == DescensionActorRole.Producer)
            _renderer.material.color = _producerColor;
        else if (_role == DescensionActorRole.Hungry)
            _renderer.material.color = _hungryColor;
        else if (_role == DescensionActorRole.Child)
            _renderer.material.color = _childColor;
    }

    private void ApplyRoleStatusText()
    {
        if (_role == DescensionActorRole.Arsonist)
            _humanAgent.SetOverrideStatusText(DescensionTextTable.ArsonistMoving);
        else if (_role == DescensionActorRole.Producer)
            _humanAgent.SetOverrideStatusText(DescensionTextTable.ProducerChasing);
        else if (_role == DescensionActorRole.Hungry)
            _humanAgent.SetOverrideStatusText(DescensionTextTable.HungryMoving);
        else if (_role == DescensionActorRole.Child)
            _humanAgent.SetOverrideStatusText(DescensionTextTable.ChildWaiting);
    }


}
