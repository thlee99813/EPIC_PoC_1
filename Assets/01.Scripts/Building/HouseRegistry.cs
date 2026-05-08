using System.Collections.Generic;
using UnityEngine;

public class HouseRegistry : MonoBehaviour
{
    private readonly List<House> _houses = new List<House>();

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
