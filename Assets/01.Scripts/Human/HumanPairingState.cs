using UnityEngine;

[RequireComponent(typeof(HumanAgent))]
public class HumanPairingState : MonoBehaviour
{
    [SerializeField] private HumanAgent _partner;
    [SerializeField] private House _targetHouse;
    [SerializeField] private bool _isInitiator;
    [SerializeField] private bool _isAtPairingHouse;

    public HumanAgent Partner => _partner;
    public House TargetHouse => _targetHouse;
    public bool IsInitiator => _isInitiator;
    public bool IsAtPairingHouse => _isAtPairingHouse;
    public bool HasPairing => _partner != null && _targetHouse != null;

    public void SetPairing(HumanAgent partner, House targetHouse, bool isInitiator)
    {
        _partner = partner;
        _targetHouse = targetHouse;
        _isInitiator = isInitiator;
        _isAtPairingHouse = false;
    }

    public void SetArrivedAtPairingHouse()
    {
        _isAtPairingHouse = true;
    }

    public void ClearPairing()
    {
        _partner = null;
        _targetHouse = null;
        _isInitiator = false;
        _isAtPairingHouse = false;
    }
}
