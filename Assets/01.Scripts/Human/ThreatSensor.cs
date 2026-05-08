using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class ThreatSensor : MonoBehaviour
{
    [SerializeField] private float _detectRange = 8f;

    private HumanAgent _agent;

    public EnemyAgent NearestEnemy { get; private set; }
    public bool HasThreat => NearestEnemy != null && !NearestEnemy.IsDead;

    private void Awake()
    {
        _agent = GetComponent<HumanAgent>();
    }

    public void Refresh()
    {
        NearestEnemy = _agent.TickSystem.GetNearestEnemy(transform.position, _detectRange);
    }
}
