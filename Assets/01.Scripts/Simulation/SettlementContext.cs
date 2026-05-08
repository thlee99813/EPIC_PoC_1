using UnityEngine;

public class SettlementContext : MonoBehaviour
{
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private ResourceNode[] _resourceNodes;
    [SerializeField] private SettlementBuildArea _buildArea;
    [SerializeField] private BuildingRegistry _buildingRegistry;
    [SerializeField] private BuildingSite _buildingSitePrefab;
    [SerializeField] private BuildingDefinition _houseDefinition;
    [SerializeField] private float _buildingMinDistance = 20f;


    public ResourceStorage Storage => _storage;

    public ResourceNode GetNearestResource(Vector3 position, ResourceType resourceType)
    {
        ResourceNode nearestNode = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < _resourceNodes.Length; i++)
        {
            ResourceNode node = _resourceNodes[i];

            if (!node.HasResource || node.ResourceType != resourceType)
                continue;

            float distance = Vector3.SqrMagnitude(node.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestNode = node;
        }

        return nearestNode;
    }
    public BuildingSite GetAvailableBuildingSite(Vector3 position)
    {
        return _buildingRegistry.GetAvailableBuildingSite(position, _storage);
    }

    public bool TryCreateBuildingSite(out BuildingSite buildingSite)
    {
        if (!_storage.HasEnough(ResourceType.Wood, _houseDefinition.WoodCost))
        {
            buildingSite = null;
            return false;
        }

        Vector3[] occupiedPositions = _buildingRegistry.GetOccupiedPositions();

        if (!_buildArea.TryGetRandomBuildPosition(_buildingMinDistance, occupiedPositions, out Vector3 buildPosition))
        {
            buildingSite = null;
            return false;
        }

        buildingSite = Instantiate(_buildingSitePrefab, buildPosition, Quaternion.identity);
        buildingSite.Initialize(_houseDefinition, _buildingRegistry);
        return true;
    }



}
