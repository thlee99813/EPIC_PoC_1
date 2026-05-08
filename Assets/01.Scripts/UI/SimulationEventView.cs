using System;
using TMPro;
using UnityEngine;

public class SimulationEventView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _bodyText;

    public event Action Closed;

    private void Start()
    {
        Hide();
    }

    public void Show(string title, string body)
    {
        _titleText.text = title;
        _bodyText.text = body;
        _root.SetActive(true);
    }

    public void Close()
    {
        Hide();
        Closed?.Invoke();
    }

    public void Hide()
    {
        _root.SetActive(false);
    }
}
