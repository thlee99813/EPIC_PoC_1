using System.Collections.Generic;
using UnityEngine;

public class AgentStatusLabelSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private AgentStatusLabel _labelPrefab;
    [SerializeField] private Transform _labelRoot;
    [SerializeField] private int _maxVisibleLabels = 50;

    private readonly Dictionary<HumanAgent, AgentStatusLabel> _activeLabels = new Dictionary<HumanAgent, AgentStatusLabel>();
    private readonly Queue<AgentStatusLabel> _labelPool = new Queue<AgentStatusLabel>();

    public void ShowLabel(HumanAgent agent)
    {
        if (_activeLabels.ContainsKey(agent))
            return;

        if (_activeLabels.Count >= _maxVisibleLabels)
            return;

        AgentStatusLabel label = GetLabel();
        label.Initialize(agent, _camera);

        _activeLabels.Add(agent, label);
    }

    public void HideLabel(HumanAgent agent)
    {
        if (!_activeLabels.TryGetValue(agent, out AgentStatusLabel label))
            return;

        label.Release();
        _activeLabels.Remove(agent);
        _labelPool.Enqueue(label);
    }

    private AgentStatusLabel GetLabel()
    {
        if (_labelPool.Count > 0)
            return _labelPool.Dequeue();

        return Instantiate(_labelPrefab, _labelRoot);
    }
}
