using UnityEngine;

public class SimulationMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _returnButton;
    private void Start()
    {
        _returnButton.SetActive(false);
    }
    private void OpenPanel()
    {
        _returnButton.SetActive(true);
    }
    public void ReturnToMain()
    {
        

    }

}
