using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private int _capacity = 3;
    [SerializeField] private float _wanderRadius = 4f;

    private readonly List<HumanAgent> _residents = new List<HumanAgent>();

    public bool HasVacancy => _residents.Count + _reservedBirthSlots < _capacity;
    public Vector3 Position => transform.position;
    public int Capacity => _capacity;
    public int ResidentCount => _residents.Count;
    [SerializeField] private int _reservedBirthSlots;


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
    public bool TryReserveBirthSlot()
    {
        if (!HasVacancy)
            return false;

        _reservedBirthSlots++;
        return true;
    }

    public void ReleaseBirthSlot()
    {
        _reservedBirthSlots = Mathf.Max(0, _reservedBirthSlots - 1);
    }



    public Vector3 GetRandomAroundPosition(float groundY)
    {
        Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;
        return new Vector3(transform.position.x + randomCircle.x, groundY, transform.position.z + randomCircle.y);
    }
    public int GetAdultResidentCount(HumanDefinition definition)
    {
        int count = 0;

        for (int i = 0; i < _residents.Count; i++)
        {
            if (_residents[i].Stats.Age >= definition.AdultAge)
                count++;
        }

        return count;
    }

}
