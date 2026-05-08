using UnityEngine;

public class SimulationSpeedController : MonoBehaviour
{
    [SerializeField] private float _defaultSpeed = 1f;
    [SerializeField] private float _fastSpeed = 2f;
    [SerializeField] private float _fasterSpeed = 5f;
    [SerializeField] private float _maxSpeed = 10f;

    public float CurrentSpeed => Time.timeScale;

    private void Start()
    {
        SetDefaultSpeed();
    }

    public void SetPaused()
    {
        SetSpeed(0f);
    }

    public void SetDefaultSpeed()
    {
        SetSpeed(_defaultSpeed);
    }

    public void SetFastSpeed()
    {
        SetSpeed(_fastSpeed);
    }

    public void SetFasterSpeed()
    {
        SetSpeed(_fasterSpeed);
    }

    public void SetMaxSpeed()
    {
        SetSpeed(_maxSpeed);
    }

    public void SetSpeed(float speed)
    {
        Time.timeScale = speed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
