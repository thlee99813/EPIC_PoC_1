using UnityEngine;
using UnityEngine.InputSystem;


public class MainMenuController : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject _mainPanel;

    [SerializeField] private GameObject _mainMenuCore;
    [SerializeField] private GameObject _observePanel;
    [SerializeField] private GameObject _biblePanel;

    [Header("Dialogue")]
    [SerializeField] private InputActionReference _advanceAction;

    [SerializeField] private CaseDialogueView _caseDialogueView;
    [SerializeField] private CaseDialogueData _celestiaDialogueData;
    [SerializeField] private CaseDialogueData _beatriceDialogueData;
    [SerializeField] private CaseDialogueData _leticiaDialogueData;
    [SerializeField] private DoctrineConfirmView _doctrineConfirmView;
    [SerializeField] private SettlementDoctrine _settlementDoctrine;

    private CaseDialogueData _currentDialogueData;
    private bool _hasEnteredObserve;


    private GameObject _currentActivePanel;

    private bool _isCaseDialogueOpen;
    private float _caseDialogueInputUnlockTime;

    private void OnEnable()
    {
        _advanceAction.action.performed += OnAdvancePerformed;
        _advanceAction.action.Enable();

        _caseDialogueView.Completed += OnCaseDialogueCompleted;
        _doctrineConfirmView.Accepted += OnDoctrineAccepted;
        _doctrineConfirmView.Declined += OnDoctrineDeclined;

    }

    private void OnDisable()
    {
        _advanceAction.action.performed -= OnAdvancePerformed;
        _advanceAction.action.Disable();

        _caseDialogueView.Completed -= OnCaseDialogueCompleted;
        _doctrineConfirmView.Accepted -= OnDoctrineAccepted;
        _doctrineConfirmView.Declined -= OnDoctrineDeclined;

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
        _doctrineConfirmView.Hide();

    }
    public void ToggleMainPanel()
    {
        TogglePanel(_mainPanel);
        _mainPanel.SetActive(false);
    }

    public void ToggleObservePanel()
    {
        _hasEnteredObserve = true;
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
        _currentDialogueData = dialogueData;
        _caseDialogueView.Show(dialogueData);
    }


    private void CloseContentPanels()
    {
        _observePanel.SetActive(false);
        _biblePanel.SetActive(false);
        _caseDialogueView.Hide();

        _currentActivePanel = null;
    }
    private void OnCaseDialogueCompleted()
    {
        if (!_hasEnteredObserve && _settlementDoctrine.CurrentDoctrine == DoctrineType.None)
        {
            _doctrineConfirmView.Show(_currentDialogueData.DoctrineProposalText);
            return;
        }

        CloseCaseDialogue();
    }

    private void OnDoctrineAccepted()
    {
        _settlementDoctrine.SetDoctrine(_currentDialogueData.DoctrineType);
        _doctrineConfirmView.Hide();
        CloseCaseDialogue();
    }

    private void OnDoctrineDeclined()
    {
        _doctrineConfirmView.Hide();
        CloseCaseDialogue();
    }

}
