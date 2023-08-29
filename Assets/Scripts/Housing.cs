using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Housing : Building
{
    [SerializeField] int popsLiving = 0;
    [SerializeField] int maxPopsInHouse;
    [SerializeField] bool popsAdded = false;

    public List<Villager> villagersList = new List<Villager>();

    private void Start() {
        
    }
    public bool CanLiveInHouse() {
        return popsLiving < maxPopsInHouse;
    }

    protected override void CompleteConstruction()
    {
        base.CompleteConstruction();
        if (popsAdded) return;
        foreach (Villager villager in FindObjectsOfType<Villager>()) {
            if (!villagersList.Contains(villager) && !villager.GetHasHouse() && CanLiveInHouse()) {
                villagersList.Add(villager);
                villager.SetHasHouse(true, this);
                popsLiving++;
            }
        }
        popsAdded = true;
        GameManager.sharedInstance.SetAvailableHouseAmount(maxPopsInHouse);
    }
}
