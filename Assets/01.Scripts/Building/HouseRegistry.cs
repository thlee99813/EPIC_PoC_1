using System.Collections.Generic;
using UnityEngine;

public class HouseRegistry : MonoBehaviour
{
    private readonly List<House> _houses = new List<House>();
    public bool HasVacancy => TotalResidentCount < TotalCapacity;

    public int TotalCapacity
    {
        get
        {
            int capacity = 0;

            for (int i = 0; i < _houses.Count; i++)
                capacity += _houses[i].Capacity;

            return capacity;
        }
    }

    public int TotalResidentCount
    {
        get
        {
            int residentCount = 0;

            for (int i = 0; i < _houses.Count; i++)
                residentCount += _houses[i].ResidentCount;

            return residentCount;
        }
    }

    public void Register(House house)
    {
        if (!_houses.Contains(house))
            _houses.Add(house);
    }

    public void Unregister(House house)
    {
        _houses.Remove(house);
    }

    public House GetNearestVacantHouse(Vector3 position)
    {
        House nearestHouse = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < _houses.Count; i++)
        {
            House house = _houses[i];

            if (!house.HasVacancy)
                continue;

            float distance = Vector3.SqrMagnitude(house.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestHouse = house;
        }

        return nearestHouse;
    }
}
