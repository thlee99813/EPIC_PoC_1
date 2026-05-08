using UnityEngine;

public class SettlementDoctrine : MonoBehaviour
{
    [SerializeField] private DoctrineType _currentDoctrine = DoctrineType.None;

    public DoctrineType CurrentDoctrine => _currentDoctrine;

    public void SetDoctrine(DoctrineType doctrine)
    {
        _currentDoctrine = doctrine;
    }

    public void SetOrderFirst()
    {
        SetDoctrine(DoctrineType.OrderFirst);
    }

    public void SetMercyFirst()
    {
        SetDoctrine(DoctrineType.MercyFirst);
    }

    public void SetFreedomFirst()
    {
        SetDoctrine(DoctrineType.FreedomFirst);
    }
}
