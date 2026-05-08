using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseDialogueView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Image _characterImage;
    [SerializeField] private Image _dialogueBoxImage;
    [SerializeField] private TMP_Text _nameTagText;
    [SerializeField] private DialogueSequencePlayer _dialogueSequencePlayer;

    public event Action Completed;

    private void OnEnable()
    {
        _dialogueSequencePlayer.Completed += OnDialogueCompleted;
    }

    private void OnDisable()
    {
        _dialogueSequencePlayer.Completed -= OnDialogueCompleted;
    }

    public void Show(CaseDialogueData data)
    {
        _root.SetActive(true);

        _characterImage.sprite = data.CharacterSprite;
        _dialogueBoxImage.sprite = data.DialogueBoxSprite;
        _nameTagText.text = data.Name;
        _dialogueSequencePlayer.Play(data.DialogueSequence);
    }

    public void Advance()
    {
        _dialogueSequencePlayer.Advance();
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    private void OnDialogueCompleted()
    {
        Completed?.Invoke();
    }
}
