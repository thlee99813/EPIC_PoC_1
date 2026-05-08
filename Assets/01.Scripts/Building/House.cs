using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private int _capacity = 2;
    [SerializeField] private float _wanderRadius = 4f;

    private readonly List<HumanAgent> _residents = new List<HumanAgent>();

    public bool HasVacancy => _residents.Count < _capacity;
    public Vector3 Position => transform.position;

    public bool TryAddResident(HumanAgent human)
    {
        if (!HasVacancy || _residents.Contains(human))
            return false;

        _residents.Add(human);
        return true;
    }

    public void RemoveResident(HumanAgent human)
    {
        _residents.Remove(human);
    }

    public Vector3 GetRandomAroundPosition(float groundY)
    {
        Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;
        return new Vector3(transform.position.x + randomCircle.x, groundY, transform.position.z + randomCircle.y);
    }
}
