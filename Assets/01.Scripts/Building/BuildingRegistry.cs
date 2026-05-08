using System.Collections.Generic;
using UnityEngine;

public class BuildingRegistry : MonoBehaviour
{
    private readonly List<BuildingSite> _buildingSites = new List<BuildingSite>();

    public void Register(BuildingSite site)
    {
        if (!_buildingSites.Contains(site))
            _buildingSites.Add(site);
    }

    public void Unregister(BuildingSite site)
    {
        _buildingSites.Remove(site);
    }

    public Vector3[] GetOccupiedPositions()
    {
        Vector3[] positions = new Vector3[_buildingSites.Count];

        for (int i = 0; i < _buildingSites.Count; i++)
            positions[i] = _buildingSites[i].transform.position;

        return positions;
    }

    public BuildingSite GetAvailableBuildingSite(Vector3 position, ResourceStorage storage)
    {
        BuildingSite nearestSite = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < _buildingSites.Count; i++)
        {
            BuildingSite site = _buildingSites[i];

            if (!site.CanBuild(storage))
                continue;

            float distance = Vector3.SqrMagnitude(site.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestSite = site;
        }

        return nearestSite;
    }
}
