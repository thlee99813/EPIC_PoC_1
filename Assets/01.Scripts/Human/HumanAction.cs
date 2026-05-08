using UnityEngine;

public abstract class HumanAction : MonoBehaviour
{
    protected HumanAgent Agent { get; private set; }

    public abstract int Priority { get; }
    public virtual string StatusText => GetType().Name;

    public void Initialize(HumanAgent agent)
    {
        Agent = agent;
    }

    public abstract bool CanRun();
    public abstract void Begin();
    public abstract bool Tick(float deltaTime);

    public virtual void End()
    {
    }

    protected bool MoveTo(Vector3 targetPosition, float deltaTime)
    {
        Transform agentTransform = Agent.transform;
        agentTransform.position = Vector3.MoveTowards(agentTransform.position, targetPosition, Agent.Definition.MoveSpeed * deltaTime);

        return Vector3.Distance(agentTransform.position, targetPosition) <= Agent.Definition.InteractDistance;
    }
}
