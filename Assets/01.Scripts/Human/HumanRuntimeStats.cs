using UnityEngine;

public class HumanRuntimeStats : MonoBehaviour
{
    [SerializeField] private float _health;
    [SerializeField] private float _hunger;
    [SerializeField] private float _age;
    [SerializeField] private bool _isDead;

    public float Health => _health;
    public float Hunger => _hunger;
    public float Age => _age;
    public bool IsDead => _isDead;

    public void Initialize(HumanDefinition definition)
    {
        _health = definition.MaxHealth;
        _hunger = Random.Range(definition.InitialHungerRange.x, definition.InitialHungerRange.y);
        _age = 0f;
        _isDead = false;
    }

    public void Tick(HumanDefinition definition, float deltaTime)
    {
        _hunger = Mathf.Min(_hunger + definition.HungerIncreasePerSecond * deltaTime, definition.MaxHunger);

        if (_hunger >= definition.MaxHunger)
            _health -= definition.StarveDamagePerSecond * deltaTime;

        if (_health <= 0f)
            _isDead = true;
    }

    public void RecoverHunger(float amount)
    {
        _hunger = Mathf.Max(0f, _hunger - amount);
    }
}
