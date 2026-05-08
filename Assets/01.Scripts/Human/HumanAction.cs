using UnityEngine;

public abstract class HumanAction : MonoBehaviour
{
    protected HumanAgent Agent { get; private set; }

    public abstract int Priority { get; }
    public virtual string StatusText => GetType().Name;
    public virtual bool CanBeInterrupted => false;


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

        Vector3 currentPosition = agentTransform.position;
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);

        agentTransform.position = Vector3.MoveTowards(currentPosition, flatTargetPosition, Agent.Definition.MoveSpeed * deltaTime);

        return Vector3.Distance(agentTransform.position, flatTargetPosition) <= Agent.Definition.InteractDistance;
    }

}
