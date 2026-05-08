using UnityEngine;

public class SettlementBuildArea : MonoBehaviour
{
    [SerializeField] private BoxCollider[] _buildAreas;

    public bool TryGetRandomBuildPosition(float minDistance, Vector3[] occupiedPositions, out Vector3 position)
    {
        const int maxTryCount = 30;

        for (int i = 0; i < maxTryCount; i++)
        {
            Vector3 candidate = GetRandomPositionInArea();

            if (IsFarEnough(candidate, minDistance, occupiedPositions))
            {
                position = candidate;
                return true;
            }
        }

        position = Vector3.zero;
        return false;
    }

    private Vector3 GetRandomPositionInArea()
    {
        BoxCollider area = _buildAreas[Random.Range(0, _buildAreas.Length)];
        Bounds bounds = area.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, area.transform.position.y, z);
    }

    private bool IsFarEnough(Vector3 candidate, float minDistance, Vector3[] occupiedPositions)
    {
        float minSqrDistance = minDistance * minDistance;

        for (int i = 0; i < occupiedPositions.Length; i++)
        {
            if (Vector3.SqrMagnitude(candidate - occupiedPositions[i]) < minSqrDistance)
                return false;
        }

        return true;
    }
}
