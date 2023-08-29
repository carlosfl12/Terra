using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : Building
{
    // A resource is a building that can grow over time

    public enum ResourceType {
        Food,
        Wood,
        Stone
    };

    [Header("Resource Properties")]
    public ResourceType resourceType;

    public Dictionary<ResourceType, int> resourceMap = new Dictionary<ResourceType, int>();

    public int maxResource = 100;
    public int currentResource;

    private void Start() {
        resourceMap.Add(resourceType, maxResource);
        currentResource = maxResource;
    }

    public void GiveResource(ResourceType Type, int Amount, Villager villager) {
        resourceMap[Type]-= Amount;
        currentResource = resourceMap[Type];
        if (currentResource <= 0) {
            foreach (Villager vill in FindObjectsOfType<Villager>()) {
                vill.currentState = Villager.VillagerState.Idle;
                vill.currentJob = Villager.VillagerJobs.None;
            }
            Destroy(gameObject);
        }
    }

    public void InteractWithVillager(Villager Villager) {
        if (resourceType == ResourceType.Food && Villager.currentJob != Villager.VillagerJobs.Farmer) {
            Villager.currentJob = Villager.VillagerJobs.Farmer;
        }
        else if (resourceType == ResourceType.Stone && Villager.currentJob != Villager.VillagerJobs.Miner) {
            Villager.currentJob = Villager.VillagerJobs.Miner;
        }
        else if (resourceType == ResourceType.Wood && Villager.currentJob != Villager.VillagerJobs.Lumberjack) {
            Villager.currentJob = Villager.VillagerJobs.Lumberjack;
        }
    }

    public int GetResourceCountOfType(ResourceType resourceType) {
        int count = 0;
        foreach (Resources resources in FindObjectsOfType<Resources>()) {
            if (resources.resourceType == resourceType) {
                count++;
            }
        }
        return count;
    }

}
