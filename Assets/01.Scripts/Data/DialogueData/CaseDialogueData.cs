using UnityEngine;

[CreateAssetMenu(fileName = "CaseDialogueData", menuName = "Epic/Dialogue/Case Dialogue Data")]
public class CaseDialogueData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _characterSprite;
    [SerializeField] private Sprite _dialogueBoxSprite;
    [SerializeField] private DialogueSequenceData _dialogueSequence;

    public string Name => _name;
    public Sprite CharacterSprite => _characterSprite;
    public Sprite DialogueBoxSprite => _dialogueBoxSprite;
    public DialogueSequenceData DialogueSequence => _dialogueSequence;
}
