using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType {
        Resources,
        Housing,
        Entertainment,
        Storaging
    }

    [Header("Building Properties")]
    public float constructionTime = 10f;
    [SerializeField] float currentConstructionProgress;
    [SerializeField] bool underConstruction;
    public BuildingType buildingType;

    private void Start() {
        underConstruction = true;
        currentConstructionProgress = 0f;
        GameManager.sharedInstance.availableBuildings.Add(this);
    }

    protected virtual void CompleteConstruction() {
        underConstruction = false;
        GameManager.sharedInstance.availableBuildings.Remove(this);
    }

    public bool IsUnderConstruction() {
        return underConstruction;
    }
    public bool IsAvailableForWork()
    {
        if (currentConstructionProgress >= constructionTime || buildingType == BuildingType.Resources) {
            CompleteConstruction();
            return false;
        }
        Debug.Log("<color=green> " + gameObject.name +  " Is available to build </color>");
        underConstruction = true;
        return true;
    }

    public void IncreaseConstructionProgress(float value) {
        if (underConstruction) {
            currentConstructionProgress += value;
        }
    }
}
