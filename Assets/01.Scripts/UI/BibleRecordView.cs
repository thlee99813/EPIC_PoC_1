using TMPro;
using UnityEngine;

public class BibleRecordView : MonoBehaviour
{
    [SerializeField] private GameObject _recordRoot;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _bodyText;

    private void Start()
    {
        _recordRoot.SetActive(false);
    }

    public void SetRecord(string title, string body)
    {
        _titleText.text = title;
        _bodyText.text = body;
        _recordRoot.SetActive(true);
    }
}
