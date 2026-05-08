using UnityEngine;

[CreateAssetMenu(fileName = "CaseDialogueData", menuName = "Epic/Dialogue/Case Dialogue Data")]
public class CaseDialogueData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _characterSprite;
    [SerializeField] private Sprite _dialogueBoxSprite;
    [SerializeField] private DialogueSequenceData _dialogueSequence;
    [SerializeField] private DoctrineType _doctrineType;
    [SerializeField, TextArea] private string _doctrineProposalText;

    public string Name => _name;
    public Sprite CharacterSprite => _characterSprite;
    public Sprite DialogueBoxSprite => _dialogueBoxSprite;
    public DialogueSequenceData DialogueSequence => _dialogueSequence;
    public DoctrineType DoctrineType => _doctrineType;
    public string DoctrineProposalText => _doctrineProposalText;

}
