using System.Collections.Generic;
using UnityEngine;

public class SimulationTickSystem : MonoBehaviour
{
    [SerializeField] private int _agentsPerFrame = 30;

    private readonly List<HumanAgent> _agents = new List<HumanAgent>();
    private readonly Dictionary<HumanAgent, float> _lastTickTimes = new Dictionary<HumanAgent, float>();

    private int _nextAgentIndex;
    public int AgentCount => _agents.Count;
    public int EnemyCount => _enemies.Count;

    private readonly List<EnemyAgent> _enemies = new List<EnemyAgent>();
    private readonly Dictionary<EnemyAgent, float> _lastEnemyTickTimes = new Dictionary<EnemyAgent, float>();
    private int _nextEnemyIndex;
    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        int tickCount = Mathf.Min(_agentsPerFrame, _agents.Count);

        for (int i = 0; i < tickCount; i++)
            TickNextAgent();

        int enemyTickCount = Mathf.Min(_agentsPerFrame, _enemies.Count);

        for (int i = 0; i < enemyTickCount; i++)
            TickNextEnemy();
    }


    

    public int GetAdultAgentCount(HumanDefinition definition)
    {
        int count = 0;

        for (int i = 0; i < _agents.Count; i++)
        {
            HumanAgent agent = _agents[i];

            if (!agent.Stats.IsDead && agent.Stats.Age >= definition.AdultAge)
                count++;
        }

        return count;
    }
    public HumanAgent GetAvailablePairingPartner(HumanAgent requester, HumanDefinition definition)
    {
        for (int i = 0; i < _agents.Count; i++)
        {
            HumanAgent agent = _agents[i];

            if (agent == requester)
                continue;

            if (agent.Stats.IsDead)
                continue;

            if (!agent.Stats.CanReproduce(definition))
                continue;

            HumanPairingState pairingState = agent.GetComponent<HumanPairingState>();

            if (pairingState.HasPairing)
                continue;

            return agent;
        }

        return null;
    }



    public void Register(HumanAgent agent)
    {
        if (_agents.Contains(agent))
            return;

        _agents.Add(agent);
        _lastTickTimes.Add(agent, Time.time);
    }

    public void Unregister(HumanAgent agent)
    {
        int removedIndex = _agents.IndexOf(agent);

        if (removedIndex < 0)
            return;

        _agents.RemoveAt(removedIndex);
        _lastTickTimes.Remove(agent);

        if (_nextAgentIndex > removedIndex)
            _nextAgentIndex--;

        if (_nextAgentIndex >= _agents.Count)
            _nextAgentIndex = 0;
    }

    private void TickNextAgent()
    {
        if (_nextAgentIndex >= _agents.Count)
            _nextAgentIndex = 0;

        HumanAgent agent = _agents[_nextAgentIndex];
        float currentTime = Time.time;
        float deltaTime = currentTime - _lastTickTimes[agent];

        _lastTickTimes[agent] = currentTime;
        _nextAgentIndex++;

        agent.SimulationTick(deltaTime);
    }
    public void RegisterEnemy(EnemyAgent enemy)
    {
        if (_enemies.Contains(enemy))
            return;

        _enemies.Add(enemy);
        _lastEnemyTickTimes.Add(enemy, Time.time);
    }

    public void UnregisterEnemy(EnemyAgent enemy)
    {
        int removedIndex = _enemies.IndexOf(enemy);

        if (removedIndex < 0)
            return;

        _enemies.RemoveAt(removedIndex);
        _lastEnemyTickTimes.Remove(enemy);

        if (_nextEnemyIndex > removedIndex)
            _nextEnemyIndex--;

        if (_nextEnemyIndex >= _enemies.Count)
            _nextEnemyIndex = 0;
    }

    public HumanAgent GetNearestHuman(Vector3 position, float range)
    {
        HumanAgent nearestHuman = null;
        float nearestDistance = range * range;

        for (int i = 0; i < _agents.Count; i++)
        {
            HumanAgent human = _agents[i];

            if (human.Stats.IsDead)
                continue;

            float distance = Vector3.SqrMagnitude(human.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestHuman = human;
        }

        return nearestHuman;
    }

    public EnemyAgent GetNearestEnemy(Vector3 position, float range)
    {
        EnemyAgent nearestEnemy = null;
        float nearestDistance = range * range;

        for (int i = 0; i < _enemies.Count; i++)
        {
            EnemyAgent enemy = _enemies[i];

            if (enemy.IsDead)
                continue;

            float distance = Vector3.SqrMagnitude(enemy.transform.position - position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestEnemy = enemy;
        }

        return nearestEnemy;
    }
    private void TickNextEnemy()
    {
        if (_nextEnemyIndex >= _enemies.Count)
            _nextEnemyIndex = 0;

        EnemyAgent enemy = _enemies[_nextEnemyIndex];
        float currentTime = Time.time;
        float deltaTime = currentTime - _lastEnemyTickTimes[enemy];

        _lastEnemyTickTimes[enemy] = currentTime;
        _nextEnemyIndex++;

        enemy.SimulationTick(deltaTime);
    }


}
