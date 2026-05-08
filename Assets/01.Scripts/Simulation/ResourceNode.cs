using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _amount = 30;
    [SerializeField] private int _gatherAmount = 1;

    [SerializeField] private float _interactionRadius = 2f;

    public ResourceType ResourceType => _resourceType;
    public bool HasResource => _amount > 0;

    public Vector3 GetRandomInteractionPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _interactionRadius;
        return transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    public int Gather()

    {
        if (_amount <= 0)
            return 0;

        int gatheredAmount = Mathf.Min(_gatherAmount, _amount);
        _amount -= gatheredAmount;
        return gatheredAmount;
    }
}
