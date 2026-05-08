using TMPro;
using UnityEngine;

public class AgentStatusLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 1.5f, 0f);

    private HumanAgent _target;
    private Camera _camera;

    public void Initialize(HumanAgent target, Camera targetCamera)
    {
        _target = target;
        _camera = targetCamera;
        gameObject.SetActive(true);
    }

    public void Release()
    {
        _target = null;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 screenPosition = _camera.WorldToScreenPoint(_target.transform.position + _worldOffset);
        transform.position = screenPosition;

        _statusText.text = _target.CurrentActionName;
    }
}
