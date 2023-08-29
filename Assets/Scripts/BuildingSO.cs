using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuildingData")]
public class BuildingSO : ScriptableObject
{
    public Sprite buildingSprite;
    public GameObject buildingPrefab;
    public float constructionTime;
    public int stoneCost;
    public int lumberCost;
    
}
