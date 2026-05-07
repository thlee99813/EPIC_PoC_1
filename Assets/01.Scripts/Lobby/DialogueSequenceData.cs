using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequenceData", menuName = "Epic/Dialogue/Dialogue Sequence Data")]
public class DialogueSequenceData : ScriptableObject
{
    [SerializeField] private TextAsset _sourceText;

    [SerializeField, TextArea(2, 5)]
    private string[] _lines;

    public string[] Lines => _lines;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_sourceText == null)
            return;

        _lines = _sourceText.text.Split(
        new[] { "\r\n\r\n", "\n\n" },
        StringSplitOptions.RemoveEmptyEntries);
    }
#endif
}
