using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyDialogueController : MonoBehaviour
{
    [SerializeField] private DialogueSequencePlayer _dialogueSequencePlayer;
    [SerializeField] private InputActionReference _advanceAction;
    [SerializeField] private string _nextSceneName;

    private void OnEnable()
    {
        _advanceAction.action.performed += OnAdvancePerformed;
        _advanceAction.action.Enable();

        _dialogueSequencePlayer.Completed += OnDialogueCompleted;
    }

    private void OnDisable()
    {
        _advanceAction.action.performed -= OnAdvancePerformed;
        _advanceAction.action.Disable();

        _dialogueSequencePlayer.Completed -= OnDialogueCompleted;
    }

    private void OnAdvancePerformed(InputAction.CallbackContext context)
    {
        _dialogueSequencePlayer.Advance();
    }

    private void OnDialogueCompleted()
    {
        SceneTransitionManager.Instance.LoadScene(_nextSceneName);
    }
}
