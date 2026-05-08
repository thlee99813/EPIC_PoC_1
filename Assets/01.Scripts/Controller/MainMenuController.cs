using UnityEngine;
using UnityEngine.InputSystem;


public class MainMenuController : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject _mainMenuCore;
    [SerializeField] private GameObject _observePanel;
    [SerializeField] private GameObject _biblePanel;

    [Header("Dialogue")]
    [SerializeField] private InputActionReference _advanceAction;

    [SerializeField] private CaseDialogueView _caseDialogueView;
    [SerializeField] private CaseDialogueData _celestiaDialogueData;
    [SerializeField] private CaseDialogueData _beatriceDialogueData;
    [SerializeField] private CaseDialogueData _leticiaDialogueData;

    private GameObject _currentActivePanel;

    private bool _isCaseDialogueOpen;
    private float _caseDialogueInputUnlockTime;

    private void OnEnable()
    {
        _advanceAction.action.performed += OnAdvancePerformed;
        _advanceAction.action.Enable();

        _caseDialogueView.Completed += CloseCaseDialogue;
    }

    private void OnDisable()
    {
        _advanceAction.action.performed -= OnAdvancePerformed;
        _advanceAction.action.Disable();

        _caseDialogueView.Completed -= CloseCaseDialogue;
    }

    private void OnAdvancePerformed(InputAction.CallbackContext context)
    {
        if (!_isCaseDialogueOpen)
            return;

        if (Time.unscaledTime < _caseDialogueInputUnlockTime)
            return;

        _caseDialogueView.Advance();
    }

    private void Start()
    {
        _observePanel.SetActive(false);
        _biblePanel.SetActive(false);
        _caseDialogueView.Hide();
    }

    public void ToggleObservePanel()
    {
        TogglePanel(_observePanel);
    }

    public void ToggleBiblePanel()
    {
        TogglePanel(_biblePanel);
    }

    public void OpenCelestiaDialogue()
    {
        OpenCaseDialogue(_celestiaDialogueData);
    }

    public void OpenBeatriceDialogue()
    {
        OpenCaseDialogue(_beatriceDialogueData);
    }

    public void OpenLeticiaDialogue()
    {
        OpenCaseDialogue(_leticiaDialogueData);
    }

    public void CloseCaseDialogue()
    {
        _isCaseDialogueOpen = false;
        _caseDialogueView.Hide();
        _mainMenuCore.SetActive(true);
    }


    private void TogglePanel(GameObject targetPanel)
    {
        bool isSamePanelActive = _currentActivePanel == targetPanel && targetPanel.activeSelf;

        CloseContentPanels();

        if (isSamePanelActive)
        {
            _currentActivePanel = null;
            return;
        }

        targetPanel.SetActive(true);
        _currentActivePanel = targetPanel;
    }

    private void OpenCaseDialogue(CaseDialogueData dialogueData)
    {
        CloseContentPanels();

        _mainMenuCore.SetActive(false);
        _isCaseDialogueOpen = true;
        _caseDialogueInputUnlockTime = Time.unscaledTime + 0.1f;

        _caseDialogueView.Show(dialogueData);
    }


    private void CloseContentPanels()
    {
        _observePanel.SetActive(false);
        _biblePanel.SetActive(false);
        _caseDialogueView.Hide();

        _currentActivePanel = null;
    }
}
