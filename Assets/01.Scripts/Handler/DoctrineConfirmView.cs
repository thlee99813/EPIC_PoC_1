using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoctrineConfirmView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _declineButton;
    [SerializeField] private float _charsPerSecond = 30f;

    public event Action Accepted;
    public event Action Declined;

    private Coroutine _typingCoroutine;

    private void OnEnable()
    {
        _acceptButton.onClick.AddListener(OnAcceptClicked);
        _declineButton.onClick.AddListener(OnDeclineClicked);
    }

    private void OnDisable()
    {
        _acceptButton.onClick.RemoveListener(OnAcceptClicked);
        _declineButton.onClick.RemoveListener(OnDeclineClicked);
    }

    public void Show(string text)
    {
        _root.SetActive(true);
        _acceptButton.interactable = false;
        _declineButton.interactable = false;

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        TextUtil.Clear(_text);
        yield return TextUtil.TypeText(_text, text, _charsPerSecond);

        _typingCoroutine = null;
        _acceptButton.interactable = true;
        _declineButton.interactable = true;
    }

    private void OnAcceptClicked()
    {
        Accepted?.Invoke();
    }

    private void OnDeclineClicked()
    {
        Declined?.Invoke();
    }
}
