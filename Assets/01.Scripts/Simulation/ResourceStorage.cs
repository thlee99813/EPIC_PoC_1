using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField] private int _food;
    [SerializeField] private int _wood;

    public int Food => _food;
    public int Wood => _wood;

    public void Add(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.Food)
            _food += amount;
        else if (resourceType == ResourceType.Wood)
            _wood += amount;
    }

    public int Take(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.Food)
        {
            int takenAmount = Mathf.Min(_food, amount);
            _food -= takenAmount;
            return takenAmount;
        }

        int takenWood = Mathf.Min(_wood, amount);
        _wood -= takenWood;
        return takenWood;
    }
    public int GetAmount(ResourceType resourceType)
    {
        if (resourceType == ResourceType.Food)
            return _food;

        return _wood;
    }

    public bool HasEnough(ResourceType resourceType, int amount)
    {
        return GetAmount(resourceType) >= amount;
    }


}
