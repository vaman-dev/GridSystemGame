using UnityEngine;

[CreateAssetMenu(fileName = "BuildingType", menuName = "GridBuilding/BuildingType")]
public class BuildingTypeSO : ScriptableObject
{
    [Header("Basic Info")]
    public string buildingName;
    public GameObject prefab;

    [Header("Tile Size (Footprint)")]
    public int tileWidth = 1;     // tiles wide
    public int tileHeight = 1;    // tiles tall

    [Header("Category")]
    public BuildingCategory category;

    [Header("Placement Rule Strategy")]
    public PlacementRuleType placementRuleType; // selects rule
}

public enum BuildingCategory
{
    Default,
    Structure,
    Road,
    Decoration,
    Defence
}

public enum PlacementRuleType
{
    SingleTile,
    MultiTile,
    RoadTile,
    DecorationTile
}
