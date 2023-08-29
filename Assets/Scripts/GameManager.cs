using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;
    
    [SerializeField] int foodAmount;
    [SerializeField] int woodAmount;
    [SerializeField] int stoneAmount;
    [SerializeField] int maxStorageAmount;
    [SerializeField] int availableHouses;

    public List<Building> availableBuildings = new List<Building>();
    
    private void Awake() {
        if (sharedInstance == null) {
            sharedInstance = this;
        }
        foreach (Building building in FindObjectsOfType<Building>()) {
            if (building.IsAvailableForWork()) {
                availableBuildings.Add(building);
            }
        }
    }

    public int GetFoodAmount() {
        return foodAmount;
    }
    public int GetWoodAmount() {
        return woodAmount;
    }
    public int GetStoneAmount() {
        return stoneAmount;
    }

    public void AddFoodAmount(int Amount) {
        foodAmount += Amount;
    }
    public void RemoveFoodAmount(int Amount) {
        foodAmount -= Amount;
    }

    public void AddWoodAmount(int Amount) {
        woodAmount += Amount;
    }
    public void RemoveWoodAmount(int Amount) {
        woodAmount -= Amount;
    }
    public void AddStoneAmount(int Amount) {
        stoneAmount += Amount;
    }
    public void RemoveStoneAmount(int Amount) {
        stoneAmount -= Amount;
    }

    public int GetMaxStorageAmount() {
        return maxStorageAmount;
    }

    public void AddStorageAmount(int Value) {
        maxStorageAmount += Value;
    }

    public int GetVillagersAmount() {
        return FindObjectsOfType<Villager>().Length;
    }
    
    public void SetAvailableHouseAmount(int Amount) {
        availableHouses += Amount;
    }
    public int GetAvailableHouseAmount() {
        return availableHouses;
    }
}
