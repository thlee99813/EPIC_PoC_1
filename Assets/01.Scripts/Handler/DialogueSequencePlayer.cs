using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSequencePlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _nextArrow;
    [SerializeField] private DialogueSequenceData _dialogueSequence;
    [SerializeField] private float _charsPerSecond = 30f;
    [SerializeField] private bool _playOnStart = true;

    public event Action Completed;

    private int _currentLineIndex;
    private bool _isTyping;
    private bool _isCompleted;
    private Coroutine _typingCoroutine;

    private void Start()
    {
        SetNextArrow(false);

        if (_playOnStart)
            ShowNextLine();
    }

    public void Advance()
    {
        if (_isCompleted)
            return;

        if (_isTyping)
        {
            CompleteCurrentLine();
            return;
        }

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        SetNextArrow(false);

        if (_currentLineIndex >= _dialogueSequence.Lines.Length)
        {
            CompleteDialogue();
            return;
        }

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeLine(_dialogueSequence.Lines[_currentLineIndex]));
        _currentLineIndex++;
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;

        yield return TextUtil.TypeText(_dialogueText, line, _charsPerSecond);

        _isTyping = false;
        _typingCoroutine = null;

        if (_currentLineIndex >= _dialogueSequence.Lines.Length)
        {
            CompleteDialogue();
            yield break;
        }

        SetNextArrow(true);
    }

    private void CompleteCurrentLine()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        TextUtil.ShowAll(_dialogueText);
        _isTyping = false;

        if (_currentLineIndex >= _dialogueSequence.Lines.Length)
        {
            CompleteDialogue();
            return;
        }

        SetNextArrow(true);
    }

    private void CompleteDialogue()
    {
        if (_isCompleted)
            return;

        _isCompleted = true;
        SetNextArrow(false);
        Completed?.Invoke();
    }

    private void SetNextArrow(bool isActive)
    {
        _nextArrow.SetActive(isActive);
    }
}
