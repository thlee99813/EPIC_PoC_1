using UnityEngine;
using UnityEngine.InputSystem;

public class DescensionPlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _attackAction;
    [SerializeField] private InputActionReference _subdueAction;
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private LayerMask _actorLayerMask;
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _actionRange = 5f;

    private DescensionMissionController _missionController;
    private Camera _camera;
    private bool _isActive;

    private void OnEnable()
    {
        _moveAction.action.Enable();
        _attackAction.action.Enable();
        _subdueAction.action.Enable();
        _interactAction.action.Enable();

        _attackAction.action.performed += OnAttackPerformed;
        _subdueAction.action.performed += OnSubduePerformed;
        _interactAction.action.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        _attackAction.action.performed -= OnAttackPerformed;
        _subdueAction.action.performed -= OnSubduePerformed;
        _interactAction.action.performed -= OnInteractPerformed;
    }

    private void Update()
    {
        if (!_isActive)
            return;

        Vector2 input = _moveAction.action.ReadValue<Vector2>();

        Vector3 forward = _camera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = _camera.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 direction = (right * input.x + forward * input.y).normalized;
        transform.position += direction * _moveSpeed * Time.unscaledDeltaTime;
    }

    public void BeginMission(DescensionMissionController missionController, Camera targetCamera)
    {
        _missionController = missionController;
        _camera = targetCamera;
        _isActive = true;
    }

    public void EndMission()
    {
        _isActive = false;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!_isActive)
            return;

        HumanDescensionActor actor = GetNearestActor(DescensionActorRole.Arsonist);
        _missionController.PlayerKillActor(actor);
    }


    private void OnSubduePerformed(InputAction.CallbackContext context)
    {
        if (!_isActive)
            return;

        HumanDescensionActor actor = GetNearestActor();
        _missionController.PlayerSubdueActor(actor);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!_isActive)
            return;

        HumanDescensionActor actor = GetNearestActor(DescensionActorRole.Child);
        _missionController.PlayerInteractActor(actor);
    }
    private HumanDescensionActor GetNearestActor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _actionRange, _actorLayerMask);

        Debug.Log($"[Descension] Found Colliders: {colliders.Length}, Range: {_actionRange}, LayerMask: {_actorLayerMask.value}");

        HumanDescensionActor nearestActor = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; i++)
        {
            HumanDescensionActor actor = colliders[i].GetComponentInParent<HumanDescensionActor>();

            Debug.Log($"[Descension] Hit: {colliders[i].name}, Layer: {LayerMask.LayerToName(colliders[i].gameObject.layer)}, Actor: {actor}");

            if (actor == null || !actor.IsActive || !actor.IsAlive)
                continue;


            float distance = Vector3.SqrMagnitude(actor.transform.position - transform.position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestActor = actor;
        }

        return nearestActor;
    }
    private HumanDescensionActor GetNearestActor(DescensionActorRole role)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _actionRange, _actorLayerMask);
        
        Debug.Log($"[Descension] Find Role: {role}, Found Colliders: {colliders.Length}");
        HumanDescensionActor nearestActor = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; i++)
        {
            HumanDescensionActor actor = colliders[i].GetComponentInParent<HumanDescensionActor>();
            Debug.Log($"[Descension] Role Hit: {colliders[i].name}, Actor: {actor}, ActorRole: {(actor != null ? actor.Role.ToString() : "None")}");

            if (actor == null || !actor.IsActive || !actor.IsAlive || actor.Role != role)
                continue;

            float distance = Vector3.SqrMagnitude(actor.transform.position - transform.position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestActor = actor;
        }

        return nearestActor;
    }


}
