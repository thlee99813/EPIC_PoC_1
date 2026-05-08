using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyAgent _enemyPrefab;
    [SerializeField] private SimulationTickSystem _tickSystem;
    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private float _initialDelay = 60f;
    [SerializeField] private float _spawnInterval = 30f;
    [SerializeField] private int _minHumanCountToSpawn = 6;

    [SerializeField] private int _baseMaxEnemyCount = 1;
    [SerializeField] private int _humanCountPerExtraEnemy = 5;
    [SerializeField] private int _absoluteMaxEnemyCount = 5;

    private float _spawnTimer;
    private bool _isInitialDelayFinished;

    private void Start()
    {
        _spawnTimer = _initialDelay;
    }

    private void Update()
    {
        if (_tickSystem.AgentCount < _minHumanCountToSpawn)
            return;

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer > 0f)
            return;

        _spawnTimer = _spawnInterval;

        if (_tickSystem.EnemyCount >= GetCurrentMaxEnemyCount())
            return;

        SpawnEnemy();
    }

    private int GetCurrentMaxEnemyCount()
    {
        int extraHumanCount = Mathf.Max(0, _tickSystem.AgentCount - _minHumanCountToSpawn);
        int extraEnemyCount = extraHumanCount / _humanCountPerExtraEnemy;
        int maxEnemyCount = _baseMaxEnemyCount + extraEnemyCount;

        return Mathf.Min(maxEnemyCount, _absoluteMaxEnemyCount);
    }


    private void SpawnEnemy()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        EnemyAgent enemy = Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.Initialize(_tickSystem);

    }
}
