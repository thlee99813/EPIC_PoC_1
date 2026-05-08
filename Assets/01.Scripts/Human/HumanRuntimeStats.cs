using UnityEngine;

public class HumanRuntimeStats : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health;
    [SerializeField] private float _hunger;
    [SerializeField] private float _age;
    [SerializeField] private bool _isDead;
    [SerializeField] private float _reproductionCooldownTimer;
    


    public float Health => _health;
    public float Hunger => _hunger;
    public float Age => _age;
    public bool IsDead => _isDead;
    public float ReproductionCooldownTimer => _reproductionCooldownTimer;

    public void Initialize(HumanDefinition definition)
    {
        _health = definition.MaxHealth;
        _hunger = Random.Range(definition.InitialHungerRange.x, definition.InitialHungerRange.y);
        _age = Random.Range(definition.InitialAgeRange.x, definition.InitialAgeRange.y);
        _reproductionCooldownTimer = Random.Range(0f, definition.ReproductionCooldown);
        _isDead = false;
    }
    public void Initialize(HumanDefinition definition, float initialAge)
    {
        _health = definition.MaxHealth;
        _hunger = Random.Range(definition.InitialHungerRange.x, definition.InitialHungerRange.y);
        _age = initialAge;
        _reproductionCooldownTimer = 0f;
        _isDead = false;
    }


    public void Tick(HumanDefinition definition, float deltaTime)
    {
        _age += definition.AgeIncreasePerSecond * deltaTime;
        _hunger = Mathf.Min(_hunger + definition.HungerIncreasePerSecond * deltaTime, definition.MaxHunger);
        _reproductionCooldownTimer = Mathf.Max(0f, _reproductionCooldownTimer - deltaTime);

        if (_hunger >= definition.MaxHunger)
            _health -= definition.StarveDamagePerSecond * deltaTime;

        if (_health <= 0f || _age >= definition.MaxAge)
            _isDead = true;
    }
    public void RecoverHunger(float amount)
    {
        _hunger = Mathf.Max(0f, _hunger - amount);
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0f)
            _isDead = true;
    }

    public bool CanReproduce(HumanDefinition definition)
    {
        return !_isDead && _age >= definition.AdultAge && _reproductionCooldownTimer <= 0f;
    }

    public void StartReproductionCooldown(HumanDefinition definition)
    {
        _reproductionCooldownTimer = definition.ReproductionCooldown;
    }
    
}
