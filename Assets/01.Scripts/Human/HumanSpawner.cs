using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    [SerializeField] private HumanAgent _humanPrefab;
    [SerializeField] private SettlementContext _settlementContext;
    [SerializeField] private SimulationTickSystem _tickSystem;
    [SerializeField] private AgentStatusLabelSystem _statusLabelSystem;

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _spawnCount = 1;

    private void Start()
    {
        SpawnInitialHumans();
    }

    public HumanAgent SpawnHuman(Vector3 position)
    {
        HumanAgent human = Instantiate(_humanPrefab, position, Quaternion.identity);
        human.Initialize(_settlementContext, _tickSystem);
        _statusLabelSystem.ShowLabel(human);
        return human;
    }


    private void SpawnInitialHumans()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Transform spawnPoint = _spawnPoints[i % _spawnPoints.Length];
            SpawnHuman(spawnPoint.position);
        }
    }
    public HumanAgent SpawnChild(Vector3 position)
    {
        HumanAgent human = Instantiate(_humanPrefab, position, Quaternion.identity);
        human.Initialize(_settlementContext, _tickSystem, 0f);
        _statusLabelSystem.ShowLabel(human);
        return human;
    }

}
