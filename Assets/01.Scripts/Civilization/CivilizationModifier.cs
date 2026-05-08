using UnityEngine;

public class CivilizationModifier : MonoBehaviour
{
    [SerializeField] private CivilizationStats _stats;
    [SerializeField] private SettlementDoctrine _doctrine;

    public float GatherChanceMultiplier => GetGatherChanceMultiplier();
    public float BuildChanceMultiplier => GetBuildChanceMultiplier();
    public float PairingChanceMultiplier => GetPairingChanceMultiplier();
    public float FleeChanceMultiplier => GetFleeChanceMultiplier();
    public float FightChanceMultiplier => GetFightChanceMultiplier();

    private float GetGatherChanceMultiplier()
    {
        float multiplier = 1f;

        if (_doctrine.CurrentDoctrine == DoctrineType.OrderFirst)
            multiplier += 0.1f;

        if (_doctrine.CurrentDoctrine == DoctrineType.FreedomFirst)
            multiplier += 0.15f;

        multiplier += _orderToMultiplier(_stats.Order, 0.03f);

        return Mathf.Clamp(multiplier, 0.5f, 1.5f);
    }

    private float GetBuildChanceMultiplier()
    {
        float multiplier = 1f;

        if (_doctrine.CurrentDoctrine == DoctrineType.OrderFirst)
            multiplier += 0.2f;

        multiplier += _orderToMultiplier(_stats.Order, 0.04f);

        return Mathf.Clamp(multiplier, 0.5f, 1.7f);
    }

    private float GetPairingChanceMultiplier()
    {
        float multiplier = 1f;

        if (_doctrine.CurrentDoctrine == DoctrineType.MercyFirst)
            multiplier += 0.2f;

        multiplier += _orderToMultiplier(_stats.Bond, 0.05f);
        multiplier -= _orderToMultiplier(_stats.Fear, 0.03f);

        return Mathf.Clamp(multiplier, 0.4f, 1.8f);
    }

    private float GetFleeChanceMultiplier()
    {
        float multiplier = 1f;

        if (_doctrine.CurrentDoctrine == DoctrineType.MercyFirst)
            multiplier += 0.1f;

        multiplier += _orderToMultiplier(_stats.Fear, 0.08f);

        return Mathf.Clamp(multiplier, 0.5f, 2f);
    }

    private float GetFightChanceMultiplier()
    {
        float multiplier = 1f;

        if (_doctrine.CurrentDoctrine == DoctrineType.OrderFirst)
            multiplier += 0.15f;

        if (_doctrine.CurrentDoctrine == DoctrineType.FreedomFirst)
            multiplier += 0.1f;

        multiplier += _orderToMultiplier(_stats.Order, 0.03f);
        multiplier -= _orderToMultiplier(_stats.Fear, 0.04f);

        return Mathf.Clamp(multiplier, 0.3f, 1.8f);
    }

    private float _orderToMultiplier(int value, float scale)
    {
        return (value - 3) * scale;
    }
}
