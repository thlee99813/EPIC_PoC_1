using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Epic/Simulation/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [SerializeField] private float _maxHealth = 40f;
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _attackRange = 1.2f;
    [SerializeField] private float _attackDamage = 15f;
    [SerializeField] private float _attackInterval = 1.5f;
    [SerializeField] private float _detectRange = 12f;

    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float AttackRange => _attackRange;
    public float AttackDamage => _attackDamage;
    public float AttackInterval => _attackInterval;
    public float DetectRange => _detectRange;
}
