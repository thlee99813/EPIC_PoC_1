using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanHome : MonoBehaviour
{
    [SerializeField] private House _house;

    public House House => _house;
    public bool HasHouse => _house != null;

    public bool TrySetHouse(House house)
    {
        if (_house == house)
            return true;

        HumanAgent human = GetComponent<HumanAgent>();

        if (house != null && !house.TryAddResident(human))
            return false;

        if (_house != null)
            _house.RemoveResident(human);

        _house = house;
        return true;
    }


    private void OnDisable()
    {
        if (_house != null)
            _house.RemoveResident(GetComponent<HumanAgent>());
    }
}
