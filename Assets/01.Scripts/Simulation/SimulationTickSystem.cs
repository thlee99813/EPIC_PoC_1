using System.Collections.Generic;
using UnityEngine;

public class SimulationTickSystem : MonoBehaviour
{
    [SerializeField] private int _agentsPerFrame = 30;

    private readonly List<HumanAgent> _agents = new List<HumanAgent>();
    private readonly Dictionary<HumanAgent, float> _lastTickTimes = new Dictionary<HumanAgent, float>();

    private int _nextAgentIndex;
    public int AgentCount => _agents.Count;
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

    private void Update()
    {
        if (_agents.Count == 0)
            return;

        int tickCount = Mathf.Min(_agentsPerFrame, _agents.Count);

        for (int i = 0; i < tickCount; i++)
            TickNextAgent();
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
}
