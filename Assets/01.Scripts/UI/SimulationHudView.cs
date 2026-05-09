using TMPro;
using UnityEngine;

public class SimulationHudView : MonoBehaviour
{
    [SerializeField] private SettlementContext _settlementContext;

    [Header("Resource")]
    [SerializeField] private TMP_Text _woodText;
    [SerializeField] private TMP_Text _foodText;
    [SerializeField] private TMP_Text _humanCountText;

    [Header("Civilization")]
    [SerializeField] private TMP_Text _doctrineText;
    [SerializeField] private TMP_Text _faithText;
    [SerializeField] private TMP_Text _bondText;
    [SerializeField] private TMP_Text _orderText;
    [SerializeField] private TMP_Text _fearText;

    [Header("Refresh")]
    [SerializeField] private float _refreshInterval = 0.2f;

    private float _refreshTimer;

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;

        if (_refreshTimer > 0f)
            return;

        _refreshTimer = _refreshInterval;
        Refresh();
    }

    private void Refresh()
    {
        ResourceStorage storage = _settlementContext.Storage;
        CivilizationStats civilizationStats = _settlementContext.CivilizationStats;
        SettlementDoctrine doctrine = _settlementContext.SettlementDoctrine;
        SimulationTickSystem tickSystem = _settlementContext.TickSystem;

        _woodText.text = storage.Wood.ToString();
        _foodText.text = storage.Food.ToString();
        _humanCountText.text = tickSystem.AgentCount.ToString();

        _doctrineText.text = GetDoctrineText(doctrine.CurrentDoctrine);
        _faithText.text = civilizationStats.Faith.ToString();
        _bondText.text = civilizationStats.Bond.ToString();
        _orderText.text = civilizationStats.Order.ToString();
        _fearText.text = civilizationStats.Fear.ToString();
    }

    private string GetDoctrineText(DoctrineType doctrineType)
    {
        if (doctrineType == DoctrineType.OrderFirst)
            return "질서우선";

        if (doctrineType == DoctrineType.MercyFirst)
            return "구원우선";

        if (doctrineType == DoctrineType.FreedomFirst)
            return "계시우선";

        return "없음";
    }
}
