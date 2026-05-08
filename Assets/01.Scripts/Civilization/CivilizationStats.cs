using UnityEngine;

public class CivilizationStats : MonoBehaviour
{
    [SerializeField] private int _faith = 3;
    [SerializeField] private int _bond = 3;
    [SerializeField] private int _order = 3;
    [SerializeField] private int _fear = 3;

    public int Faith => _faith;
    public int Bond => _bond;
    public int Order => _order;
    public int Fear => _fear;

    public void AddFaith(int amount)
    {
        _faith += amount;
    }

    public void AddBond(int amount)
    {
        _bond += amount;
    }

    public void AddOrder(int amount)
    {
        _order += amount;
    }

    public void AddFear(int amount)
    {
        _fear += amount;
    }
}
