using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : Building
{
    [SerializeField] int maxStorage = 5000;
    [SerializeField] bool resourcesGiven = false;


    protected override void CompleteConstruction()
    {
        base.CompleteConstruction();
        if (resourcesGiven) return;
        GameManager.sharedInstance.AddStorageAmount(maxStorage);
        resourcesGiven = true;
        
    }

}
