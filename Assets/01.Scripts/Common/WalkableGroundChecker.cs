using UnityEngine;

public class WalkableGroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask _walkableLayerMask;
    [SerializeField] private float _groundCheckHeight = 10f;
    [SerializeField] private float _groundCheckDistance = 30f;

    public bool TryGetWalkablePosition(Vector3 position, out Vector3 walkablePosition)
    {
        Vector3 origin = position + Vector3.up * _groundCheckHeight;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _groundCheckDistance, _walkableLayerMask))
        {
            walkablePosition = new Vector3(position.x, hit.point.y, position.z);
            return true;
        }

        walkablePosition = position;
        return false;
    }

    public bool IsWalkable(Vector3 position)
    {
        return TryGetWalkablePosition(position, out _);
    }
}
