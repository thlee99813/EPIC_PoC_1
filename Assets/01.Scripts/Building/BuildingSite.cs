using UnityEngine;

public class BuildingSite : MonoBehaviour
{
    [SerializeField] private BuildingDefinition _definition;
    [SerializeField] private GameObject _previewObject;
    [SerializeField] private GameObject _completedObject;
    [SerializeField] private float _currentBuildWork;
    [SerializeField] private bool _isCompleted;
    private BuildingRegistry _registry;


    private bool _isCostPaid;
    private HumanAgent _reservedBuilder;

    public BuildingDefinition Definition => _definition;
    public bool IsCompleted => _isCompleted;
    public bool IsReserved => _reservedBuilder != null;

    private void Awake()
    {
        RefreshVisual();
    }
    public void Initialize(BuildingDefinition definition, BuildingRegistry registry)
    {
        _definition = definition;
        _registry = registry;
        _registry.Register(this);

        RefreshVisual();
    }


    public bool CanBuild(ResourceStorage storage)
    {
        if (_isCompleted || IsReserved)
            return false;

        if (_isCostPaid)
            return true;

        return storage.GetAmount(ResourceType.Wood) >= _definition.WoodCost;
    }

    public bool TryReserve(HumanAgent builder)
    {
        if (_isCompleted || IsReserved)
            return false;

        _reservedBuilder = builder;
        return true;
    }

    public void Release(HumanAgent builder)
    {
        if (_reservedBuilder == builder)
            _reservedBuilder = null;
    }

    public bool PrepareMaterials(ResourceStorage storage)
    {
        if (_isCostPaid)
            return true;

        int takenWood = storage.Take(ResourceType.Wood, _definition.WoodCost);

        if (takenWood < _definition.WoodCost)
        {
            storage.Add(ResourceType.Wood, takenWood);
            return false;
        }

        _isCostPaid = true;
        return true;
    }

    public void AddBuildWork(float amount)
    {
        if (_isCompleted)
            return;

        _currentBuildWork += amount;

        if (_currentBuildWork < _definition.BuildWorkRequired)
            return;

        _isCompleted = true;
        RefreshVisual();
    }

    public Vector3 GetRandomInteractionPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _definition.InteractionRadius;
        return new Vector3(transform.position.x + randomCircle.x, 0f, transform.position.z + randomCircle.y);
    }

    private void OnDestroy()
    {
        if (_registry != null)
            _registry.Unregister(this);
    }


    private void RefreshVisual()
    {
        if (_previewObject != null)
            _previewObject.SetActive(!_isCompleted);

        if (_completedObject != null)
            _completedObject.SetActive(_isCompleted);
    }
}
