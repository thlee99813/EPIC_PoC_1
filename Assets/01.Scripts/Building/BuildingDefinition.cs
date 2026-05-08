using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDefinition", menuName = "Epic/Simulation/Building Definition")]
public class BuildingDefinition : ScriptableObject
{
    [SerializeField] private string _displayName = "집";
    [SerializeField] private int _woodCost = 10;
    [SerializeField] private float _buildWorkRequired = 10f;
    [SerializeField] private float _interactionRadius = 2f;

    public string DisplayName => _displayName;
    public int WoodCost => _woodCost;
    public float BuildWorkRequired => _buildWorkRequired;
    public float InteractionRadius => _interactionRadius;
}
