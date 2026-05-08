using UnityEngine;

public class SimulationObservationController : MonoBehaviour
{
    [SerializeField] private SimulationSpeedController _speedController;
    [SerializeField] private FirstTabooEventSystem _firstTabooEventSystem;

    private bool _hasStarted;

    public bool HasStarted => _hasStarted;

    public void StartObservation()
    {
        if (_hasStarted)
            return;

        _hasStarted = true;
        _speedController.SetDefaultSpeed();
        _firstTabooEventSystem.StartObservation();
    }
}
