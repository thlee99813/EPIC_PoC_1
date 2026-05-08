using UnityEngine;

[CreateAssetMenu(fileName = "HumanDefinition", menuName = "Epic/Simulation/Human Definition")]
public class HumanDefinition : ScriptableObject
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _maxHunger = 100f;
    [SerializeField] private Vector2 _initialHungerRange = new Vector2(0f, 30f);
    [SerializeField] private float _hungerIncreasePerSecond = 0.3f;
    [SerializeField] private float _starveDamagePerSecond = 5f;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _gatherDuration = 2f;
    [SerializeField] private int _carryCapacity = 3;
    [SerializeField] private int _eatAmount = 2;
    [SerializeField] private float _eatDuration = 1.5f;
    [SerializeField] private float _eatHungerRecovery = 40f;
    [SerializeField] private float _decisionInterval = 0.5f;
    [SerializeField] private float _interactDistance = 0.2f;

    [SerializeField] private Vector2 _initialAgeRange = new Vector2(18f, 35f);
    [SerializeField] private float _adultAge = 18f;
    [SerializeField] private float _maxAge = 70f;
    [SerializeField] private float _ageIncreasePerSecond = 0.01f;

    [SerializeField] private int _reproductionFoodCost = 5;
    [SerializeField] private float _reproductionDuration = 8f;
    [SerializeField] private float _reproductionCooldown = 30f;


    public float MaxHealth => _maxHealth;
    public float MaxHunger => _maxHunger;
    public float HungerIncreasePerSecond => _hungerIncreasePerSecond;
    public float StarveDamagePerSecond => _starveDamagePerSecond;
    public float MoveSpeed => _moveSpeed;
    public float GatherDuration => _gatherDuration;
    public int CarryCapacity => _carryCapacity;
    public int EatAmount => _eatAmount;
    public float EatDuration => _eatDuration;
    public float EatHungerRecovery => _eatHungerRecovery;

    public float DecisionInterval => _decisionInterval;
    public float InteractDistance => _interactDistance;
    public Vector2 InitialHungerRange => _initialHungerRange;

    public Vector2 InitialAgeRange => _initialAgeRange;
    public float AdultAge => _adultAge;
    public float MaxAge => _maxAge;
    public float AgeIncreasePerSecond => _ageIncreasePerSecond;

    public int ReproductionFoodCost => _reproductionFoodCost;
    public float ReproductionDuration => _reproductionDuration;
    public float ReproductionCooldown => _reproductionCooldown;


}
